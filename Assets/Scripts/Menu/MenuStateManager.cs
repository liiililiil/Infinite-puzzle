using SimpleActions;
using Type.Enums.Menu;





public class MenuStateManager : Managers<MenuStateManager>
{
    public SimpleEvent<MenuState> onMenuStateChanged = new SimpleEvent<MenuState>();

    private MenuState currentMenuState = MenuState.Init;

    private void Awake()
    {
        Singleton(false);
    }

    private void Start()
    {
        ChangeMenuState(MenuState.MainMenu);
    }

    //메뉴 변경
    public void ChangeMenuState(MenuState newState)
    {
        //같은 메뉴면 무시
        if (currentMenuState == newState) return;
        currentMenuState = newState;

        //메뉴 상태 변경 처리
        onMenuStateChanged.Invoke(newState);
    }

}
