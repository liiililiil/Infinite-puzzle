using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleActions
{
    //한번만 작동하고 자동 해제
    public class OneTimeEvent
    {
        List<Action> actions = new List<Action>();

        public void AddListener(Action action)
        {
            actions.Add(action);
        }

        public void RemoveListener(Action action)
        {
            actions.Remove(action);
        }

        public void Invoke()
        {
            //임시 액션에 리스트를 복사하고 원본 리스트는 초기화
            List<Action> tempActions = new List<Action>(actions);
            actions.Clear();

            for (int i = tempActions.Count - 1; i >= 0; i--)
            {
                try
                {
                    tempActions[i].Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        ~OneTimeEvent()
        {
            actions.Clear();
        }
    }
    //경량화된 이벤트
    public class SimpleEvent
    {
        private Action onEventInvoked;

        public void AddListener(Action action)
        {
            onEventInvoked += action;
        }

        public void RemoveListener(Action action)
        {
            onEventInvoked -= action;
        }

        public void Invoke()
        {
            onEventInvoked?.Invoke();
        }

        public void Clear()
        {
            onEventInvoked = null;
        }
    }
    public class SimpleEvent<T1>
    {
        private Action<T1> onEventInvoked;

        public void AddListener(Action<T1> action)
        {
            onEventInvoked += action;
        }

        public void RemoveListener(Action<T1> action)
        {
            onEventInvoked -= action;
        }

        public void Invoke(T1 param1)
        {
            onEventInvoked?.Invoke(param1);
        }

        public void Clear()
        {
            onEventInvoked = null;
        }
    }

    public class SimpleEvent<T1, T2>
    {
        private Action<T1, T2> onEventInvoked;

        public void AddListener(Action<T1, T2> action)
        {
            onEventInvoked += action;
        }

        public void RemoveListener(Action<T1, T2> action)
        {
            onEventInvoked -= action;
        }

        public void Invoke(T1 param1, T2 param2)
        {
            onEventInvoked?.Invoke(param1, param2);
        }

        public void Clear()
        {
            onEventInvoked = null;
        }
    }

    public class SimpleEvent<T1, T2, T3>
    {
        private Action<T1, T2, T3> onEventInvoked;

        public void AddListener(Action<T1, T2, T3> action)
        {
            onEventInvoked += action;
        }

        public void RemoveListener(Action<T1, T2, T3> action)
        {
            onEventInvoked -= action;
        }

        public void Invoke(T1 param1, T2 param2, T3 param3)
        {
            onEventInvoked?.Invoke(param1, param2, param3);
        }

        public void Clear()
        {
            onEventInvoked = null;
        }
    }
}
