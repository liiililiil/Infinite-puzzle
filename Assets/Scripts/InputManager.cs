using SimpleActions;
using Type;
using Type.Utils;
using UnityEngine;

public class InputManager : Managers<InputManager>
{
    private Vector2 _prevInputPosition;
    private bool _isLeftDragging;
    private bool _isRightDragging;

    public bool _isInputable { get; private set; }

    public SimpleEvent OnRightDown { get; private set; } = new SimpleEvent();
    public SimpleEvent OnRightUp { get; private set; } = new SimpleEvent();
    public SimpleEvent OnLeftDown { get; private set; } = new SimpleEvent();
    public SimpleEvent OnLeftUp { get; private set; } = new SimpleEvent();

    // 변화량을 제한 없이 그대로 전달
    public SimpleEvent<float> OnZoomDelta { get; private set; } = new SimpleEvent<float>();
    public SimpleEvent<Vector2> OnDragDelta { get; private set; } = new SimpleEvent<Vector2>();

    private void Awake() => Singleton(false);

    private void Update()
    {
        Vector2 rawInput = Input.mousePosition;

        // 좌측 버튼 드래그
        if (Input.GetMouseButtonDown(0)) { _isLeftDragging = true; _prevInputPosition = rawInput; OnLeftDown.Invoke(); }
        if (Input.GetMouseButtonUp(0)) { _isLeftDragging = false; OnLeftUp.Invoke(); }

        // 우측 버튼 드래그
        if (Input.GetMouseButtonDown(1)) { _isRightDragging = true; _prevInputPosition = rawInput; OnRightDown.Invoke(); }
        if (Input.GetMouseButtonUp(1)) { _isRightDragging = false; OnRightUp.Invoke(); }

        // 드래그 중 처리 (데드존 적용)
        if (_isLeftDragging || _isRightDragging)
        {
            Vector2 delta = rawInput - _prevInputPosition;

            // 데드존 체크: sqrMagnitude 사용으로 최적화 (1f * 1f = 1f)
            if (delta.sqrMagnitude > 20f)
            {
                OnDragDelta.Invoke(delta);
                _prevInputPosition = rawInput;
            }
        }

        // 줌 처리
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            OnZoomDelta.Invoke(scroll);
        }
    }

    public Vector2 GetInputPositionToWorld()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}