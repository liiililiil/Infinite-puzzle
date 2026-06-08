using UnityEngine;

public class FocusCurtain : MonoBehaviour
{
    [SerializeField]
    GameObject target;
    void OnApplicationFocus(bool focus)
    {
        target.SetActive(!focus);
    }

    private void Start()
    {
        target.SetActive(false);
    }
}
