using System;
using System.Collections.Generic;
using SimpleActions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 개발자 편의를 위한 타입
/// </summary>
namespace Type.Utils
{
    #region ObjectWithComponent

    // 처음 컴포넌를 겟하면 그 컴포넌트를 불러 올수 있는 클래스
    public class InitableComponent<_T1> where _T1 : Component
    {
        private _T1 _component;

        private Func<_T1> getter;
        public _T1 component
        {
            get
            {
                return getter();
            }
            private set
            {
                _component = value;
            }
        }


        public InitableComponent(GameObject gameObject)
        {
            getter = () => ComponentInit(gameObject);
        }

        private _T1 ComponentInit(GameObject gameObject)
        {
            gameObject.TryGetComponent(out _component);
            getter = GetComponent;

            return _component;
        }

        private _T1 GetComponent()
        {
            return _component;
        }
    }


    // 게임 오브젝트와 함께 추가로 필요한 컴포넌트가 한번에 포함된 타입

    [Serializable]
    public class ObjectWithComponent<_T1> where _T1 : Component
    {
        [SerializeField]
        public GameObject gameObject;

        private InitableComponent<_T1> _component;
        public _T1 component
        {
            get
            {
                if (_component == null) _component = new InitableComponent<_T1>(gameObject);
                return _component.component;
            }
        }
    }
    [Serializable]
    public class ObjectWithComponent<_T1, _T2> where _T1 : Component where _T2 : Component
    {
        [SerializeField]
        public GameObject gameObject;

        private InitableComponent<_T1> firstComponent;
        private InitableComponent<_T2> secondComponent;

        public _T1 component1
        {
            get
            {
                if (firstComponent == null) firstComponent = new InitableComponent<_T1>(gameObject);
                return firstComponent.component;
            }
        }
        public _T2 component2
        {
            get
            {
                if (secondComponent == null) secondComponent = new InitableComponent<_T2>(gameObject);
                return secondComponent.component;
            }
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ObjectWithComponent<>), true)]
    public class FirstObjectWithComponentDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 내부의 'gameObject' 필드를 찾습니다.
            SerializedProperty gameObjectProperty = property.FindPropertyRelative("gameObject");
            EditorGUI.PropertyField(position, gameObjectProperty, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 한 줄 높이만 사용하도록 설정 (접힘/펼침 공간 제거)
            return EditorGUIUtility.singleLineHeight;
        }
    }

    [CustomPropertyDrawer(typeof(ObjectWithComponent<,>), true)]
    public class SecondObjectWithComponentDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 내부의 'gameObject' 필드를 찾습니다.
            SerializedProperty gameObjectProperty = property.FindPropertyRelative("gameObject");
            EditorGUI.PropertyField(position, gameObjectProperty, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 한 줄 높이만 사용하도록 설정 (접힘/펼침 공간 제거)
            return EditorGUIUtility.singleLineHeight;
        }
    }
#endif

    #endregion
    public struct Switcher<_T1> where _T1 : IEquatable<_T1>
    {
        private _T1 value;
        public _T1 Value => value;

        public bool Switch(_T1 newValue)
        {
            // 이전 값과 다르면 덮어쓰고 true반환
            if (!value.Equals(newValue))
            {
                value = newValue;
                return true;
            }

            return false;
        }

        public Switcher(_T1 value)
        {
            this.value = value;
        }

        public static implicit operator _T1(Switcher<_T1> target)
        {
            return target.value;
        }
    }

    [Serializable]
    public class FloatRange
    {
        public float start;
        public float end;

        public FloatRange(float start = 0, float end = 0)
        {
            this.start = start;
            this.end = end;
        }

    }

    public class Capsule<T>
    {
        private T _value;

        private SimpleEvent<Capsule<T>> StartGet = new SimpleEvent<Capsule<T>>();
        private SimpleEvent<Capsule<T>> StartSet = new SimpleEvent<Capsule<T>>();

        public T Get(bool IsSilence = false)
        {
            if (!IsSilence)
                StartGet.Invoke(this);
            return _value;
        }

        public void Set(T value, bool IsSilence = false)
        {
            if (!IsSilence)
                StartSet.Invoke(this);
            _value = value;
        }
    }

    public class WaitForTask : CustomYieldInstruction
    {
        private System.Threading.Tasks.Task _task;
        public override bool keepWaiting => !_task.IsCompleted;

        public WaitForTask(System.Threading.Tasks.Task task) => _task = task;
    }

    public class Log
    {
        public static void Log2DArray<T>(T[,] array, bool convertMine = true)
        {
            if (array == null)
            {
                Debug.LogError("Log2DArray: 배열이 Null입니다!");
                return;
            }

            int ySize = array.GetLength(0); // 행 (Row)
            int xSize = array.GetLength(1); // 열 (Column)

            // 스트링빌더 용량 최적화 할당
            System.Text.StringBuilder sb = new System.Text.StringBuilder((xSize * 4 + 2) * ySize);
            sb.AppendLine($"[2D Array Log] Size: {ySize} x {xSize}");

            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    T value = array[y, x];
                    string elementStr = value.ToString();

                    // 지뢰찾기 디버깅 편의 기능: -1은 지뢰 기호로 치환
                    if (convertMine && elementStr == "-1")
                    {
                        elementStr = "*";
                    }

                    // 각 칸을 3글자 크기로 우측 정렬해서 공백을 채움 (줄 뒤틀림 방지)
                    sb.Append(elementStr.PadLeft(3));
                    sb.Append(" ");
                }
                sb.AppendLine(); // 한 행이 끝나면 줄바꿈
            }

            // 단 한 번의 로그 호출로 맵 전체 가독성 확보
            Debug.Log(sb.ToString());
        }
    }

    [Serializable]
    public struct SpritePlus
    {
        public Sprite sprite;
        public Color color;
    }

    [Serializable]
    public struct IndexPaird<_T1, _T2>
    {
        [SerializeField]
        private _T1 _key;
        public _T1 key { get => _key; }

        [SerializeField]
        private _T2 _value;
        public _T2 value { get => value; }
    }

    [Serializable]
    public struct ReadOnlyValue<_T1>
    {
        [SerializeField]
        private _T1 _value;

        public _T1 value { get => _value; }
    }

    [Serializable]
    public class TextList
    {
        [SerializeField]
        private List<Text> texts;
        public string text { set => SetText(value); }

        public void SetText(string str)
        {
            foreach (Text text in texts)
            {
                text.text = str;
            }
        }

        public int Count() => texts.Count;
    }
}