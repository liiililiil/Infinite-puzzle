using System;
using Extensions;
using Type;
using Type.Utils;
using UnityEngine;
using UnityEngine.UI;

public class ConfigSelecter : MonoBehaviour
{
    [Header("Seed Settings")]
    [SerializeField] private InputField seed;
    private int previousSeed;

    [Header("Size Settings (Vector2Byte)")]
    // size를 2개의 필드로 분리
    [SerializeField] private InputField sizeX;
    [SerializeField] private InputField sizeY;
    private byte previousSizeX;
    private byte previousSizeY;

    [Header("Difficulty Settings")]
    [SerializeField] private InputField diff;
    private byte previousDiff;

    private Coroutine coroutine;
    private RectTransform rectTransform;
    private Switcher<bool> isPlaying;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        // 1. 이벤트 리스너 연결 (값이 바뀔 때마다 함수가 호출되도록)
        seed.onValueChanged.AddListener(OnChangeSeed);
        sizeX.onValueChanged.AddListener(OnChangeSizeX);
        sizeY.onValueChanged.AddListener(OnChangeSizeY);
        diff.onValueChanged.AddListener(OnChangeDiff);

        // 2. 초기화 로직 수정
        init();

        this.SafeStartCoroutine(ref coroutine, Utils.Generic.AnimationUtils.EasingChange(1000f, 0f, PosChange, 1, SimpleEasing.EaseType.OutCubic));
    }

    private void init()
    {
        int initialSeed = Math.Abs((int)DateTime.Now.Ticks);
        seed.text = initialSeed.ToString();
        previousSeed = initialSeed;

        sizeX.text = 30.ToString();
        sizeY.text = 30.ToString();

        // 300은 byte 제한(255)을 초과하므로 255로 수정
        diff.text = 150.ToString();
    }

    private void OnChangeSeed(string value)
    {
        if (int.TryParse(value, out int intValue))
        {
            MSManager.Instance.mSConfig.seed = intValue;
            previousSeed = intValue;
        }
        else if (!string.IsNullOrEmpty(value))
        {
            // 빈 칸이 아닌데 int 변환에 실패했다면(너무 긴 숫자 등) 이전 값으로 강제 복구
            seed.text = previousSeed.ToString();
        }
    }

    private void OnChangeSizeX(string value)
    {
        byte validX = ValidateInput(sizeX, value, ref previousSizeX);

        // 기존 size의 Y값을 가져와서 X값만 새로운 값으로 갱신
        byte currentY = MSManager.Instance.mSConfig.size.y;
        MSManager.Instance.mSConfig.size = new Vector2Byte(validX, currentY);
    }

    private void OnChangeSizeY(string value)
    {
        byte validY = ValidateInput(sizeY, value, ref previousSizeY);

        // 기존 size의 X값을 가져와서 Y값만 새로운 값으로 갱신
        byte currentX = MSManager.Instance.mSConfig.size.x;
        MSManager.Instance.mSConfig.size = new Vector2Byte(currentX, validY);
    }

    private void OnChangeDiff(string value)
    {
        MSManager.Instance.mSConfig.diff = ValidateInput(diff, value, ref previousDiff);
    }

    // 1~255 제한 로직 (단일 byte 검증용으로 통일)
    private byte ValidateInput(InputField inputField, string text, ref byte previousValue)
    {
        if (string.IsNullOrEmpty(text)) return 0; // 지웠을 때 임시로 0 허용 (원한다면 1로 수정 가능)

        if (int.TryParse(text, out int value))
        {
            // 1보다 작으면 1, 255보다 크면 255로 고정
            int clampedValue = Mathf.Clamp(value, 1, 255);
            previousValue = (byte)clampedValue;

            // 값이 수정되었다면 다시 텍스트에 반영
            if (value != clampedValue)
            {
                inputField.text = clampedValue.ToString();
            }
            return (byte)clampedValue;
        }

        // 숫자 파싱 실패 시(문자 입력 등) 이전 값으로 되돌림
        inputField.text = previousValue.ToString();
        return previousValue;
    }

    public void ClickStart()
    {
        if (!isPlaying.Switch(true)) return;
        MSManager.Instance.MapMake();

        this.SafeStartCoroutine(ref coroutine, Utils.Generic.AnimationUtils.EasingChange(0f, 1000f, PosChange, 1, SimpleEasing.EaseType.InCubic));

    }

    public void ReSelect()
    {
        isPlaying.Switch(false);
        this.SafeStartCoroutine(ref coroutine, Utils.Generic.AnimationUtils.EasingChange(1000f, 0f, PosChange, 1, SimpleEasing.EaseType.OutCubic));


        //시드만 바꾸기
        int initialSeed = Math.Abs((int)DateTime.Now.Ticks);
        seed.text = initialSeed.ToString();
    }
    private void PosChange(float v)
    {
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, v);
    }
}