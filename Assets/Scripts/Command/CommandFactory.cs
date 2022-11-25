using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommandFactory
{
    // Start is called before the first frame update
    public static ICommand Create(KeyCode keycode,GameObject obj)
    {
        switch(keycode){
            case KeyCode.W: return new MoveForwardCommand(obj.transform.position);
            case KeyCode.S: return new MoveBackCommand(obj.transform.position);
            case KeyCode.A: return new MoveLeftCommand(obj.transform.position);
            case KeyCode.D: return new MoveRightCommand(obj.transform.position);
            case KeyCode.Q: return new TurnLeftCommand(obj.transform.localEulerAngles);
            case KeyCode.E: return new TurnRightCommand(obj.transform.localEulerAngles);
            case KeyCode.J: return new FireCommand();

            default: return new NullCommand();
        }
    }
}
