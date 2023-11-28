using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementToPositionEvent))]
[DisallowMultipleComponent]
public class MovementToPosition : MonoBehaviour
{
    Rigidbody2D _rigidBody2D;
    MovementToPositionEvent _movementToPositionEvent;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    private void OnEnable()
    {
        _movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }

    private void OnDisable()
    {
        _movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionEventArgs movementToPositionEventArgs)
    {
        MoveRigidBody(movementToPositionEventArgs._movePosition, movementToPositionEventArgs._currentPosition, movementToPositionEventArgs._moveSpeed);
    }

    void MoveRigidBody(Vector3 movePosition, Vector3 currentPosition, float moveSpeed)
    {
        // 단위 벡터를 구하는 이유
        // Speed 변수가 따로 존재하기 때문에 Speed에 따른 일정한 속도의 이동이 필요하다
        // 따라서 목표 방향의 단위벡터를 구한 후 Speed 값만큼 곱하면 일정한 속도만큼 목표 방향으로 나아가게 된다.
        Vector2 unitVector = Vector3.Normalize(movePosition - currentPosition);

        _rigidBody2D.MovePosition(_rigidBody2D.position + (unitVector * moveSpeed * Time.fixedDeltaTime));
    }
}
