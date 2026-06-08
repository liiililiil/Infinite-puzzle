using Type;
using Type.Enums.GamePlay;
using UnityEngine;

public class ClearDetecter : MonoBehaviour
{
    [SerializeField]
    private ushort mine = 0;
    [SerializeField]
    private ushort leftTile = 0;

    private bool isFail = false;

    private void ValueReset()
    {
        mine = 0;
        leftTile = 0;
    }
    private void OnStart(Vector2Byte size, Vector2Byte __, int ___)
    {
        leftTile = (ushort)(size.x * size.y);
        isFail = false;
    }

    private void Start()
    {
        MSManager.Instance.OnInstantiate.AddListener(TileAdd);
        MSCellInputManager.Instance.OnOpenedEach.AddListener(OpenCheck);
        MSManager.Instance.OnFail.AddListener(Fail);
        MSManager.Instance.OnClear.AddListener(Clear);
        MSManager.Instance.OnStart.AddListener(OnStart);
    }

    private void OpenCheck(MSCell cell)
    {
        leftTile--;

        if (mine == leftTile && !isFail && cell.TileState != MineSweeperTiles.Mine) MSManager.Instance.OnClear.Invoke();

    }

    private void Fail()
    {
        isFail = true;
        ValueReset();
    }
    private void Clear()
    {
        isFail = true;
        ValueReset();
    }

    private void TileAdd(GameObject cell)
    {
        if (cell.GetComponent<MSCell>().TileState == MineSweeperTiles.Mine)
        {
            mine++;
        }
    }



}
