    public interface ICommand
    {
    void Execute(IGameActor actor);

    void Undo(IGameActor actor);

    }