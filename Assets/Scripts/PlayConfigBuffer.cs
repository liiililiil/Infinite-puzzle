using Type.Enums.GamePlay;
using Type.Utils;
using UnityEngine;

public class PlayConfigBuffer : Managers<PlayConfigBuffer>
{
    public GameStyle gameStyle { get; set; }
    private void Awake()
    {
        Singleton(true);
    }
}
