using Type;
using UnityEngine;

public class BackPlate : MonoBehaviour
{
    private void Start()
    {
        MSManager.Instance.OnStart.AddListener(OnStart);
    }
    private void OnStart(Vector2Byte size, Vector2Byte _, int __)
    {

        transform.localScale = (Vector2)size;
    }
}
