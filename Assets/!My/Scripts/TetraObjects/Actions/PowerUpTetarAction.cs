using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ApplyEffectType { Always, NotApply, Never }
public class PowerUpTetarAction : TetraAction
{
    public float AddUpRock = 0f;
    public float AddUpMoney = 0f;
    public float PowerUpRock = 1f;
    public float PowerUpMoney = 1f;

    [Space]
    public ApplyEffectType applyToUp = ApplyEffectType.Always;
    public ApplyEffectType applyToRight = ApplyEffectType.Always;
    public ApplyEffectType applyToLeft = ApplyEffectType.Always;
    public ApplyEffectType applyToDown = ApplyEffectType.Always;

    private TetraActionContext context;

    public override IEnumerator DoAction<T>(TetraActionContext context)
    {
        this.context = context;

        context.GridObject.OnPutInPlace += PutInPlaceWork;
        context.GridObject.OnDeliteUniversalGrid += DeliteWork;

        yield return base.DoAction<T>(context);
    }

    private void DeliteWork(IGridObject gridObject)
    {
        GameG.Exicuter.RecaluculateObjectInfluence();
    }

    private void PutInPlaceWork(IGridObject gridObject, Vector2Int position)
    {
        GameG.Exicuter.RecaluculateObjectInfluence();
    }

    public override void ApplyInfluence(TetraActionContext context, ref Dictionary<Vector2Int, CellInfluenceInformation> influenceInformations)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        List<Vector2Int> myPoses = new List<Vector2Int>();
        positions.AddRange(context.Behaviour.GetClosedPositions());
        myPoses.AddRange(context.Behaviour.GetPositions());

        foreach (var pos in positions)
        {
            if (influenceInformations.ContainsKey(pos) && TestToApply(pos, myPoses.ToArray()))
            {
                influenceInformations[pos].AddUpMoney += GetAddUpAtLevel(AddUpMoney, context.Level);
                influenceInformations[pos].AddUpRock += GetAddUpAtLevel(AddUpRock, context.Level);

                influenceInformations[pos].PowerUpRock *= GetPowerUpAtLevel(PowerUpRock, context.Level);
                influenceInformations[pos].PowerUpMoney *= GetPowerUpAtLevel(PowerUpMoney, context.Level);
            }
        }
        
        base.ApplyInfluence(context, ref influenceInformations);
    }

    private bool TestToApply(Vector2Int pos, Vector2Int[] myPoses)
    {
        bool hasNeighbour = false;
        bool allowed = false;

        foreach (var myPos in myPoses)
        {
            int dx = pos.x - myPos.x;
            int dy = pos.y - myPos.y;

            // только клетки на расстоянии 1
            if (Mathf.Abs(dx) + Mathf.Abs(dy) != 1)
                continue;

            hasNeighbour = true;

            // Up
            if (dx > 0)
            {
                if (applyToUp == ApplyEffectType.Never) return false;
                if (applyToUp == ApplyEffectType.Always) allowed = true;
            }
            // Down
            else if (dx < 0)
            {
                if (applyToDown == ApplyEffectType.Never) return false;
                if (applyToDown == ApplyEffectType.Always) allowed = true;
            }
            // Right
            else if (dy > 0)
            {
                if (applyToRight == ApplyEffectType.Never) return false;
                if (applyToRight == ApplyEffectType.Always) allowed = true;
            }
            // Left
            else if (dy < 0)
            {
                if (applyToLeft == ApplyEffectType.Never) return false;
                if (applyToLeft == ApplyEffectType.Always) allowed = true;
            }
        }

        // если соседей нет — эффект применяется
        if (!hasNeighbour)
            return true;

        return allowed;
    }

    public override string GetTextInfo(TetraActionContext context, string prefix = "")
    {
        float powerUpRock = GetPowerUpAtLevel(PowerUpRock, context.Level);
        float powerUpMoney = GetPowerUpAtLevel(PowerUpMoney, context.Level);
        float addUpRock = GetAddUpAtLevel(AddUpRock, context.Level);
        float addUpMoney = GetAddUpAtLevel(AddUpMoney, context.Level);

        if (powerUpRock == 1f && powerUpMoney == 1f)
            return "";

        string text = prefix + $"Production neighboring cells:\n";
        int countLine = 0;

        if (addUpRock != 0f)
        {
            if (countLine > 0)
                text += "\n";

            bool isAddRock = addUpRock > 1f;
            text += $"<color={(isAddRock ? "yellow" : "red")}>{TB.R}{TB.PAdd}{addUpRock}</color>";

            countLine++;
        }

        if (addUpMoney != 0f)
        {
            if (countLine > 0)
                text += "\n";

            bool isAddMoney = addUpMoney > 1f;
            text += $"<color={(isAddMoney ? "yellow" : "red")}>{TB.M}{TB.PAdd}{addUpMoney}</color>";

            countLine++;
        }

        if (powerUpRock != 1f)
        {
            if (countLine > 0)
                text += "\n";

            bool isAddRock = powerUpRock > 1f;
            text += $"<color={(isAddRock ? "yellow" : "red")}>{TB.R}{TB.PUp}{powerUpRock}</color>";

            countLine++;
        }

        if (powerUpMoney != 1f)
        {
            if (countLine > 0)
                text += "\n";

            bool isAddMoney = powerUpMoney > 1f;
            text += $"<color={(isAddMoney ? "yellow" : "red")}>{TB.M}{TB.PUp}{powerUpMoney}</color>";

            countLine++;
        }

        return text;
    }

    public float GetPowerUpAtLevel (float value, float level)
    {
        return (float)Math.Round(Mathf.Pow(value, level), 1);
    }

    public float GetAddUpAtLevel(float value, float level)
    {
        return (float)Math.Round(Mathf.Pow(value, level), 1);
    }
}