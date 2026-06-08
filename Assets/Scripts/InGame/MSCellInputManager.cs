using SimpleActions;
using Type;
using UnityEditor;
using UnityEngine;

public class MSCellInputManager : Managers<MSCellInputManager>
{
    bool isPlaying = true;
    private void Awake()
    {
        Singleton(false);
    }
    public SimpleEvent<MSCell> OnOpened { get; private set; } = new SimpleEvent<MSCell>();
    public SimpleEvent<MSCell> OnOpenedEach { get; private set; } = new SimpleEvent<MSCell>();
    public SimpleEvent<MSCell> OnFlaged { get; private set; } = new SimpleEvent<MSCell>();
    public SimpleEvent<MSCell> OnClick { get; private set; } = new SimpleEvent<MSCell>();

    void Start()
    {
        InputManager.Instance.OnRightDown.AddListener(OpenReady);
        InputManager.Instance.OnLeftDown.AddListener(OpenReady);

        MSManager.Instance.OnStart.AddListener(OnStart);
        MSManager.Instance.OnFail.AddListener(OnEnd);
        MSManager.Instance.OnClear.AddListener(OnEnd);
    }

    public void OpenReady()
    {
        if (!isPlaying) return;

        GetCellInWorld(InputManager.Instance.GetInputPositionToWorld())?.OpenReady();
    }


    private MSCell GetCellInWorld(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapBox(position, Vector2.one / 10, 0, LayerMask.GetMask("Cell"));
        return hit?.gameObject.GetComponent<MSCell>();
    }

    private void OnStart(Vector2Byte _, Vector2Byte __, int ___)
    {
        isPlaying = true;

    }

    private void OnEnd()
    {
        isPlaying = false;
    }
}
