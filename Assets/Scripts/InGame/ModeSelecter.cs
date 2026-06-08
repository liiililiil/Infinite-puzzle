using UnityEngine;

public class ModeSelecter : MonoBehaviour
{
    [SerializeField]
    private GameObject select;
    [SerializeField]
    private GameObject repeat;
    private void Awake()
    {
        if (PlayConfigBuffer.Instance.gameStyle == Type.Enums.GamePlay.GameStyle.Single)
        {
            Instantiate(select);
        }
        else
        {
            Instantiate(repeat);
        }

        Destroy(gameObject);
    }
}
