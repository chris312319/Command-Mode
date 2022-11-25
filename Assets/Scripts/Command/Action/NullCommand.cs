using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullCommand : ICommand
{
    // Start is called before the first frame update
    public void Execute(IGameActor actor)
    {
        
    }
    public void Undo(IGameActor actor)
    {
        
    }
}