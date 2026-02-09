using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class TetraObjectData
{
    public string ID;
    public GameObject Prefab;
    [HideInInspector] public InfoOverData InfoOverData;

    [Space]
    public Sprite Icon;
    public Color IconColor = Color.white;
    public string Name;

    [Space]
    public CreateCondition Condition;
    public CreateCondition ConditionUnlock;
    public CreateCondition ConditionUpgrade;

    public string GetTextCost(int level)
    {
        return Condition.GetTextCondition(level);
    }

    public string GetTextCostUpgrade(int level)
    {
        return ConditionUpgrade.GetTextCondition(level);
    }
}

[Serializable]
public class CreateCondition
{
    [SerializeField] private int costRock;
    [SerializeField] private int costMoney;

    public int CostRock => costRock;
    public int CostMoney => costMoney;

    public bool IsZero => costRock == 0 && costMoney == 0;

    public bool TestCondition(ConditionContext context, int level = 1)
    {
       /* if(context.MyRock < CostRock * level)
            return false;
        if (context.MyMoney < CostMoney * level)
            return false;
       */
        return TestConditionFloat(context, level);
    }

    public bool TestConditionFloat(ConditionContext context, float coef = 1)
    {
        if (context.MyRock < CostRock * coef)
            return false;
        if (context.MyMoney < CostMoney * coef)
            return false;

        return true;
    }

    public string GetTextCondition(float level)
    {
        string text = "";

        if (costRock > 0)
            text += $"{costRock * level} {TB.R}\n";

        if (costMoney > 0)
            text += $"{costMoney * level} {TB.M}\n";

        text += $"</color>";

        return text;
    }
}

public struct ConditionContext
{
    public int MyRock;
    public int MyMoney;

    public ConditionContext(int myRock, int myMoney)
    {
        MyRock = myRock;
        MyMoney = myMoney;
    }
}
