using DG.Tweening;
using System;
using UnityEngine;

// EntryPoint сцены Game
public class GameEntryPoint : ISceneEntryPoint
{
    public string SceneName => GameSceneManager.GameSceneName;
    public void InitGSystems() => GameG.Init();
    public void OnSceneLoaded()
    {
        Debug.Log("Game scene loaded");

        GameG.SessionData.Init(G.GlobalData);

        GameG.Player.Init(G._Input);
        InitButtonMediator();
        InitInterface();
        InitCreateManager();
        InitResourceEvents();

        AudioManager.Init();

        GameG.Exicuter.Init();
        GameG.CreateObjectScroll.Init(G._Input, GameG.CellPlacer);
    }

    private static void InitInterface()
    {
        InterfaceManager.Init();
        GameG.ScoreSys.OnScoreChange += (score, point) => InterfaceManager.BarMediator.ShowForID("Score", score);
        GameG.ScoreSys.OnAddScore += (score, point) => InterfaceManager.CreateScoreFlyingText(score, point);

        InterfaceManager.BarMediator.ShowForID("Score", 0);

        GameG.SessionData.RocksContainer.OnChangeValue += (value) => 
            InterfaceManager.BarMediator.ShowForID("Rocks", value);
        GameG.SessionData.MoneyContainer.OnChangeValue += (value) =>
            InterfaceManager.BarMediator.ShowForID("Money", value);

        DOVirtual.DelayedCall(Time.deltaTime, () =>
        {
            GameG.SessionData.MoneyContainer.OnChangeValue += (value) =>
            {
                bool record = LeaderBoard.SaveScore("Money", value);
                if (!GameG.UseRecordMoney && record)
                {
                    GameG.GroopRecordMoney.StartAnim();
                    GameG.UseRecordMoney = true;
                }
            };
            GameG.SessionData.RocksContainer.OnChangeValue += (value) =>
            {
                bool record = LeaderBoard.SaveScore("Rock", value);
                if (!GameG.UseRecordRock && record)
                {
                    GameG.GroopRecordRock.StartAnim();
                    GameG.UseRecordRock = true;
                }
            };
        });

        GameG.SessionData.RocksContainer.UpdateValue();
        GameG.SessionData.MoneyContainer.UpdateValue();

        InitCellUI();
    }

    private static void InitCellUI()
    {
        GameG.CellPlacerGODraw.Init(GameG.CellPlacer);
        foreach (var cell in GameG.CellPlacerGODraw.CellsGO.Keys)
        {
            GameG.DicCellInfoGOs.Add(cell, GameG.CellPlacerGODraw.CellsGO[cell].GetComponent<CellInfoGO>());
        }

        GameG.Exicuter.OnUpdateInfluenceInformations += (dicGridInfo) =>
        {
            foreach (var info in dicGridInfo.Values)
            {
                GameG.DicCellInfoGOs[info.Cell].SetInfo(info);
            }
        };
    }

    private void InitCreateManager()
    {
        GameG.CreateManager.OnApplyCondition += (infoApply) =>
        {
            if(infoApply.Condition.CostRock != 0)
                GameG.ResourceManager.AddRock(-(int)(infoApply.Condition.CostRock * infoApply.Coef), infoApply.point);
            if (infoApply.Condition.CostMoney != 0)
                GameG.ResourceManager.AddMoney(-(int)(infoApply.Condition.CostMoney * infoApply.Coef), infoApply.point);
        };

        GameG.SessionData.OnChangeResource += (cont) => GameG.CreateObjectScroll.TestAllButtons();

        GameG.CreateObjectScroll.OnStartDrag += (cont) => GameG.Player.isActive = false;
        GameG.CreateObjectScroll.OnEndDrag += (cont) => GameG.Player.isActive = true;
    }

    private void InitButtonMediator()
    {
        GameG.ButtonGameMediator.OnClick += (actionInfo) =>
        {
            switch (actionInfo.ActionType)
            {
                case ButtonGameActionType.None:
                    break;
                case ButtonGameActionType.Delite:
                    GameG.Player.DeliteCurrent();
                    break;
            }
        };
    }

    private void InitResourceEvents()
    {
        GameG.SessionData.RocksContainer.OnChangeValue += (value) => ChangeResourceWork(GameG.SessionData.RocksContainer, value);
        GameG.SessionData.MoneyContainer.OnChangeValue += (value) => ChangeResourceWork(GameG.SessionData.MoneyContainer, value);
    }

    private void ChangeResourceWork(IntContainer container, int newValue)
    {
        GameG.SessionData.OnChangeResource.Invoke(container);
    }

    public void OnSceneUnloaded()
    {
    }
}