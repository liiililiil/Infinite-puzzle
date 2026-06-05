using Type.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneListManager : Managers<SceneListManager>
{
    [SerializeField]
    private SceneList startScene;

    private void Awake()
    {
        Singleton(true);
    }

    private void Start()
    {
        LoadScene(startScene);
    }


    public void LoadScene(SceneList scene)
    {
        SceneManager.LoadScene(Type.Consts.Scene.GetScene(scene));
    }
}
