using UnityEngine;

public class Managers<T> : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected void Singleton(bool isDontDestroyOnLoad = true)
    {
        if (Instance == null)
        {
            Instance = (T)(object)this;

            if (isDontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(this);
        }
    }

    protected void OnDisable()
    {
        Instance = default;
    }
}
