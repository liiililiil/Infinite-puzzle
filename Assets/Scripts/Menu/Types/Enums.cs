
namespace Type.Enums.Menu
{
    // 메뉴 상태들
    public enum MenuState : byte
    {
        Init,
        MainMenu,
        Credit,
        Select,
        Load,
    }
}

namespace Type.Enums.GamePlay
{
    public enum GameType : byte
    {

        // 지뢰 찾기
        MineSweeper
    }


    public enum GameStyle : byte
    {
        Single,
        Marathon,
    }

    public enum MineSweeperTiles : sbyte
    {
        First = -2,
        Mine = -1,
        Empty = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
    }
}

namespace Type.Enums
{
    public enum SceneList : byte
    {
        init,
        Menu,
        MineSweeper,
    }
}