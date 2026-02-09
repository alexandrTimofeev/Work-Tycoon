using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ResourceManager
{
    public void Init()
    {

    }

    public void AddRock (int add, Vector3? point)
    {
        GameG.SessionData.RocksContainer.AddValue(add);
        if(point.HasValue)
            CreateFlyingText(add, TB.R, point.Value);
    }

    public void AddMoney(int add, Vector3? point)
    {
        GameG.SessionData.MoneyContainer.AddValue(add);
        string p = add > 0 ? "<color=green>+" : (add == 0 ? "<color=grey>+" : "<color=red>");
        if (point.HasValue)
            CreateFlyingText(add, TB.M, point.Value);
    }

    private void CreateFlyingText (int valueDelta, string resTxt, Vector3 point)
    {
        string p = valueDelta > 0 ? "<color=green>+" : (valueDelta == 0 ? "<color=grey>+" : "<color=red>");
        TextScoreUpLR textScoreUpLR = InterfaceManager.CreateFlyingText($"{p}{valueDelta}</color> {resTxt}", Color.white, point, null);

        textScoreUpLR.transform.LookAt(Camera.main.transform.position, Camera.main.transform.up);
        textScoreUpLR.transform.DOScale(1f, 15f);
    }
}