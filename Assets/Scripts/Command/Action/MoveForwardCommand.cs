using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForwardCommand : ICommand
{
    public Vector3 startposition;
    public Vector3 resultposition;
    // Start is called before the first frame update
    public MoveForwardCommand(Vector3 pos)
    {
        startposition = pos;
    }

    public void Execute(IGameActor actor)
    {
        actor.MoveForward();
    }
    public void Undo(IGameActor actor)
    {
        actor.MovetoPos(startposition);
    }
}
