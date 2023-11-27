using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] MovementDetailsSO _movementDetails;

    [SerializeField] Transform _weaponShootPosition;

    Player _player;

    float _moveSpeed;

    private void Awake()
    {
        _player = GetComponent<Player>();

        _moveSpeed = _movementDetails.GetMoveSpeed();
    }

    private void Update()
    {
        MovementInput();

        WeaponInput();
    }

    void MovementInput()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

        // �밢 �̵����� 1�� �̵��Ϸ��� ���� �ﰢ�������� �ﰢ������ �����ϸ� �ȴ�
        // ���� ���ΰ� 0.7�� �����ﰢ���� ������ 1�� �ǹǷ� 0.7�� �����־� ũ�⸦ �����ϰ� �����ش�.
        if(horizontalMovement != 0f && verticalMovement != 0f)
        {
            direction *= 0.7f;
        }

        if (direction != Vector2.zero)
        {
            _player._movementByVelocityEvent.CallMovementByVelocityEvent(direction, _moveSpeed);
        }
        else
        {
            _player._idleEvent.CallIdleEvent();
        }
    }

    void WeaponInput()
    {
        Vector3 weaponDirection;
        float weaponAngleDegrees, playerAngleDegrees;

        AimDirection playerAimDirection;

        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);
    }

    void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
    {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        weaponDirection = mouseWorldPosition - _weaponShootPosition.position;

        Vector3 playerDirection = mouseWorldPosition - transform.position;

        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);

        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);

        _player._aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(_movementDetails), _movementDetails);
    }
#endif
}
