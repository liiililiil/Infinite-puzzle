using Extensions;
using Type.Enums.GamePlay;
using Type.Utils;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chain : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private ObjectWithComponent<SVGImage, RectTransform> plate;
    [SerializeField]
    private GameStyle gameStyle;

    private Coroutine plateCoroutine;
    private Coroutine colorCoroutine;

    private float targetSize = 1;
    ChainManager chainManager;
    byte index;

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.SafeStartCoroutine(ref plateCoroutine, Utils.Generic.AnimationUtils.EasingChange(
            plate.component2.localScale.x, 0.9f, SizeChange, 0.6f, SimpleEasing.EaseType.OutCirc
        ));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.SafeStartCoroutine(ref plateCoroutine, Utils.Generic.AnimationUtils.EasingChange(
            plate.component2.localScale.x, targetSize, SizeChange, 0.6f, SimpleEasing.EaseType.OutCirc
        ));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Select();
    }

    public void Bind(byte index, ChainManager chainManager)
    {
        this.index = index;
        this.chainManager = chainManager;

    }
    public void UnSelect()
    {
        if (plate == null || plate.component2 == null) return;

        targetSize = 1;
        this.SafeStartCoroutine(ref plateCoroutine, Utils.Generic.AnimationUtils.EasingChange(
            plate.component2.localScale.x, targetSize, SizeChange, 0.6f, SimpleEasing.EaseType.OutCirc
        ));
        this.SafeStartCoroutine(ref colorCoroutine, Utils.Generic.AnimationUtils.EasingChange(
            plate.component1.color, Color.white, ColorChange, 0.6f, SimpleEasing.EaseType.OutCirc
        ));

    }

    private void SizeChange(float v)
    {
        plate.component2.localScale = Utils.Vector2Utils.FloatToVector2(v);
    }

    private void ColorChange(Color color)
    {
        plate.component1.color = color;
    }

    public void Select()
    {
        targetSize = 0.8f;
        this.SafeStartCoroutine(ref plateCoroutine, Utils.Generic.AnimationUtils.EasingChange(
            plate.component2.localScale.x, targetSize, SizeChange, 0.6f, SimpleEasing.EaseType.OutCirc
        ));
        this.SafeStartCoroutine(ref colorCoroutine, Utils.Generic.AnimationUtils.EasingChange(
            plate.component1.color, new Color(80f / 255f, 255f / 255f, 120f / 255f, 1), ColorChange, 0.6f, SimpleEasing.EaseType.OutCirc
        ));

        PlayConfigBuffer.Instance.gameStyle = gameStyle;
        chainManager.Select(index);
    }

}
