using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameExicuter : MonoBehaviour
{
    private List<TetraObject> tetraObjects = new List<TetraObject>();
    public Dictionary<Vector2Int, CellInfluenceInformation> InfluenceInformations = new Dictionary<Vector2Int, CellInfluenceInformation>();

    [Space]
    [SerializeField] private bool isDrawPowerUp = true;

    public event Action<Dictionary<Vector2Int, ReedOnlyCellInfluenceInformation>> OnUpdateInfluenceInformations;
    public event Action<TetraWorkInfo> OnTetraActionWork;

    public void Init()
    {
        InfluenceInformations.Clear();
        for (int x = 0; x < GameG.CellPlacer.GridGizmos.Cells.x; x++)
        {
            for (int y = 0; y < GameG.CellPlacer.GridGizmos.Cells.x; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                InfluenceInformations.Add(position, new CellInfluenceInformation() { Cell = position });
            }
        }
    }

    public void AddTetraObject (TetraObject obj)
    {
        tetraObjects.Add(obj);
        RecaluculateObjectInfluence();
    }

    public void RemoveTetraObject (TetraObject tetraObject)
    {
        tetraObjects.Remove(tetraObject);
        RecaluculateObjectInfluence();
    }

    public void RecaluculateObjectInfluence()
    {
        foreach (var cell in InfluenceInformations.Values)
        {
            cell.Clear();
        }

        List<(TetraAction, TetraBehaviour)> tetraActionsAndB = new List<(TetraAction, TetraBehaviour)>();
        foreach (var tetraObject in tetraObjects)
        {
            foreach (var tetraAction in tetraObject.TetraBehaviour.tetraActions)
            {
                tetraActionsAndB.Add((tetraAction, tetraObject.TetraBehaviour));
            }
        }
        tetraActionsAndB.Sort((ta, tb) => ta.Item1.Priority > tb.Item1.Priority ? 1 : (ta.Item1.Priority == tb.Item1.Priority ? 0 : -1));

        foreach (var actionAndB in tetraActionsAndB)
        {
            actionAndB.Item1.ApplyInfluence(actionAndB.Item2.GetActionContext(), ref InfluenceInformations);
        }

        OnUpdateInfluenceInformations?.Invoke(GetReedOnlyInfluanceInfo());
    }

    public Dictionary<Vector2Int, ReedOnlyCellInfluenceInformation> GetReedOnlyInfluanceInfo()
    {
        Dictionary<Vector2Int, ReedOnlyCellInfluenceInformation> keyValues = new Dictionary<Vector2Int, ReedOnlyCellInfluenceInformation>();
        foreach (var info in InfluenceInformations.Values)
        {
            keyValues.Add(info.Cell, new ReedOnlyCellInfluenceInformation(info));
        }
        return keyValues;
    }

    public static ConditionContext GetConditionContext()
    {
        return new ConditionContext(
            GameG.SessionData.RocksContainer.Value,
            GameG.SessionData.MoneyContainer.Value);
    }

    public bool TestCondition(CreateCondition createCondition, float coef = 1f)
    {
        return createCondition.TestConditionFloat(GetConditionContext(), coef);
    }

    private void OnDrawGizmos()
    {
        GridGizmos gridGizmos = FindFirstObjectByType<GridGizmos>(FindObjectsInactive.Include);
        string log = "";
        foreach (var pos in InfluenceInformations.Keys)
        {
            Gizmos.color = Color.yellow;
            //if (isDrawPowerUp && (InfluenceInformations[pos].PowerUpRock != 1f || InfluenceInformations[pos].PowerUpRock != 1f))
            //    Gizmos.DrawWireCube(gridGizmos.GetCellCenterWorld(pos.x, pos.y), gridGizmos.Grid.cellSize * 0.95f);

            //log += $"InfluenceInformations [{InfluenceInformations.Keys}] = " +
            //    $"({InfluenceInformations[pos].PowerUpRock != 0} || {InfluenceInformations[pos].PowerUpRock != 0})\n";
        }
        //Debug.Log(log);
    }

    public TetraObject GetTetraObject(Vector2Int pos)
    {
        return tetraObjects.FirstOrDefault((to) => to.TetraBehaviour.GetPositions().Contains(pos));
    }

    public void TertraObjectInvokeWork(TetraWorkInfo tetraWorkInfo)
    {
        OnTetraActionWork?.Invoke(tetraWorkInfo);
    }
}

[Serializable]
public struct TetraWorkInfo
{
    public TetraAction Action;
    public TetraActionContext Context;
    public List<TetraBehaviour> TetraBehaviourInvokers;

    public int ResultAddRock;
    public int ResultAddMoney;

    public TetraWorkInfo(TetraAction action, TetraActionContext context, List<TetraBehaviour> tetraBehaviours = null, 
        int resultAddRock = 0, int resultAddMoney = 0)
    {
        Action = action;
        Context = context;
        TetraBehaviourInvokers = tetraBehaviours == null ? new List<TetraBehaviour>() : tetraBehaviours;
        ResultAddRock = resultAddRock;
        ResultAddMoney = resultAddMoney;
    }
}

[Serializable]
public class CellInfluenceInformation
{
    public Vector2Int Cell;

    [Space]
    public float AddUpRock = 0f;
    public float AddUpMoney = 0f;
    public float PowerUpRock = 1f;
    public float PowerUpMoney = 1f;

    [Space]
    public float SpeedUpRock = 1f;
    public float SpeedUpMoney = 1f;

    [Space]
    public List<TetraObject> tetraObjects = new List<TetraObject>();

    public bool IsZero => PowerUpRock == 1f && PowerUpMoney == 1f && SpeedUpRock == 1f && SpeedUpMoney == 1f &&
        AddUpRock == 0 && AddUpMoney == 0;

    public void Clear()
    {
        AddUpRock = 0f;
        AddUpMoney = 0f;
        PowerUpRock = 1f;
        PowerUpMoney = 1f;
        SpeedUpRock = 1f;
        SpeedUpMoney = 1f;
        tetraObjects.Clear();
    }

    public CellInfluenceInformation()
    {
        Clear();
    }

    public CellInfluenceInformation(ReedOnlyCellInfluenceInformation reedOnly)
    {
        Cell = reedOnly.Cell;
        AddUpRock = reedOnly.AddUpRock;
        AddUpMoney = reedOnly.AddUpMoney;
        PowerUpMoney = reedOnly.PowerUpMoney;
        PowerUpRock = reedOnly.PowerUpRock;
        SpeedUpMoney = reedOnly.SpeedUpMoney;
        SpeedUpRock = reedOnly.SpeedUpRock;
    }

    public static CellInfluenceInformation operator +(CellInfluenceInformation A, CellInfluenceInformation B)
    {
        CellInfluenceInformation sum = new CellInfluenceInformation();
        sum.AddUpRock = B.AddUpRock + A.AddUpRock;
        sum.AddUpMoney = B.AddUpMoney + A.AddUpMoney;
        sum.PowerUpMoney = B.PowerUpMoney * A.PowerUpMoney;
        sum.PowerUpRock = B.PowerUpRock * A.PowerUpRock;
        sum.SpeedUpMoney = B.SpeedUpMoney * A.SpeedUpMoney;
        sum.SpeedUpRock = B.SpeedUpRock * A.SpeedUpRock;

        return sum;
    }

    public ReedOnlyCellInfluenceInformation GetReedOnly()
    {
        return new ReedOnlyCellInfluenceInformation(this);
    }

    public CellInfluenceInformationResult GetResult()
    {
        return new CellInfluenceInformationResult(this);
    }
}

public class ReedOnlyCellInfluenceInformation
{
    public readonly Vector2Int Cell;

    [Space]
    public readonly float AddUpRock = 0f;
    public readonly float AddUpMoney = 0f;
    public readonly float PowerUpRock = 1f;
    public readonly float PowerUpMoney = 1f;

    [Space]
    public readonly float SpeedUpRock = 1f;
    public readonly float SpeedUpMoney = 1f;

    public bool IsZero => PowerUpRock == 1f && PowerUpMoney == 1f && SpeedUpRock == 1f && SpeedUpMoney == 1f &&
        AddUpRock == 0 && AddUpMoney == 0;

    public ReedOnlyCellInfluenceInformation(CellInfluenceInformation cellInfluence)
    {
        Cell = cellInfluence.Cell;
        AddUpRock = (float)cellInfluence.AddUpRock;
        AddUpMoney = (float)cellInfluence.AddUpMoney;
        PowerUpRock = (float)cellInfluence.PowerUpRock;
        PowerUpMoney = (float)cellInfluence.PowerUpMoney;
        SpeedUpRock = (float)cellInfluence.SpeedUpRock;
        SpeedUpMoney = (float)cellInfluence.SpeedUpMoney;
    }
    
    public ReedOnlyCellInfluenceInformation(CellInfluenceInformation A, CellInfluenceInformation B)
    {
        CellInfluenceInformation sum = new CellInfluenceInformation();
        sum.AddUpRock = (float)B.AddUpRock + A.AddUpRock;
        sum.AddUpMoney = (float)B.AddUpMoney + A.AddUpMoney;
        sum.PowerUpRock = (float)B.PowerUpRock * A.PowerUpRock;
        sum.PowerUpMoney = (float)B.PowerUpMoney * A.PowerUpMoney;
        sum.SpeedUpRock = (float)B.SpeedUpRock * A.SpeedUpRock;
        sum.SpeedUpMoney = (float)B.SpeedUpMoney * A.SpeedUpMoney;
    }

    public string GetText(bool seeZero = false)
    {
        return GetResult().GetText();
        /*string text = "";

         if (IsZero && !seeZero)
             return text;

         float coefRock = 1f + AddUpRock;
         float coefMoney = 1f + AddUpMoney;
         coefRock *= PowerUpRock;
         coefMoney *= PowerUpMoney;

         if (seeZero || coefRock != 1f)
             text += $"{TB.R}{TB.X}{coefRock}\n";
         if (seeZero || coefMoney != 1f)
             text += $"{TB.M}{TB.X}{coefMoney}\n";

         if (seeZero || SpeedUpRock != 1f)
             text += $"{TB.Rs}{TB.X}{SpeedUpRock}\n";
         if (seeZero || SpeedUpMoney != 1f)
             text += $"{TB.Ms}{TB.X}{SpeedUpMoney}\n";

         return text;*/
    }

    public static ReedOnlyCellInfluenceInformation operator +(ReedOnlyCellInfluenceInformation A, ReedOnlyCellInfluenceInformation B)
    {
        return new ReedOnlyCellInfluenceInformation(new CellInfluenceInformation(A), new CellInfluenceInformation(A));
    }

    public CellInfluenceInformationResult GetResult()
    {
        return new CellInfluenceInformationResult(this);
    }
}

public struct CellInfluenceInformationResult
{
    [Space]
    public float CoefRock;
    public float CoefMoney;

    [Space]
    public float SpeedCoefRock;
    public float SpeedCoefMoney;
    public bool IsZero => CoefRock == 1f && CoefMoney == 1f && SpeedCoefRock == 1f && SpeedCoefMoney == 1f;


    public CellInfluenceInformationResult(bool empty = true)
    {
        CoefRock = 1f;
        CoefMoney = 1f;
        SpeedCoefRock = 1f;
        SpeedCoefMoney = 1f;
    }

    public CellInfluenceInformationResult(ReedOnlyCellInfluenceInformation reedOnly)
    {
        CoefRock = (1f + reedOnly.AddUpRock) * reedOnly.PowerUpRock;
        CoefMoney = (1f + reedOnly.AddUpMoney) * reedOnly.PowerUpMoney;
        SpeedCoefRock = reedOnly.SpeedUpRock;
        SpeedCoefMoney = reedOnly.SpeedUpMoney;
    }

    public CellInfluenceInformationResult(CellInfluenceInformation information)
    {
        CoefRock = (1f + information.AddUpRock) * information.PowerUpRock;
        CoefMoney = (1f + information.AddUpMoney) * information.PowerUpMoney;
        SpeedCoefRock = information.SpeedUpRock;
        SpeedCoefMoney = information.SpeedUpMoney;
    }

    public string GetText(bool seeZero = false)
    {
        string text = "";

        if (IsZero && !seeZero)
            return text;

        double minCoefRock = Math.Round(CoefRock, 1);
        string coefRockTxt = minCoefRock.ToString() + ((CoefRock - minCoefRock) != 0 ? TB.Dots : "");
        double minCoefMoney = Math.Round(CoefMoney, 1);
        string coefMoneyTxt = minCoefMoney.ToString() + ((CoefMoney - minCoefMoney) != 0 ? TB.Dots : "");

        if (seeZero || CoefRock != 1f)
            text += $"{TB.R}{TB.X}{coefRockTxt}\n";
        if (seeZero || CoefMoney != 1f)
            text += $"{TB.M}{TB.X}{coefMoneyTxt}\n";

        double minSpeedRock = Math.Round(SpeedCoefRock, 1);
        string coefSpeedRockTxt = minSpeedRock.ToString() + ((SpeedCoefRock - minSpeedRock) != 0 ? TB.Dots : "");
        double minSpeedMoney = Math.Round(SpeedCoefMoney, 1);
        string coefSpeedMoneyTxt = minSpeedMoney.ToString() + ((SpeedCoefMoney - minSpeedMoney) != 0 ? TB.Dots : "");

        if (seeZero || SpeedCoefRock != 1f)
            text += $"{TB.Rs}{TB.X}{coefSpeedRockTxt}\n";
        if (seeZero || SpeedCoefMoney != 1f)
            text += $"{TB.Ms}{TB.X}{coefSpeedMoneyTxt}\n";

        return text;
    }

    public static CellInfluenceInformationResult operator +(CellInfluenceInformationResult A, CellInfluenceInformationResult B)
    {
        return new CellInfluenceInformationResult()
        {
            CoefMoney = (float)B.CoefMoney * A.CoefMoney,
            CoefRock = (float)B.CoefRock * A.CoefRock,
            SpeedCoefMoney = (float)B.SpeedCoefMoney * A.SpeedCoefMoney,
            SpeedCoefRock = (float)B.SpeedCoefRock * A.SpeedCoefRock
        };
    }

    /*public static CellInfluenceInformationResult operator +(CellInfluenceInformationResult A, ReedOnlyCellInfluenceInformation B)
    {
        return new CellInfluenceInformationResult()
        {
            CoefMoney = (float)Math.Round(((1f + B.AddUpMoney) * B.PowerUpMoney) + A.CoefMoney, 1),
            CoefRock = (float)Math.Round(((1f + B.AddUpRock) * B.PowerUpRock) + A.CoefRock, 1),
            SpeedCoefMoney = (float)Math.Round(B.SpeedUpMoney + A.SpeedCoefMoney, 1),
            SpeedCoefRock = (float)Math.Round(B.SpeedUpRock + A.SpeedCoefRock, 1)
        };
    }*/
}