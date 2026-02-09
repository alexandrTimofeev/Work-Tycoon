using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlayGameObject))]
public class TetraBehaviour : MonoBehaviour,
    IPlayGame<IPlayGameMainGameState>,
    IPlayGameUpdate<IPlayGameMainGameState>,
    IPlayGameBase
{
    [SerializeReference] public List<TetraAction> tetraActions = new();
    [SerializeReference] public List<TetraAction> tetraActionsUpdate = new();
    [SerializeField] private int order = 100;

    [Space]
    [SerializeField] private RadialProgress3D radialProgress;

    private TetraObject tetraObject;
    private GridUniversalInteractable3D universalGridObject;
    private int level;

    public void Init()
    {
        tetraObject = GetComponent<TetraObject>();
        universalGridObject = GetComponent<GridUniversalInteractable3D>();

        radialProgress.gameObject.SetActive(false);
        foreach (var action in tetraActions)
        {
            if (action.IsNeedProgress)
            {
                radialProgress.gameObject.SetActive(true);
                return;
            }
        }

        foreach (var action in tetraActionsUpdate)
        {
            if (action.IsNeedProgress)
            {
                radialProgress.gameObject.SetActive(true);
                break;
            }
        }
    }

    int IOrdered.Order() => order;

    // PlayGame один раз на входе шага
    IEnumerator IPlayGame<IPlayGameMainGameState>.PlayGame()
    {
        foreach (var action in tetraActions)
            yield return action.DoAction<IPlayGameMainGameState>(GetActionContext());
    }

    public TetraActionContext GetActionContext()
    {
        return new TetraActionContext { 
            Behaviour = this,
            Tetra = tetraObject,
            GridObject = universalGridObject,
            Level = level
        };
    }

    // UpdatePlayGame вызывается каждый кадр
    void IPlayGameUpdate<IPlayGameMainGameState>.UpdatePlayGame()
    {
        foreach (var action in tetraActionsUpdate)
            action.DoActionUpdate<IPlayGameMainGameState>(GetActionContext());
        //Debug.Log($"[TetraBehaviour] IPlayGameUpdate<IPlayGameMainGameState>.UpdatePlayGame {gameObject.name}");
    }

    // Для совместимости с PlayGameObject
    public IEnumerator PlayGame()
    {
        Debug.Log($"[TetraBehaviour] base.PlayGame {gameObject.name}");
        yield return null;
    }
    public void UpdatePlayGame()
    {
        Debug.Log($"[TetraBehaviour] base.UpdatePlayGame {gameObject.name}");
    }

    public void ApplyInfluence(ref Dictionary<Vector2Int, CellInfluenceInformation> influenceInformations)
    {
        foreach(var action in tetraActions)
            action.ApplyInfluence(GetActionContext(), ref influenceInformations);
    }

    public Vector2Int[] GetPositions()
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        positions.Add(universalGridObject.CellPosition);
        foreach (var offset in universalGridObject.MyPlaces)
        {
            positions.Add(universalGridObject.CellPosition + offset);
        }
        return positions.ToArray();
    }

    public Vector2Int[] GetClosedPositions()
    {
        Vector2Int[] myPositions = GetPositions();
        List<Vector2Int> positions = new List<Vector2Int>();

        Vector2Int[] offsets = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        foreach (var pos in myPositions)
        {
            foreach (var offset in offsets)
                if(!positions.Contains(pos + offset) && !myPositions.Contains(pos + offset))
                    positions.Add(pos + offset);
        }

        return positions.ToArray();
    }

    public string GetInfoBehaviourText(int level = -1)
    {
        string text = "";

        TetraActionContext tetraActionContext = GetActionContext();
        if(level != -1)
            tetraActionContext.Level = level;

        if (tetraActions.Count > 0)
        {
            //text += "ON CREATE:\n";
            foreach (var action in tetraActions)
            {
                text += $"{action.GetTextInfo(tetraActionContext)}\n\n";
            }
        }

        if (tetraActionsUpdate.Count > 0)
        {
            //text += "WORK:\n";
            foreach (var action in tetraActionsUpdate)
            {
                text += $"{action.GetTextInfo(tetraActionContext)}\n\n";
            }
        }

        return text;
    }

    public void SetLevel(int level)
    {
        this.level = level; 
    }

    public void SetProggress (float unProggress)
    {
        radialProgress.SetProgress(unProggress);
    }
}

[Serializable]
public class TetraAction
{
    public string ID;
    public int Priority = 1000;

    public virtual bool IsNeedProgress => false;

    public virtual IEnumerator DoAction<T>(TetraActionContext context) where T : IPlayGameBase
    {
        yield break;
    }

    public virtual void DoActionUpdate<T> (TetraActionContext context) where T : IPlayGameBase
    { 
    }

    public virtual void ApplyInfluence(TetraActionContext context, ref Dictionary<Vector2Int, CellInfluenceInformation> influenceInformations)
    {
    }

    public virtual string GetTextInfo(TetraActionContext context, string prefix = "")
    {
        return "[no info]";
    }

    public CellInfluenceInformation[] GetInfluenceInformation(TetraActionContext context)
    {
        List<CellInfluenceInformation> cellInfluenceInformation = new List<CellInfluenceInformation>();
        Vector2Int[] poses = context.Behaviour.GetPositions();
        foreach (var pos in poses)
        {
            if (!GameG.Exicuter.InfluenceInformations.ContainsKey(pos))
                continue;

            cellInfluenceInformation.Add(GameG.Exicuter.InfluenceInformations[pos]);
        }

        return cellInfluenceInformation.ToArray();
    }

    public CellInfluenceInformationResult GetSumInfluenceInformation (CellInfluenceInformation[] informations)
    {
        CellInfluenceInformationResult result = new CellInfluenceInformationResult(true);
        foreach (var info in informations)
        {
            result = (result + info.GetResult());
        }

        return result;
    }
}

// Контекст для действий
public class TetraActionContext
{
    public TetraBehaviour Behaviour;
    public TetraObject Tetra;
    public GridUniversalInteractable3D GridObject;
    public int Level;
}

// Пример наследника
[Serializable]
public class ExampleAction : TetraAction
{
    public float someValue = 1f;
    public string actionName = "New Action";

    public override IEnumerator DoAction<T>(TetraActionContext context)
    {
        Debug.Log($"Executing {actionName} with value {someValue}");
        yield return new WaitForSeconds(someValue);
    }

    public override void DoActionUpdate<T>(TetraActionContext context)
    {
        Debug.Log($"Executing {actionName} UPDATE");
    }
}