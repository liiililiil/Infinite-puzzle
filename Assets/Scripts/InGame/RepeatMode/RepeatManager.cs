using System;
using System.Collections;
using Extensions;
using Type;
using Type.Utils;
using UnityEngine;

public class RepeatManager : MonoBehaviour
{
    Vector2Byte size;
    byte diff;

    ushort clearCount;

    const byte SIZE_START = 5;
    const byte DIFF_START = 5;

    bool endSkip = false;
    Switcher<bool> playing = new Switcher<bool>(false);
    Switcher<bool> firstplaying = new Switcher<bool>(false);


    [SerializeField]
    private RectTransform rectTransform;
    private Coroutine coroutine;

    void Start()
    {
        MSManager.Instance.OnClear.AddListener(Clear);
        // 초기화
        size = new Vector2Byte(SIZE_START, SIZE_START);
        diff = DIFF_START;

        firstplaying.Switch(false);
    }

    void Clear()
    {
        increaseDiff();
        StartCoroutine(Waiter(10, StartGen));

        playing.Switch(false);

    }

    IEnumerator Waiter(float t, Action callback)
    {
        float elapsed = 0f;
        endSkip = false;

        while (elapsed < t)
        {
            if (endSkip) break;
            elapsed += Time.deltaTime;
            yield return null;
        }


        callback?.Invoke();
    }
    void increaseDiff()
    {
        clearCount++;

        // 3판 클리어마다 맵 사이즈 증가
        size = new Vector2Byte((byte)(SIZE_START + (clearCount / 5)), (byte)(SIZE_START + (clearCount / 3)));

        float logValue = Mathf.Log(clearCount + 1, 1.15f);

        // 최종 난이도를 1~255로 제한 (Clamp)
        diff = (byte)Mathf.Clamp(1 + logValue * 10f, 1, 255);
    }


    public void StartGen()
    {
        if (!playing.Switch(true)) return;
        if (firstplaying.Switch(true))
        {
            this.SafeStartCoroutine(ref coroutine, Utils.Generic.AnimationUtils.EasingChange(0f, 1000f, PosChange, 1, SimpleEasing.EaseType.InCubic, () => this.SafeStopCoroutine(ref coroutine)));
        }

        MSManager.Instance.mSConfig.seed = Math.Abs((int)DateTime.Now.Ticks);
        MSManager.Instance.mSConfig.size = size;
        MSManager.Instance.mSConfig.diff = diff;

        MSManager.Instance.MapMake();
    }

    public void skip()
    {
        endSkip = true;
    }

    public void ReTry()
    {
        // 초기화
        size = new Vector2Byte(SIZE_START, SIZE_START);
        diff = DIFF_START;

        playing.Switch(false);

        StartGen();
    }
    private void PosChange(float v)
    {
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, v);
    }

}
