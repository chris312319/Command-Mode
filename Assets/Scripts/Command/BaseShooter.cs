using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseShooter : MonoBehaviour , IGameActor
{
    public float movespeed,rotatespeed;
    protected ICommand moveForward;
    protected ICommand moveBack;
    protected ICommand moveLeft;
    protected ICommand moveRight;
    protected ICommand turnRight;
    protected ICommand turnLeft;
    protected ICommand fire;
    protected ICommand nullCommand;
    public Stack<ICommand> undolist = new Stack<ICommand>();
    public Stack<ICommand> redolist = new Stack<ICommand>();

    void Start()
    {
        moveForward = CommandFactory.Create(KeyCode.W,this.gameObject);
        moveBack = CommandFactory.Create(KeyCode.S, this.gameObject);
        moveLeft = CommandFactory.Create(KeyCode.A, this.gameObject);
        moveRight = CommandFactory.Create(KeyCode.D, this.gameObject);
        turnLeft = CommandFactory.Create(KeyCode.Q, this.gameObject);
        turnRight = CommandFactory.Create(KeyCode.E, this.gameObject);
        fire = CommandFactory.Create(KeyCode.J, this.gameObject);
        nullCommand = CommandFactory.Create(default, this.gameObject);
    }

    // Update is called once per frame
    public void Update()
    {
        var command = HandleInput();
        if (command != nullCommand)
        {
            command.Execute(this);
            undolist.Push(command);
        }       
    }

    protected abstract ICommand HandleInput();
    
    public void Fire()
    {
       
    }
    public void TurnRight()
    {
        var lastEuler = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(lastEuler.x, lastEuler.y + rotatespeed * Time.deltaTime, lastEuler.z);
    }
    public void TurnLeft()
    {
        var lastEuler = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(lastEuler.x, lastEuler.y - rotatespeed * Time.deltaTime, lastEuler.z);
    }
    public void MoveForward()
    {
        transform.position += (transform.forward * (movespeed * Time.deltaTime));
    }
    public void MoveLeft()
    {
        transform.position -= (transform.right * (movespeed * Time.deltaTime));
    }
    public void MoveRight()
    {
        transform.position += (transform.right * (movespeed * Time.deltaTime));
    }
    public void MoveBack()
    {
        transform.position -= (transform.forward * (movespeed * Time.deltaTime));
    }

    public void MovetoPos(Vector3 pos)
    {
        this.transform.position = pos;
    }
    public void RotatetoPos(Vector3 pos)
    {
        this.transform.localEulerAngles = pos;
    }
}
