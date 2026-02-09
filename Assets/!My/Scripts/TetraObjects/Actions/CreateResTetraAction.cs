using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CreateResTetraAction : TetraAction
{
    public override bool IsNeedProgress => true;
    public virtual bool IsPause { get; private set; }

    [Space]
    public int AddRock;
    public int AddMoney;

    [Space]
    public float timer;
    public float currentTimer;

    [Space]
    public CreateCondition addCondition;

    [Space]
    public bool notUseToPowerUp;
    public bool notUseToSpeedUp;

    [Space]
    public bool isDebug;

    public override void DoActionUpdate<T>(TetraActionContext context)
    {
        CellInfluenceInformationResult influenceInfo = GetSumInfluenceInformation(GetInfluenceInformation(context));
        float speed = 1f;
        if (!notUseToSpeedUp)
        {
            if (AddRock != 0 || (!addCondition.IsZero && addCondition.CostRock != 0))
                speed *= influenceInfo.SpeedCoefRock;
            if (AddMoney != 0 || (!addCondition.IsZero && addCondition.CostMoney != 0))
                speed *= influenceInfo.SpeedCoefMoney;
        }

        //Debug.Log($"speed {speed} {influenceInfo.SpeedCoefRock} {influenceInfo.SpeedCoefMoney}");
        if(!IsPause)
            currentTimer -= Time.deltaTime * 2f * speed;
        context.Behaviour.SetProggress(currentTimer / timer);

        if (currentTimer < 0)
        {
            DoAndBreakTimer(context, influenceInfo);
        }
    }

    public bool DoAndBreakTimer(TetraActionContext context, CellInfluenceInformationResult influenceInfo)
    {
        currentTimer = timer;

        if (!addCondition.IsZero)
        {
            float coef = GetCoefCondition(context, influenceInfo);

            if (GameG.Exicuter.TestCondition(addCondition, coef) == false)
                return false;

            GameG.CreateManager.ApplyCondition(new ApplyConditionInfo(addCondition, coef, GetPosForText(context)));
        }

        int addRock = AddRock * context.Level;
        int addMoney = AddMoney * context.Level;

        if (!notUseToPowerUp)
        {
            addRock = (int)(addRock * influenceInfo.CoefRock);
            addMoney = (int)(addMoney * influenceInfo.CoefMoney);
        }

        ApplyAdditionOperations(context, ref addRock, ref addMoney);

        if (AddRock != 0 || addRock != 0)
            GameG.ResourceManager.AddRock(addRock, GetPosForText(context));

        if (AddMoney != 0 || addMoney != 0)
            GameG.ResourceManager.AddMoney(addMoney, GetPosForText(context));

        if (AddRock != 0 || addRock != 0 || AddMoney != 0 || addMoney != 0)
            InvokeWork(context, influenceInfo, default, addRock, addMoney);

        if (isDebug)
            Debug.Log($"{context.Behaviour.gameObject.name} do CreateRes: ResultAddRock {addRock} ({AddRock}); ResultAddMoney {addMoney} ({AddMoney})");
        return true;
    }

    public virtual void InvokeWork(TetraActionContext context, CellInfluenceInformationResult influenceInfo,
        List<TetraBehaviour> behavioursInvokers = null, int addRock = 0, int addMoney = 0)
    {
        if (behavioursInvokers == null)
            behavioursInvokers = new List<TetraBehaviour>();
        behavioursInvokers.Add(context.Behaviour);
        GameG.Exicuter.TertraObjectInvokeWork(new TetraWorkInfo(this, context, behavioursInvokers, addRock, addMoney));
    }

    public virtual void ApplyAdditionOperations(TetraActionContext context, ref int addRock, ref int addMoney)
    {

    }

    private static Vector3 GetPosForText(TetraActionContext context)
    {
        return context.Tetra.transform.position + (Vector3.up * 2) + Random.insideUnitSphere;
    }

    private float GetCoefCondition(TetraActionContext context)
    {
        return GetCoefCondition(context, new CellInfluenceInformationResult(true));
    }

    private float GetCoefCondition(TetraActionContext context, CellInfluenceInformationResult influenceInfo)
    {
        float coef = context.Level;
        if (!notUseToPowerUp && !influenceInfo.IsZero)
        {
            if (addCondition.CostRock > 0)
                coef *= influenceInfo.CoefRock;

            if (addCondition.CostMoney > 0)
                coef *= influenceInfo.CoefMoney;
        }

        return coef;
    }

    public override string GetTextInfo(TetraActionContext context, string prefix = "")
    {
        int addRock = AddRock * context.Level;
        int addMoney = AddMoney * context.Level;

        if (addRock == 0 && addMoney == 0)
            return "";
        
        string text = prefix + GetTextPrefixValue();
        text += GetTextValues(context, addRock, addMoney);

        return text;
    }

    public virtual string GetTextPrefixValue()
    {
        return $"every {timer}s:\n";
    }

    public virtual string GetTextValues(TetraActionContext context, int addRock, int addMoney)
    {
        string text = "";

        if (!addCondition.IsZero)
        {
            text += $"<color=blue>GET: ";
            text += addCondition.GetTextCondition(GetCoefCondition(context));
            text += $"</color> for:\n\n";
        }

        if (addRock != 0)
        {
            bool isAddRock = addRock > 0;
            text += $"<color={(isAddRock ? "yellow" : "red")}>Rock {(isAddRock ? $"+" : "")}{addRock}</color>\n";
        }

        if (addMoney != 0)
        {
            bool isAddMoney = addMoney > 0;
            text += $"<color={(isAddMoney ? "yellow" : "red")}>Money {(isAddMoney ? $"+" : "")}{addMoney}</color>\n";
        }

        if (notUseToPowerUp || notUseToSpeedUp)
        {
            int countRecivesLine = 0;
            text += "Not recive: ";

            if (notUseToPowerUp)
            {
                if (countRecivesLine > 0)
                    text += ", ";
                text += TB.PowerUp;
                countRecivesLine++;
            }

            if (notUseToSpeedUp)
            {
                if (countRecivesLine > 0)
                    text += ", ";
                text += TB.SpeedUp;
                countRecivesLine++;
            }
        }

        return text;
    }
}
