namespace Type.Consts
{

    public class Text
    {
        public const string MAIN_MENU = "MainMenuText";
    }

    public class Audio
    {
        public const string MUSIC = "Music";
        public const string MUSICINFO = "MusicInfo";
        public const string PLAYERABLE = "Playerable";
        public const string BACKGROUNDINFO = "BackGroundInfo";
    }

    public class Sprite
    {
        public const string MAIN_MENU = "MainMenuSprite";
    }

    public class Prefab
    {
        public const string MAIN_MENU = "MainMenuPrefab";
    }

    public class Scene
    {
        public const string INIT_SCENE = "Init";
        public const string MENU_SCENE = "Menu";
        public const string INPLAYING_SCENE = "InPlaying";

        public static string GetScene(Enums.SceneList scene)
        {
            switch (scene)
            {
                case Enums.SceneList.init:
                    return INIT_SCENE;
                case Enums.SceneList.Menu:
                    return MENU_SCENE;
                case Enums.SceneList.InPlaying:
                    return INPLAYING_SCENE;
                default:
                    return "-";
            }
        }

    }



}