using System.Collections;
using Extensions;
using SimpleActions;
using Type;
using Type.Utils;
using UnityEngine;

// 플레이 시간 통합 관리용
public class TimeManager : Managers<TimeManager>
{
    private Coroutine coroutine;

    private bool timeFlow = false;
    private bool Isfocus = false;
    public float time { get; private set; }
    public string timeString { get; private set; }

    public SimpleEvent OnTimeChange { get; private set; } = new SimpleEvent();


    private void Awake()
    {
        Singleton(false);
    }

    private void Start()
    {
        MSManager.Instance.OnFail.AddListener(OnFail);
        MSManager.Instance.OnClear.AddListener(OnClear);
        MSManager.Instance.OnStart.AddListener(OnStart);

    }
    private void OnFail()
    {
        timeFlow = false;
    }
    private void OnClear()
    {
        timeFlow = false;
    }
    void OnApplicationFocus(bool focus)
    {
        Isfocus = focus; ;
    }
    private void OnOpen(MSCell _)
    {
        if (time > 0.001f) return;

        MSCellInputManager.Instance.OnOpened.RemoveListener(OnOpen);

        this.SafeStartCoroutine(ref coroutine, TimeFlow());
    }
    private void OnStart(Vector2Byte vector2Byte, Vector2Byte _, int bv)
    {
        time = 0f;
        timeFlow = false;


        MSCellInputManager.Instance.OnOpened.RemoveListener(OnOpen); // 중복 방지용
        MSCellInputManager.Instance.OnClick.AddListener(OnOpen);


    }
    private IEnumerator TimeFlow()
    {
        timeFlow = true;

        while (timeFlow)
        {
            if (Isfocus)
            {
                time += Time.deltaTime;
                int mlisec = (int)((time - (int)time) * 1000);

                int intTime = (int)time;
                int sec = intTime % 60;
                int min = intTime / 60;

                timeString = $"{min:00}:{sec:00}:{mlisec:000}";

                OnTimeChange.Invoke();
            }

            yield return null;
        }
    }


}
