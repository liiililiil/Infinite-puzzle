using SimpleActions;
using Type;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

public class InputManager : Managers<InputManager>
{
    private DeviceType currentDevice;

    [SerializeField]
    private Vector2 inputPosition;

    [SerializeField]
    private float currentDistance;
    public SimpleEvent OnInputDown { get; private set; } = new SimpleEvent();
    public SimpleEvent OnInputUp { get; private set; } = new SimpleEvent();
    public SimpleEvent<float> OnZoomChange { get; private set; } = new SimpleEvent<float>();


    private void Awake()
    {
        Singleton(true);
    }

    private void Update()
    {
        PcUpdate();
    }


    private void PcUpdate()
    {
        // Pos
        inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Down Up
        if (Input.GetMouseButtonDown(0))
            OnInputDown.Invoke();

        if (Input.GetMouseButtonUp(0))
            OnInputUp.Invoke();

        // Zoom
        float distance = Input.GetAxis("Mouse ScrollWheel");

        distanceApply(distance);
    }

    private void distanceApply(float distance)
    {
        distance += currentDistance;

        if (currentDistance != distance)
        {
            OnZoomChange.Invoke(distance);
        }

        currentDistance = distance;
    }



    public Vector2 GetInputPositionToWorld()
    {
        return inputPosition;
    }


}
