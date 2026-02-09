using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

public class CreateTetraObjectManager
{
    public Action<GameObject> OnCreateObject;
    public Action<TetraObject> OnCreateTetraObject;
    public Action<ApplyConditionInfo> OnApplyCondition;

    public GameObject CreateObject(string id, Vector3 position)
    {
        return CreateObject(G.GlobalData.TetraObjectDatas[id], position);
    }

    public GameObject CreateObject(TetraObjectData data, Vector3 position)
    {
        GameObject ngo = Object.Instantiate(data.Prefab, position, Quaternion.identity);
        if (ngo.TryGetComponent(out TetraObject tetraObject))
            tetraObject.Init(data);

        OnCreateObject?.Invoke(ngo);
        if(tetraObject)
            OnCreateTetraObject?.Invoke(tetraObject);

        return ngo;
    }

    public void ApplyCondition (ApplyConditionInfo info)
    {
        OnApplyCondition?.Invoke(info);
    }
}

[Serializable]
public struct ApplyConditionInfo
{
    public CreateCondition Condition;
    public float Coef;
    public Vector3? point;

    public ApplyConditionInfo(CreateCondition condition, float coef = 1f, Vector3? point = null)
    {
        Condition = condition;
        Coef = coef;
        this.point = point;
    }
}