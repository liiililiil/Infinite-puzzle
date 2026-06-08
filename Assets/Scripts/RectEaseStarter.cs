using Extensions;
using SimpleEasing;
using UnityEngine;

public class RectEaseStarter : MonoBehaviour
{
    [SerializeField]
    EaseType easeType;
    [SerializeField]
    float duration;
    [SerializeField]
    Vector2 targetPos;

    RectTransform rectTransform;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(Utils.Generic.AnimationUtils.EasingChange(
            rectTransform.anchoredPosition, targetPos, rectTransform.SetAnchoredPosition, duration, easeType
        ));
    }
}
