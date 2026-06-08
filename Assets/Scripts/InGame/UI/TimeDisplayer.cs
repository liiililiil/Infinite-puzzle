using Type;
using Type.Utils;
using UnityEngine;

public class TimeDisplayer : MonoBehaviour
{
    [SerializeField]
    private int clickCount;
    [SerializeField]
    private float zoomActive;
    private bool isFail;

    private Switcher<bool> isHide = new Switcher<bool>();

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        MSCellInputManager.Instance.OnClick.AddListener(ClickActive);
        InputManager.Instance.OnZoomDelta.AddListener(ZoomActive);
        MSManager.Instance.OnFail.AddListener(OnFail);
        MSManager.Instance.OnStart.AddListener(OnStart);

        StartCoroutine(Utils.Generic.AnimationUtils.EasingChange(-500f, 0f, SetPosition, 1, SimpleEasing.EaseType.OutExpo));
    }

    private void ZoomActive(float value)
    {
        zoomActive += Mathf.Abs(value);
        if (zoomActive > 1)
        {
            clickCount = 0;
        }

        ActiveCheck();
    }

    private void ActiveCheck()
    {
        if (isFail) return;

        if (zoomActive < 1 && clickCount > 3)
        {
            if (!isHide.Switch(true)) return;

            // 숨기기
            StartCoroutine(Utils.Generic.AnimationUtils.EasingChange(rectTransform.anchoredPosition.x, -500f, SetPosition, 1, SimpleEasing.EaseType.InExpo));
        }
        else
        {
            if (!isHide.Switch(false)) return;
            // 보이기
            StartCoroutine(Utils.Generic.AnimationUtils.EasingChange(rectTransform.anchoredPosition.x, 0f, SetPosition, 1, SimpleEasing.EaseType.OutExpo));
        }
    }

    private void ClickActive(MSCell _)
    {
        clickCount++;
        zoomActive = 0;

        ActiveCheck();



    }

    private void OnFail()
    {
        isFail = true;
        if (!isHide.Switch(true)) return;

        // 숨기기
        StartCoroutine(Utils.Generic.AnimationUtils.EasingChange(rectTransform.anchoredPosition.x, -500f, SetPosition, 1, SimpleEasing.EaseType.InExpo));
    }

    private void OnStart(Vector2Byte _, Vector2Byte __, int ___)
    {
        isFail = false;
    }

    private void SetPosition(float value)
    {
        rectTransform.anchoredPosition = new Vector2(value, rectTransform.anchoredPosition.y);
    }
}
