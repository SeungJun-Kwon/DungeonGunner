using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MovementByVelocityEvent : MonoBehaviour
{
    public event Action<MovementByVelocityEvent, MovementByVelocityEventArgs> OnMovementByVelocity;

    public void CallMovementByVelocityEvent(Vector2 moveDirection, float moveSpeed)
    {
        OnMovementByVelocity?.Invoke(this, new MovementByVelocityEventArgs()
        {
            _moveDirection = moveDirection,
            _moveSpeed = moveSpeed
        });
    }
}

public class MovementByVelocityEventArgs : EventArgs
{
    public Vector2 _moveDirection;
    public float _moveSpeed;
}