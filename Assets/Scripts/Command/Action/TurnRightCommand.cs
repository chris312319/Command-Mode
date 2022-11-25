using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnRightCommand : ICommand
{
    public Vector3 startrotation;
    public Vector3 resultrotation;
    // Start is called before the first frame update
    public TurnRightCommand(Vector3 pos)
    {
        startrotation = pos;
    }

    public void Execute(IGameActor actor)
    {
        actor.TurnRight();
    }

    public void Undo(IGameActor actor)
    {
        actor.RotatetoPos(startrotation);
    }
}