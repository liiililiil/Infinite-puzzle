using System.Collections;
using Extensions;
using Type;
using Type.Utils;
using UnityEngine;
using UnityEngine.UI;

public class MSInfoDisplayer : MonoBehaviour
{
    private int bv = 0;
    private Vector2Byte size;
    private int click = 0;

    [SerializeField]
    private TextList timeText;
    [SerializeField]
    private TextList bvText;
    [SerializeField]
    private TextList clickText;
    [SerializeField]
    private TextList TDCText;
    [SerializeField]
    private TextList efficiency;

    private Coroutine coroutine;


    private void Start()
    {
        MSCellInputManager.Instance.OnClick.AddListener(OnClick);
        MSManager.Instance.OnStart.AddListener(OnStart);
        MSManager.Instance.OnFail.AddListener(OnFail);
        TimeManager.Instance.OnTimeChange.AddListener(TimeChange);
    }

    void TimeChange()
    {
        timeText.text = TimeManager.Instance.timeString;
    }

    private void OnClick(MSCell mSCell)
    {
        click++;
        clickText.text = click.ToString();

        TDC();
        Efficiency();
    }

    private void OnFail()
    {
        efficiency.text = "0%";
    }





    private void OnStart(Vector2Byte vector2Byte, Vector2Byte _, int bv)
    {
        size = vector2Byte;
        this.bv = bv;

        bvText.text = bv.ToString();
        clickText.text = click.ToString();
        timeText.text = "00:00:000"; // 시작 전 초기 UI 셋팅
    }

    private void TDC()
    {
        if (TDCText.Count() <= 0) return;
        float value = 0;

        if (click > 0 && TimeManager.Instance.time > 0)
        {
            value = TimeManager.Instance.time / click;
        }

        int mlisec = (int)((value - (int)value) * 1000);

        int intTime = (int)value;
        int sec = intTime;

        TDCText.text = $"{sec:00}:{mlisec:000}";
    }

    private void Efficiency()
    {
        if (efficiency.Count() <= 0) return;

        float p = 0;
        if (click > 0 && bv > 0)
        {
            p = (float)bv / click * 100f;
        }

        efficiency.text = $"{(int)p}%";
    }
}