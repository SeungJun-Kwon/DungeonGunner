using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MovementToPositionEvent : MonoBehaviour
{
    public event Action<MovementToPositionEvent, MovementToPositionEventArgs> OnMovementToPosition;

    public void CallMovementToPositionEvent(Vector3 movePosition, Vector3 currentPosition, float moveSpeed, Vector2 moveDirection, bool isRolling = false)
    {
        OnMovementToPosition?.Invoke(this, new MovementToPositionEventArgs()
        {
            _movePosition = movePosition,
            _currentPosition = currentPosition,
            _moveSpeed = moveSpeed,
            _moveDirection = moveDirection,
            _isRolling = isRolling
        });
    }
}

public class MovementToPositionEventArgs : EventArgs
{
    public Vector3 _movePosition;
    public Vector3 _currentPosition;
    public float _moveSpeed;
    public Vector2 _moveDirection;
    public bool _isRolling;
}
