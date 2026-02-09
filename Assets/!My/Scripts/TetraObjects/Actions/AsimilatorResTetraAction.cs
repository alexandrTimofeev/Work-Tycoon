using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AsimilatorResTetraAction : CreateResTetraAction
{
    public override bool IsNeedProgress => false;
    public override bool IsPause => true;

    TetraActionContext context = null;    

    public override IEnumerator DoAction<T>(TetraActionContext context)
    {
        this.context = context;

        GameG.Exicuter.OnTetraActionWork += TetraWorkWork();
        context.GridObject.OnDeliteUniversalGrid += DeliteWork;

        yield return base.DoAction<T>(context);
    }

    private void DeliteWork(IGridObject @object)
    {
        GameG.Exicuter.OnTetraActionWork -= TetraWorkWork();
    }

    private Action<TetraWorkInfo> TetraWorkWork()
    {
        return (info) => TetraWorkWork(info, context);
    }

    private Action<TetraWorkInfo> TetraWorkWork(TetraActionContext context)
    {
        return (info) => TetraWorkWork(info, context);
    }

    private void TetraWorkWork(TetraWorkInfo info, TetraActionContext context)
    {
        if (info.TetraBehaviourInvokers != null && info.TetraBehaviourInvokers.Contains(context.Behaviour))
            return;

        Vector2Int[] closedPoses = context.Behaviour.GetClosedPositions();
        Vector2Int[] himPoses = info.Context.Behaviour.GetPositions();

        if (!himPoses.Any((pos) => closedPoses.Contains(pos)))
            return;

        if (info.ResultAddRock <= 0 && info.ResultAddMoney <= 0)
            return;

        CellInfluenceInformationResult influenceInfo = GetSumInfluenceInformation(GetInfluenceInformation(context));
        tetraBehaviours = info.TetraBehaviourInvokers;
        DoAndBreakTimer(context, influenceInfo);
    }

    List<TetraBehaviour> tetraBehaviours = new List<TetraBehaviour>();
    public override void InvokeWork(TetraActionContext context, CellInfluenceInformationResult influenceInfo,
        List<TetraBehaviour> behavioursInvokers = null, int addRock = 0, int addMoney = 0)
    {
        if(behavioursInvokers == null)
            behavioursInvokers = new List<TetraBehaviour>();
        behavioursInvokers.AddRange(tetraBehaviours);

        base.InvokeWork(context, influenceInfo, tetraBehaviours, addRock, addMoney);
    }

    public override string GetTextPrefixValue()
    {
        return "When the neighbor is triggered:\n";
    }
}