using System;
using System.Collections;
using System.Collections.Generic; // Queue<T> 사용을 위해 추가
using Type;
using Type.Enums.GamePlay;
using Type.Utils;
using UnityEngine;

public class MSCell : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject flag;
    [SerializeField] private GameObject backIcon;

    private const float RAY_DISTANCE = 0.5f;

    // 비트 마스크 상수
    private const byte MASK_FLAGGED = 1 << 0;
    private const byte MASK_OPEN = 1 << 1;
    private const byte MASK_EXPLORED = 1 << 2;
    private const byte MASK_DRAG_WHILE_CLICK = 1 << 3;

    private Action adf;

    [SerializeField]
    private MineSweeperTiles state;
    private byte _flag;

    public MineSweeperTiles TileState => state;

    public bool IsFlagged
    {
        get => (_flag & MASK_FLAGGED) != 0;
        private set => _flag = (byte)(value ? (_flag | MASK_FLAGGED) : (_flag & ~MASK_FLAGGED));
    }

    public bool IsOpen
    {
        get => (_flag & MASK_OPEN) != 0;
        private set => _flag = (byte)(value ? (_flag | MASK_OPEN) : (_flag & ~MASK_OPEN));
    }

    public bool IsExplored
    {
        get => (_flag & MASK_EXPLORED) != 0;
        internal set => _flag = (byte)(value ? (_flag | MASK_EXPLORED) : (_flag & ~MASK_EXPLORED)); // BFS 확산을 위해 internal set으로 변경 (원할 경우 private 유지)
    }

    public bool IsDragWhileClick
    {
        get => (_flag & MASK_DRAG_WHILE_CLICK) != 0;
        private set => _flag = (byte)(value ? (_flag | MASK_DRAG_WHILE_CLICK) : (_flag & ~MASK_DRAG_WHILE_CLICK));
    }


    public void Bind(sbyte targetState)
    {
        state = (MineSweeperTiles)targetState;
        flag.SetActive(false);

        if (state == MineSweeperTiles.Mine) return;

        SpritePlus spritePlus;
        if (state == MineSweeperTiles.First)
        {
            MSSpriteTable.Instance.GetSpritePlus(state, out spritePlus);
            GetComponent<SpriteRenderer>().sprite = spritePlus.sprite;
            GetComponent<SpriteRenderer>().color = spritePlus.color;

            Destroy(backIcon);
            state = MineSweeperTiles.Empty;

            return;
        }

        if (MSSpriteTable.Instance.GetSpritePlus(state, out spritePlus))
        {
            SpriteRenderer spriteRenderer = backIcon.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = spritePlus.sprite;
            spriteRenderer.color = spritePlus.color;
        }
        else
        {
            Destroy(backIcon);
        }
    }

    public void OpenReady()
    {
        IsDragWhileClick = false;
        InputManager.Instance.OnDragDelta.AddListener(MoveWhileDrag);
        InputManager.Instance.OnLeftUp.AddListener(OpenStarter);
        InputManager.Instance.OnRightUp.AddListener(FlagStarter);
    }

    private void MoveWhileDrag(Vector2 _)
    {
        IsDragWhileClick = true;
    }
    private void OpenStarter()
    {

        if (IsDragWhileClick) return;

        InputManager.Instance.OnDragDelta.RemoveListener(MoveWhileDrag);
        InputManager.Instance.OnLeftUp.RemoveListener(OpenStarter);
        InputManager.Instance.OnRightUp.RemoveListener(FlagStarter);

        MSCellInputManager.Instance.OnOpened.Invoke(this);

        Open();
    }

    private void FlagStarter()
    {

        if (IsDragWhileClick) return;

        InputManager.Instance.OnDragDelta.RemoveListener(MoveWhileDrag);
        InputManager.Instance.OnLeftUp.RemoveListener(OpenStarter);
        InputManager.Instance.OnRightUp.RemoveListener(FlagStarter);

        if (IsOpen) return;

        MSCellInputManager.Instance.OnFlaged.Invoke(this);
        MSCellInputManager.Instance.OnClick.Invoke(this);

        Flag();
    }


    private void Open()
    {

        if (IsOpen)
        {
            TryChordSurroundings();
            return;
        }

        if (IsFlagged || IsExplored) return;

        if (state == MineSweeperTiles.Mine)
        {
            MSManager.Instance.OnFail.Invoke();
            return;
        }

        MSCellInputManager.Instance.OnClick.Invoke(this);
        SpreadOpen();
    }

    private void Flag()
    {
        bool targetFlag = !IsFlagged;
        IsFlagged = targetFlag;
        flag.SetActive(targetFlag);

        IsExplored = false;
    }

    /// <summary>
    /// 열린 숫자 타일을 클릭했을 때, 주변 깃발 수가 숫자와 일치하면 나머지 타일을 엽니다.
    /// </summary>
    private void TryChordSurroundings()
    {
        if (state == MineSweeperTiles.Mine || state == MineSweeperTiles.Empty) return;

        byte flagCount = 0;

        // 주변 깃발 개수 확인
        foreach (Vector2SByte dir in Type.Consts.Vector2.directions8Way)
        {
            MSCell cell = Utils.RayCast.TryGetCell(transform.position, dir, RAY_DISTANCE);
            if (cell != null && cell.IsFlagged)
            {
                flagCount++;
            }
        }

        // 깃발 개수와 본인의 숫자가 일치할 때만 주변 오픈
        if (flagCount == (byte)state)
        {
            bool opened = false;
            foreach (Vector2SByte dir in Type.Consts.Vector2.directions8Way)
            {
                MSCell cell = Utils.RayCast.TryGetCell(transform.position, dir, RAY_DISTANCE);
                if (cell == null) continue;
                if (cell.IsFlagged || cell.IsOpen) continue;

                //폭탄일 경우 종료
                if (cell.state == MineSweeperTiles.Mine)
                {
                    MSManager.Instance.OnFail.Invoke();
                    return;
                }

                opened = true;
                cell.SpreadOpen();
            }

            if (opened) MSCellInputManager.Instance.OnClick.Invoke(this);
        }

    }

    /// <summary>
    /// 타일을 열고 빈 타일일 경우 큐(Queue)를 이용해 주변으로 확산(BFS)합니다.
    /// </summary>
    public void SpreadOpen()
    {
        // 이미 탐색되었거나, 깃발이 꽂혀있거나, 열려있는 타일이면 무시
        if (IsExplored || IsFlagged || IsOpen) return;

        Queue<MSCell> queue = new Queue<MSCell>();

        // 현재 셀을 시작점으로 설정
        this.IsExplored = true;
        queue.Enqueue(this);

        while (queue.Count > 0)
        {
            MSCell current = queue.Dequeue();

            // 시각적, 논리적 오픈 처리
            current.Transparent();

            // 2. 빈 타일(Empty)일 경우: 주변 4(8)방향으로 BFS 확산
            if (current.state == MineSweeperTiles.Empty)
            {
                foreach (Vector2SByte dir in Type.Consts.Vector2.directions8Way)
                {
                    MSCell nextCell = Utils.RayCast.TryGetCell(current.transform.position, dir, RAY_DISTANCE);

                    // 큐에 넣기 전 유효성 검사 (아직 탐색되지 않았고, 깃발이 없고, 열려있지 않은 셀)
                    if (nextCell != null && !nextCell.IsExplored && !nextCell.IsFlagged && !nextCell.IsOpen)
                    {
                        nextCell.IsExplored = true; // 큐 중복 삽입 방지를 위한 사전 처리
                        queue.Enqueue(nextCell);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 주변이 숫자이고 본인도 숫자일 경우 연쇄적으로 여는 특별 로직
    /// </summary>
    public void OpenTogether()
    {
        byte count = 0;

        foreach (Vector2SByte dir in Type.Consts.Vector2.directions)
        {
            MSCell cell = Utils.RayCast.TryGetCell(transform.position, dir, RAY_DISTANCE);
            if (cell == null) continue;

            MineSweeperTiles type = cell.TileState;
            if (type != MineSweeperTiles.Mine && type != MineSweeperTiles.Empty)
            {
                count++;
            }
        }

        // 두 방향 이상 숫자 타일이 있다면 투명화(오픈) 처리
        if (count >= 2)
        {
            Transparent();
        }
    }

    /// <summary>
    /// 실제 타일의 시각적, 논리적 오픈을 처리합니다.
    /// </summary>
    public void Transparent()
    {
        if (IsFlagged) return;

        IsOpen = true;
        MSCellInputManager.Instance.OnOpenedEach.Invoke(this);

        AnimationManager.Instance.delayedQueue.Enqueue(SlowTransparent);
    }

    private void SlowTransparent()
    {
        StartCoroutine(TransparentCoroutine());
    }

    private IEnumerator TransparentCoroutine()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color startColor = spriteRenderer.color;
        Color endColor = spriteRenderer.color - new Color(0, 0, 0, spriteRenderer.color.a);

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * 5;
            spriteRenderer.color = Color.Lerp(startColor, endColor, time);
            yield return null;
        }

        spriteRenderer.color = endColor;
    }
}