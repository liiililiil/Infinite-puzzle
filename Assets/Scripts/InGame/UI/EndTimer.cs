using Extensions;
using UnityEngine;
using UnityEngine.UI;

public class EndTimer : MonoBehaviour
{
    [SerializeField]
    Text text;

    Coroutine coroutine;

    private void Start()
    {
        MSManager.Instance.OnClear.AddListener(OnEnd);
    }

    private void OnEnd()
    {
        this.SafeStartCoroutine(ref coroutine, Utils.Generic.AnimationUtils.EasingChange(
            9f, 0f, TextChange, 10, SimpleEasing.EaseType.Linear
        ));
    }

    private void TextChange(float v)
    {
        text.text = $"Start In {v.ToString("F2")} Sec";
    }
}
