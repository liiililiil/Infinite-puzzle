using Extensions;
using Type;
using UnityEngine;
using UnityEngine.UI;

public class EndDisplayer : MonoBehaviour
{
    [SerializeField]
    private Text hideText;
    [SerializeField]
    private Text titleText;
    RectTransform rectTransform;

    [SerializeField]
    bool OnLoadFail = true;

    [SerializeField]
    bool OnLoadClear = true;

    [SerializeField]
    private RectTransform hide;

    Coroutine coroutine1;
    Coroutine coroutine2;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {

        MSManager.Instance.OnStart.AddListener(OnStart);

        if (OnLoadClear)
            MSManager.Instance.OnClear.AddListener(OnClear);

        if (OnLoadFail)
            MSManager.Instance.OnFail.AddListener(OnFail);
    }
    public void OnHide()
    {
        if (gameObject.activeSelf)
        {
            hideText.text = "Show";
        }
        else
        {
            hideText.text = "Hide";
        }

        gameObject.SetActive(!gameObject.activeSelf);

    }

    public void OnExit()
    {
        SceneListManager.Instance.LoadScene(Type.Enums.SceneList.Menu);
    }

    private void OnStart(Vector2Byte _, Vector2Byte __, int ___)
    {
        if (!gameObject.activeSelf)
        {
            OnHide();
        }

        this.SafeStopCoroutine(ref coroutine1);
        this.SafeStopCoroutine(ref coroutine2);
        DisplayPosChange(1150f);
        HidePosChange(80f);
    }
    private void OnFail()
    {
        titleText.text = "Fail!";
        Appear();
    }
    private void OnClear()
    {
        titleText.text = "Clear!";
        Appear();
    }

    private void Appear()
    {
        this.SafeStartCoroutine(ref coroutine1, Utils.Generic.AnimationUtils.EasingChange(
        80f, -80f, HidePosChange, 2, SimpleEasing.EaseType.OutCubic
        ));
        this.SafeStartCoroutine(ref coroutine2, Utils.Generic.AnimationUtils.EasingChange(
            1150f, 0f, DisplayPosChange, 2, SimpleEasing.EaseType.OutCubic
        ));
    }
    public void Hide()
    {
        this.SafeStartCoroutine(ref coroutine1, Utils.Generic.AnimationUtils.EasingChange(
        -80f, 80f, HidePosChange, 1, SimpleEasing.EaseType.OutCubic
        ));
        this.SafeStartCoroutine(ref coroutine2, Utils.Generic.AnimationUtils.EasingChange(
            0f, 1150f, DisplayPosChange, 1, SimpleEasing.EaseType.OutCubic
        ));
    }
    private void DisplayPosChange(float value)
    {
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, value);
    }
    private void HidePosChange(float value)
    {
        hide.anchoredPosition = new Vector2(value, hide.anchoredPosition.y);
    }
}
