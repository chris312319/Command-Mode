using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackCommand : ICommand
{
    public Vector3 startposition;
    public Vector3 resultposition;
    // Start is called before the first frame update
    public MoveBackCommand(Vector3 pos)
    {
        startposition = pos;
    }
    public void Execute(IGameActor actor)
    {
        actor.MoveBack();
    }
    public void Undo(IGameActor actor)
    {
        //actor.MoveForward();
        actor.MovetoPos(startposition);
    }
}