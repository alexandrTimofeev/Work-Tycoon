using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GameMain : MonoBehaviour
{
    private static readonly List<PlayGameObject> _playGameObjects = new();
    private static readonly List<PlayGameObject> _playGameObjectsNew = new();

    public GameLoop Loop;
    public GameLoopContext Context;

    void Awake()
    {
        Context = new GameLoopContext(this);
    }

    void Start()
    {
        Loop = new GameLoop(new IGameStep[]
        {
            // Тут прописываешь шаги
            new StepMainGameState()
        });

        StartCoroutine(Loop.Run(Context));
    }

    public static void AddPlayGames(PlayGameObject obj)
    {
        if (!_playGameObjects.Contains(obj))
            _playGameObjects.Add(obj);

        //if (!_playGameObjectsNew.Contains(obj))
        _playGameObjectsNew.Add(obj);
    }

    public static void RemovePlayGames(PlayGameObject obj)
    {
        _playGameObjects.Remove(obj);
        _playGameObjectsNew.Remove(obj);
    }

    public static IEnumerator WaitAllContunityProcessStart<T>() where T : class, IPlayGameBase
    {
        _playGameObjectsNew.Clear();
        yield return WaitAllContunity<T>(_playGameObjects.ToArray());
    }

    public static IEnumerator WaitAllContunityProcessUpdate<T>() where T : class, IPlayGameBase
    {
        PlayGameObject[] array = _playGameObjectsNew.ToArray();
        yield return WaitAllContunity<T>(array);
        foreach (var playGame in array)        
            _playGameObjectsNew.Remove(playGame);        
    }

    public static IEnumerator WaitAllContunity<T>(PlayGameObject[] playGameObjects) where T : class, IPlayGameBase
    {
        foreach (var obj in playGameObjects)
        {
            if (obj == null)
                continue;

            foreach (var play in obj.GetPlayGameInterfaces<T>())
            {
                yield return play.PlayGame(); // здесь вызывается конкретная явная реализация
            }
        }
    }

    public static void UpdateAllContinuity<TStage>() where TStage : class, IPlayGameBase
    {
        foreach (var obj in _playGameObjects)
        {
            IPlayGameUpdate<TStage>[] updates = obj.GetPlayGameUpdateInterfaces<TStage>();
            foreach (var upd in updates)
                upd.UpdatePlayGame();
        }
    }
}