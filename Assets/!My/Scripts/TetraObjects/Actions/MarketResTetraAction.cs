using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MarketResTetraAction : CreateResTetraAction
{
    public override bool IsNeedProgress => true;

    [Space]
    public bool calculaeteOnlyNew = false;
    public float AddCoefForNigberhod = 0f;
    public float PowerCoefForNigberhod = 1f;

    public override void ApplyAdditionOperations(TetraActionContext context, ref int addRock, ref int addMoney)
    {
        base.ApplyAdditionOperations(context, ref addRock, ref addMoney);
        Vector2Int[] closePoses = context.Behaviour.GetClosedPositions();

        int countNiber = 0;
        List<IGridObject> gridObjects = new List<IGridObject>();

        foreach (Vector2Int pos in closePoses)
        {
            IGridObject gridObject = GameG.CellPlacer.GetObject(pos, true);
            if (gridObject == null)
                continue;

            if (calculaeteOnlyNew && gridObjects.Contains(gridObject))
                continue;
            gridObjects.Add(gridObject);

            GridUniversalInteractable3D gridUniversal = gridObject as GridUniversalInteractable3D;
            if (gridUniversal == null)
                continue;

            TetraObject tetraObject = gridUniversal.GetComponent<TetraObject>();
            if (tetraObject != null)            
                countNiber++;            
        }

        float coefRock = (1f + (AddCoefForNigberhod * countNiber)) * (1f + (PowerCoefForNigberhod * countNiber));
        float coefMoney = (1f + (AddCoefForNigberhod * countNiber)) * (1f + (PowerCoefForNigberhod * countNiber));
        addRock = (int)(addRock * coefRock);
        addMoney = (int)(addMoney * coefMoney);
    }

    public override string GetTextInfo(TetraActionContext context, string prefix)
    {
        string text = base.GetTextInfo(context, prefix);
        text += $"\nBonus for the neighbors:";

        if(AddCoefForNigberhod != 0)
            text += $" {TB.PAdd}{AddCoefForNigberhod}";

        if (PowerCoefForNigberhod != 0)
            text += $" {TB.PUp}{PowerCoefForNigberhod}";

        return text;
    }
}