using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpTetraAction : TetraAction
{
    public float SpeedCoefUpRock = 1f;
    public float SpeedCoefUpMoney = 1f;

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
                influenceInformations[pos].SpeedUpRock *= GetSpeedUpAtLevel(SpeedCoefUpRock, context.Level);
                influenceInformations[pos].SpeedUpMoney *= GetSpeedUpAtLevel(SpeedCoefUpMoney, context.Level);
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
        float speedUpRock = GetSpeedUpAtLevel(SpeedCoefUpRock, context.Level);
        float speedUpMoney = GetSpeedUpAtLevel(SpeedCoefUpMoney, context.Level);

        if (speedUpRock == 1f && speedUpMoney == 1f)
            return "";

        string text =  prefix + $"On neighboring cells:\n";

        if (speedUpRock != 1f)
        {
            bool isAddRock = speedUpRock > 1f;
            text += $"<color={(isAddRock ? "yellow" : "red")}>{TB.Rs}{TB.PUp}{speedUpRock}</color>";
        }

        if (speedUpMoney != 1f)
        {
            if (speedUpRock != 1f)
                text += "\n";
            bool isAddMoney = speedUpMoney > 1f;
            text += $"<color={(isAddMoney ? "yellow" : "red")}>{TB.Ms}{TB.PUp}{speedUpMoney}</color>";
        }

        return text;
    }

    public float GetSpeedUpAtLevel(float value, float level)
    {
        return (float)Math.Round(Mathf.Pow(value, (level + 0.5f) / 1f), 2);
    }
}