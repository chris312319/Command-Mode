using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeftCommand : ICommand
{
    public Vector3 startposition;
    public Vector3 resultposition;
    // Start is called before the first frame update
    public MoveLeftCommand(Vector3 pos)
    {
        startposition = pos;
    }

    public void Execute(IGameActor actor)
    {
        actor.MoveLeft();
    }
    public void Undo(IGameActor actor)
    {
        actor.MovetoPos(startposition);
    }
}