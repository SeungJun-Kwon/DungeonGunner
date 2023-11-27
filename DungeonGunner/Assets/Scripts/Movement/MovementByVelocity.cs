using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[DisallowMultipleComponent]
public class MovementByVelocity : MonoBehaviour
{
    Rigidbody2D _rigidBody2D;
    MovementByVelocityEvent _movementByVelocityEvent;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
    }

    private void OnEnable()
    {
        _movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void OnDisable()
    {
        _movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
    }

    void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityEventArgs movementByVelocityEventArgs)
    {
        MoveRigidBody(movementByVelocityEventArgs._moveDirection, movementByVelocityEventArgs._moveSpeed);
    }

    void MoveRigidBody(Vector2 moveDirection, float moveSpeed)
    {
        _rigidBody2D.velocity = moveDirection * moveSpeed;
    }
}
