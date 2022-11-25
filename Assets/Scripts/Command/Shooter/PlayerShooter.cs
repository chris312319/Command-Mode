using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : BaseShooter
{
    ICommand command;
    new void Update()
    {
        command = HandleInput();
        if (command != nullCommand && command != null)
        {
            command.Execute(this);
            undolist.Push(command);
            if (redolist != null) redolist.Clear();
            Debug.Log("Execute:" + command);
        }
        if (Input.GetKey(KeyCode.Z) && undolist.Count != 0)
        {
            command = undolist.Pop();
            Debug.Log("Undo:" + command);
            command.Undo(this);
            redolist.Push(command);
        }
        if (Input.GetKey(KeyCode.X) && redolist.Count != 0)
        {
            command = redolist.Pop();
            Debug.Log("Redo:" + command);
            command.Execute(this);
            undolist.Push(command);
        }
    }
    // Start is called before the first frame update
    protected override ICommand HandleInput()
    {
        ICommand command = nullCommand;
        if (Input.GetKey(KeyCode.W))
        {
            command = new MoveForwardCommand(this.transform.position);
        }

        else if (Input.GetKey(KeyCode.S))
        {
            command = new MoveBackCommand(this.transform.position);
        }

        else if (Input.GetKey(KeyCode.A))
        {
            command = new MoveLeftCommand(this.transform.position);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            command = new MoveRightCommand(this.transform.position);
        }

        else if (Input.GetKey(KeyCode.Q))
        {
            command = new TurnLeftCommand(this.transform.localEulerAngles);
        }

        else if (Input.GetKey(KeyCode.E))
        {
            command = new TurnRightCommand(this.transform.localEulerAngles);
        }

        else if (Input.GetKey(KeyCode.J))
        {
            command = fire;
        }
        return command;
    }
}
