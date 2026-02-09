using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;

// Глобальные системы
public static class G
{
    public static IInput _Input;
    public static ReadOnlyGameData GlobalData;

    public static void Init()
    {
        Debug.Log("G initialized (global systems)");

        _Input = InputFabric.GetOrCreateInpit(true);
        GameGlobalData.ClearInstance();
        GlobalData = GameGlobalData.Instance.Data;
    }
}

// Локальные G-системы сцены Game
public static class GameG
{
    public static GameSessionData SessionData;

    public static CellPlacer CellPlacer;
    public static InteractorGame3D Player;
    //public static GameMain Main;
    public static ScoreSystem ScoreSys;
    public static CreateObjectScrollView CreateObjectScroll;
    public static CreateTetraObjectManager CreateManager;
    public static ButtonGameActionMediator ButtonGameMediator;
    public static GameExicuter Exicuter;
    public static CellPlacerGODrawer CellPlacerGODraw;
    public static Dictionary<Vector2Int, CellInfoGO> DicCellInfoGOs = new Dictionary<Vector2Int, CellInfoGO>();

    public static ResourceManager ResourceManager;

    public static GroopBlackAnimation GroopRecordRock;
    public static GroopBlackAnimation GroopRecordMoney;

    public static bool UseRecordRock;
    public static bool UseRecordMoney;

    public static void Init()
    {
        Debug.Log("GameG initialized (scene systems)");

        CellPlacer = Object.FindAnyObjectByType<CellPlacer>();
        Player = Object.FindAnyObjectByType<InteractorGame3D>(FindObjectsInactive.Include);
        //Main = Object.FindAnyObjectByType<GameMain>();
        CreateObjectScroll = Object.FindAnyObjectByType<CreateObjectScrollView>(FindObjectsInactive.Include);
        Exicuter = Object.FindAnyObjectByType<GameExicuter>(FindObjectsInactive.Include);
        CellPlacerGODraw = Object.FindAnyObjectByType<CellPlacerGODrawer>(FindObjectsInactive.Include);
        DicCellInfoGOs.Clear();

        GroopRecordRock = GameObject.Find("GroopRecordRock").GetComponent<GroopBlackAnimation>();
        GroopRecordMoney = GameObject.Find("GroopRecordMoney").GetComponent<GroopBlackAnimation>();

        ScoreSys = new ScoreSystem();
        SessionData = new GameSessionData();
        CreateManager = new CreateTetraObjectManager();
        ButtonGameMediator = new ButtonGameActionMediator();
        ResourceManager = new ResourceManager();

        UseRecordRock = false;
        UseRecordMoney = false;
    }
}

// Локальные G-системы сцены Menu
public static class MenuG
{
    public static void Init()
    {
        Debug.Log("MenuG initialized (scene systems)");
    }
}