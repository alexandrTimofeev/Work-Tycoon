using System.Collections;

public interface IPlayGameBase : IOrdered
{
    IEnumerator PlayGame();
}

public interface IPlayGame<in TStage>
    where TStage : class, IPlayGameBase
{
    IEnumerator PlayGame()
    {
        yield return (this as TStage).PlayGame();
    }
}

public interface IPlayGameUpdate<in TStage>
    where TStage : IPlayGameBase
{
    void UpdatePlayGame();
}

//-----------------------------------------

public interface IPlayGameMainGameState : IPlayGameBase { new IEnumerator PlayGame(); }

//-----------------------------------------
public interface IOrdered
{
    int Order();
}