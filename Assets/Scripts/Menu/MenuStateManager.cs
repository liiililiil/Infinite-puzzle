using System.Collections;
using System.Collections.Generic;
using SimpleActions;
using Type.Enums.Menu;
using UnityEngine;





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
        onMenuStateChanged.AddListener(OnChangeMenuState);
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

    private void OnChangeMenuState(MenuState menuState)
    {
        // Load 상태면 인게임 로딩
        if (menuState == MenuState.Load) StartCoroutine(Slower());

    }

    private IEnumerator Slower()
    {
        yield return new WaitForSeconds(1f);
        SceneListManager.Instance.LoadScene(Type.Enums.SceneList.MineSweeper);
    }

}
