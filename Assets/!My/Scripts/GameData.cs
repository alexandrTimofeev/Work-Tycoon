using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

[Serializable]
public class GameData
{
     public IntContainer RocksContainer;
     public IntContainer MoneyContainer;
     public List<TetraObjectData> TetraObjectDatas;

    /// <summary>
    /// Возвращает полностью защищённый фасад
    /// </summary>
    public ReadOnlyGameData GetReadOnly()
    {
        return ReadOnlyGameData.Create(this);
    }
}

public class ReadOnlyGameData
{
    public ReadOnlyIntContainer RocksContainer { get; private set; }
    public ReadOnlyIntContainer MoneyContainer { get; private set; }

    public IReadOnlyDictionary<string, TetraObjectData> TetraObjectDatas;

    // Приватный конструктор, чтобы не создавать объект напрямую
    private ReadOnlyGameData() { }

    /// <summary>
    /// Фабричный метод для создания ReadOnlyGameData
    /// из GameData.
    /// </summary>
    public static ReadOnlyGameData Create(GameData data)
    {
        var readOnly = new ReadOnlyGameData
        {
            RocksContainer = new ReadOnlyIntContainer(data.RocksContainer),
            MoneyContainer = new ReadOnlyIntContainer(data.MoneyContainer),
            TetraObjectDatas = Utils.ToReadOnlyDictionary<string, TetraObjectData>(data.TetraObjectDatas.ToArray(),
            (tetra) => tetra.ID)
        };
        return readOnly;
    }
}