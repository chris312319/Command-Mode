using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCommand : ICommand
{
    // Start is called before the first frame update
    public void Execute(IGameActor actor)
    {
        actor.Fire();
    }
    public void Undo(IGameActor actor)
    {

    }
}