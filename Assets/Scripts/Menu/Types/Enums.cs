
namespace Type.Enums.Menu
{
    // 메뉴 상태들
    public enum MenuState : byte
    {
        Init,
        MainMenu,
        Credit,
        Select
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
}

namespace Type.Enums
{
    public enum SceneList : byte
    {
        init,
        Menu,
        InPlaying,
    }
}