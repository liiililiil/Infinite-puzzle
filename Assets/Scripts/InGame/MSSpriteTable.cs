using Type.Enums.GamePlay;
using Type.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

public class MSSpriteTable : Managers<MSSpriteTable>
{
    [SerializeField]
    private SpritePlus first;
    [SerializeField]
    private SpritePlus mineSprite;

    [SerializeField]
    private SpritePlus[] tileSprite;

    private void Awake()
    {
        Singleton(false);
    }

    public bool GetSpritePlus(MineSweeperTiles key, out SpritePlus sprite)
    {
        sprite = new SpritePlus();
        switch (key)
        {
            case MineSweeperTiles.First:
                sprite = first;
                return true;
            case MineSweeperTiles.Mine:
                sprite = mineSprite;
                return true;
            case MineSweeperTiles.Empty:
                return false;
            default:
                int index = (int)key - 1;
                if (index >= 0 && index < tileSprite.Length)
                {
                    sprite = tileSprite[index];
                    return true;
                }
                return false;
        }
    }
}
