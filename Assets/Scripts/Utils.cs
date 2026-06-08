using System;
using System.Collections;
using SimpleEasing;
using Type;
using UnityEngine;

namespace Utils
{

    public class RayCast
    {
        public static MSCell TryGetCell(Vector2 position, Vector2 direction, float distance)
        {
            return TryGetGameObject(position, direction, distance, LayerMask.GetMask("Cell"))?.GetComponent<MSCell>();
        }


        // 게임 오브젝트 탐색
        public static GameObject TryGetGameObject(Vector2 position, Vector2 direction, float distance, int layer)
        {
            Vector2 startPos = (Vector2)position + direction * ((Type.Consts.MineSweeper.TILE_SIZE / 2) + 0.1f);
            float targetDistance = Mathf.Max(distance - ((Type.Consts.MineSweeper.TILE_SIZE / 2) + 0.1f), 0.1f);

#if UNITY_EDITOR
            DrawRayDebug(startPos, direction, targetDistance);
#endif
            RaycastHit2D hit = Physics2D.Raycast(startPos, direction, targetDistance, layer);
            return hit.collider?.gameObject;
        }



#if UNITY_EDITOR
        public static void DrawRayDebug(Vector2 center, Vector2 direction, float distance)
        {
            Debug.DrawRay(center, direction * distance, Color.green, 1);
        }
#endif
    }
    public static class CameraUtils
    {
        public static Vector2 GetMousePosition()
        {
            Vector3 mouse = Input.mousePosition;
            return Camera.main.ScreenToWorldPoint(mouse);
        }
    }

    public static class CoroutineUtils
    {
        public static IEnumerator SlowStart(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }
    }

    public static class Vector2Utils
    {
        public static Vector2 FloatToVector2(float value)
        {
            return new Vector2(value, value);
        }

        public static Vector3 FloatToVector3(float value)
        {
            return new Vector3(value, value, value);
        }

        /// <summary>
        /// a에서 b를 바라보기 위한 오일러 각도를 구합니다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float LookAt2d(Vector2 a, Vector2 b)
        {
            float dx = b.x - a.x;
            float dy = b.y - a.y;

            return Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        }

        public static Vector2Byte CompositeToVector2Byte(Vector2 vector2)
        {
            byte x = (byte)(int)vector2.x;
            byte y = (byte)(int)vector2.y;

            return new Vector2Byte(x, y);
        }


    }

    public static class FloatUtils
    {

    }




    public static class MathUtils
    {
        public static bool IsInRange(float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static byte ByteLerp(byte a, byte b, byte t)
        {
            return (byte)(a + (((b - a) * t) >> 8));
        }
    }

}

namespace Utils.Generic
{
    [Serializable]
    public class Row<_T1>
    {
        public _T1[] cells;
    }
    public static class AnimationUtils
    {

        public static IEnumerator EasingChange<_T1>(_T1 start, _T1 end, Action<_T1> applyAction, float duration, EaseType easeType, Action callback = null)
        {
            // Lerp 함수 찾기
            Func<_T1, _T1, float, _T1> lerp = Utils.Generic.Generic.GetLerp<_T1>();

            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                t = Ease.Easing(t, easeType);

                _T1 target = lerp(start, end, t);

                applyAction(target);

                yield return null;
            }

            // 보정 및 콜백
            applyAction(end);
            callback?.Invoke();
        }
    }
    public static class Generic
    {


        public static Func<_T1, _T1, float, _T1> GetLerp<_T1>()
        {
            switch (typeof(_T1))
            {
                case System.Type t when t == typeof(Vector2):
                    return (Func<_T1, _T1, float, _T1>)(object)GetVector2Lerp();

                case System.Type t when t == typeof(Vector3):
                    return (Func<_T1, _T1, float, _T1>)(object)GetVector3Lerp();

                case System.Type t when t == typeof(float):
                    return (Func<_T1, _T1, float, _T1>)(object)GetFloatLerp();
                case System.Type t when t == typeof(Color):
                    return (Func<_T1, _T1, float, _T1>)(object)GetColorLerp();
                case System.Type t when t == typeof(string):
                    return (Func<_T1, _T1, float, _T1>)(object)GetStringLerp();

                default:
                    throw new NotSupportedException($"타입 {typeof(_T1)}에 대응되는 Lerp가 존재하지 않습니다!");
            }
        }
        public static Func<float, float, float, float> GetFloatLerp()
        {
            return Mathf.LerpUnclamped;
        }

        public static Func<Vector2, Vector2, float, Vector2> GetVector2Lerp()
        {
            return Vector2.LerpUnclamped;
        }

        public static Func<Vector3, Vector3, float, Vector3> GetVector3Lerp()
        {
            return Vector3.LerpUnclamped;
        }
        public static Func<Color, Color, float, Color> GetColorLerp()
        {
            return Color.LerpUnclamped;
        }

        public static Func<string, string, float, string> GetStringLerp()
        {
            return LerpString;
        }

        public static string LerpString(string a, string b, float t)
        {
            t = Mathf.Clamp01(t);

            int aLen = a.Length;
            int bLen = b.Length;

            if (t < 0.5f)
            {
                float localT = t * 2f;
                int len = aLen - (int)(localT * aLen);
                return a.Substring(0, len);
            }
            else
            {
                float localT = (t - 0.5f) * 2f;
                int len = (int)(localT * bLen);
                return b.Substring(0, len);
            }
        }

    }


}