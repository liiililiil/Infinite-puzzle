using Type;
using Type.Enums.GamePlay;
using UnityEngine;
using UnityEngine.UI;

public class LeftMineDisplayer : MonoBehaviour
{
    [SerializeField]
    private ushort mine = 0;
    [SerializeField]
    private ushort flag = 0;

    [SerializeField]
    private Text text;
    private RectTransform rectTransform;
    private void ValueReset()
    {
        mine = 0;
        flag = 0;
    }
    private void OnStart(Vector2Byte _, Vector2Byte __, int ___)
    {
        text.text = (mine - flag).ToString();
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        MSManager.Instance.OnInstantiate.AddListener(TileAdd);
        MSCellInputManager.Instance.OnFlaged.AddListener(FlagCheck);
        MSManager.Instance.OnFail.AddListener(Fail);
        MSManager.Instance.OnClear.AddListener(Clear);
        MSManager.Instance.OnStart.AddListener(OnStart);

        StartCoroutine(Utils.Generic.AnimationUtils.EasingChange(
            500f, 0, SetPosition, 1.3f, SimpleEasing.EaseType.OutCubic
        ));
    }

    private void FlagCheck(MSCell cell)
    {
        if (cell.IsFlagged)
        {
            flag--;
        }
        else
        {
            flag++;
        }

        text.text = (mine - flag).ToString();
    }

    private void Fail()
    {
        ValueReset();
    }
    private void Clear()
    {
        ValueReset();
    }
    private void TileAdd(GameObject cell)
    {
        if (cell.GetComponent<MSCell>().TileState == MineSweeperTiles.Mine)
        {
            mine++;
        }
    }

    private void SetPosition(float v)
    {
        rectTransform.anchoredPosition = new Vector2(v, rectTransform.anchoredPosition.y);
    }
}
