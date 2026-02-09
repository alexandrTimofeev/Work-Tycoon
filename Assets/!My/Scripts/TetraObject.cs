using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TetraObject : MonoBehaviour
{
    private GridUniversalInteractable3D gridUniversal;
    private TetraBehaviour tetraBehaviour;

    [Space]
    [SerializeField] private TMP_Text levelTmp;
    private int level;

    public TetraObjectData Data { get; private set; }
    public TetraBehaviour TetraBehaviour => tetraBehaviour;
    public GridUniversalInteractable3D GridUniversal => gridUniversal;

    public void Init(TetraObjectData data)
    {
        this.Data = data;
        gridUniversal = GetComponent<GridUniversalInteractable3D>();
        tetraBehaviour = GetComponent<TetraBehaviour>();
        tetraBehaviour.Init();

        gridUniversal.OnDelite += DeliteWork;

        GameG.Exicuter.AddTetraObject(this);
    }

    private void DeliteWork(MonoBehaviour behaviour)
    {
        GameG.SessionData.RocksContainer.AddValue(Data.Condition.CostRock / 2);
        GameG.SessionData.MoneyContainer.AddValue(Data.Condition.CostMoney / 2);
        GameG.Exicuter.RemoveTetraObject(this);
    }

    public void ApplyInfluence(ref Dictionary<Vector2Int, CellInfluenceInformation> influenceInformations)
    {
        tetraBehaviour.ApplyInfluence(ref influenceInformations);
    }

    public void SetLevel (int level)
    {
        this.level = level;
        levelTmp.text = $"Lvl {level}";
    }
}