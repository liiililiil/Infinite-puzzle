using Type;
using Type.Enums.GamePlay;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera _camera;

    private Vector2 targetPos;
    private float targetZoom;
    private byte size;

    private bool isPlaying = false;

    private float mapHalfX;
    private float mapHalfY;

    private float zoomSensitivity = 2.0f;
    private float minZoom = 1f;


    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        InputManager.Instance.OnZoomDelta.AddListener(OnChangeZoom);
        InputManager.Instance.OnDragDelta.AddListener(OnChangePos);

        MSManager.Instance.OnFail.AddListener(GameEnd);
        MSManager.Instance.OnClear.AddListener(GameEnd);
        MSManager.Instance.OnStart.AddListener(OnStart);
    }

    private void FixedUpdate()
    {
        if (!isPlaying) return;

        transform.position =
            (Vector3)Vector2.Lerp(transform.position, targetPos, 0.4f)
            + new Vector3(0, 0, -1);

        _camera.orthographicSize =
            Mathf.Lerp(_camera.orthographicSize, targetZoom, 0.4f);
    }

    private void OnChangePos(Vector2 delta)
    {
        if (!isPlaying) return;

        targetPos -= delta * 0.02f * (targetZoom * 0.2f);
        ClampTargetPosition();
    }

    private void OnChangeZoom(float delta)
    {
        if (!isPlaying) return;

        float t = Mathf.InverseLerp(minZoom, size, targetZoom);
        float minMultiplier = 1f;
        float maxMultiplier = 10f;
        float speedFactor = (delta > 0) ? Mathf.Lerp(maxMultiplier, minMultiplier, t) : Mathf.Lerp(minMultiplier, maxMultiplier, t);

        targetZoom += -delta * zoomSensitivity * speedFactor;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, size);

        ClampTargetPosition();
    }

    private void ClampTargetPosition()
    {
        if (!isPlaying) return;

        float camHalfY = targetZoom;
        float camHalfX = targetZoom * _camera.aspect;

        float visibleCellSize = 3.0f;

        float maxX = Mathf.Max(0, mapHalfX + camHalfX - visibleCellSize);
        float maxY = Mathf.Max(0, mapHalfY + camHalfY - visibleCellSize);

        targetPos.x = Mathf.Clamp(targetPos.x, -maxX, maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, -maxY, maxY);
    }

    private void OnStart(Vector2Byte mapSize, Vector2Byte starting, int _)
    {
        isPlaying = true;

        size = (byte)Mathf.Max(mapSize.x / 2 + 1, mapSize.y / 2 + 1);
        mapHalfX = mapSize.x * 0.5f;
        mapHalfY = mapSize.y * 0.5f;

        targetZoom = 5f;
        SetZoom(targetZoom);

        float startX = -mapHalfX;
        float startY = -mapHalfY;

        targetPos = new Vector2(startX + starting.x, startY + starting.y);
        ClampTargetPosition();
    }

    private void GameEnd()
    {
        isPlaying = false;

        StartCoroutine(Utils.Generic.AnimationUtils.EasingChange(
            _camera.orthographicSize, size, SetZoom, 2f, SimpleEasing.EaseType.InOutQuad, setMoveable));

        StartCoroutine(Utils.Generic.AnimationUtils.EasingChange(
            (Vector2)transform.position, Vector2.zero, SetPosition, 2f, SimpleEasing.EaseType.InOutQuad, setMoveable));
    }

    private void SetZoom(float value)
    {

        _camera.orthographicSize = value;
        targetZoom = value;
    }

    private void SetPosition(Vector2 position)
    {

        transform.position = new Vector3(position.x, position.y, -1);
        targetPos = position;
    }

    private void setMoveable()
    {
        isPlaying = true;
        ClampTargetPosition();
    }
}