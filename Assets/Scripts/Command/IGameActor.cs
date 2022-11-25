using UnityEngine;
public interface IGameActor
{
    void Fire();
    void MoveForward();
    void MoveLeft();
    void MoveRight();
    void MoveBack();
    void TurnLeft();
    void TurnRight();
    void MovetoPos(Vector3 pos);
    void RotatetoPos(Vector3 pos);
}