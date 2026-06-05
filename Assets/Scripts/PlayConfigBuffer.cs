using Type.Enums.GamePlay;
using Types.Utils;
using UnityEngine;

public class PlayConfigBuffer : Managers<PlayConfigBuffer>
{
    public Capsule<GameType> gameType;
    public Capsule<GameStyle> gameStyle;

    private void Awake()
    {
        Singleton(true);
    }
}
