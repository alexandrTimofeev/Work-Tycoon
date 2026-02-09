using System;
using UnityEngine;

public class GameSessionData : MonoBehaviour
{
    public IntContainer RocksContainer;
    public IntContainer MoneyContainer;

    public Action<IntContainer> OnChangeResource;

    public void Init(ReadOnlyGameData gameGlobalData)
    {
        RocksContainer = gameGlobalData.RocksContainer.CloneForEdit();
        MoneyContainer = gameGlobalData.MoneyContainer.CloneForEdit();

        OnChangeResource = null;
    }
}