using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShooter : BaseShooter
{
    private ICommand[] Commands;
    private float stayTime = 2f;
    private ICommand _lastCommand;
    // Start is called before the first frame update
    protected override ICommand HandleInput()
    {
        if (_lastCommand == null) _lastCommand = moveForward;
        if(stayTime > 0)
        {
            stayTime -= Time.deltaTime;
            return _lastCommand;
        }
        else
        {
            stayTime = Random.Range(0f, 2f);
            int rand = Random.Range(0, 8);
            switch (rand)
            {
                case 1: _lastCommand = moveForward;
                    break;
                case 2:
                    _lastCommand = moveBack;
                    break;
                case 3:
                    _lastCommand = moveLeft;
                    break;
                case 4:
                    _lastCommand = moveRight;
                    break;
                case 5:
                    _lastCommand = turnLeft;
                    break;
                case 6:
                    _lastCommand = turnRight;
                    break;
                case 7:
                    _lastCommand = fire;
                    break;
            }
        }
        return _lastCommand;
    }
}
