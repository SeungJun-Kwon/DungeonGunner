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
        // ���� ���͸� ���ϴ� ����
        // Speed ������ ���� �����ϱ� ������ Speed�� ���� ������ �ӵ��� �̵��� �ʿ��ϴ�
        // ���� ��ǥ ������ �������͸� ���� �� Speed ����ŭ ���ϸ� ������ �ӵ���ŭ ��ǥ �������� ���ư��� �ȴ�.
        Vector2 unitVector = Vector3.Normalize(movePosition - currentPosition);

        _rigidBody2D.MovePosition(_rigidBody2D.position + (unitVector * moveSpeed * Time.fixedDeltaTime));
    }
}
