namespace Type.Consts
{

    public class Scene
    {
        public const string INIT_SCENE = "Init";
        public const string MENU_SCENE = "Menu";
        public const string MINESWEEPER_SCENE = "MineSweeper";

        public static string GetScene(Enums.SceneList scene)
        {
            switch (scene)
            {
                case Enums.SceneList.init:
                    return INIT_SCENE;
                case Enums.SceneList.Menu:
                    return MENU_SCENE;
                case Enums.SceneList.MineSweeper:
                    return MINESWEEPER_SCENE;
                default:
                    return "-";
            }
        }

    }

    public class MineSweeper
    {
        public const byte TILE_SIZE = 2;
    }

    public class Vector2
    {
        public static Vector2SByte[] directions = new Vector2SByte[]
        {
            new Vector2SByte(1, 0),   // 우
            new Vector2SByte(-1, 0),  // 좌
            new Vector2SByte(0, 1),   // 상
            new Vector2SByte(0, -1)   // 하
        };
        public static Vector2SByte[] directions8Way = new Vector2SByte[]
            {
        // Cardinal Directions
        new Vector2SByte(1, 0),   // 우
        new Vector2SByte(-1, 0),  // 좌
        new Vector2SByte(0, 1),   // 상
        new Vector2SByte(0, -1),  // 하
        
        // Diagonal Directions
        new Vector2SByte(1, 1),   // 우상
        new Vector2SByte(1, -1),  // 우하
        new Vector2SByte(-1, 1),  // 좌상
        new Vector2SByte(-1, -1)  // 좌하
            };
    }


}