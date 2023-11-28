using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] MovementDetailsSO _movementDetails;

    [SerializeField] Transform _weaponShootPosition;

    Player _player;

    float _moveSpeed;

    Coroutine _playerRollCoroutine;
    WaitForFixedUpdate _waitForFixedUpdate;
    bool _isPlayerRolling = false;
    float _playerRollCoolDownTimer = 0f;

    private void Awake()
    {
        _player = GetComponent<Player>();

        _moveSpeed = _movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        _waitForFixedUpdate = new();
    }

    private void Update()
    {
        if (_isPlayerRolling)
            return;

        MovementInput();

        WeaponInput();

        PlayerRollCoolDownTimer();
    }

    void MovementInput()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        bool rightMouseButtonDown = Input.GetMouseButtonDown(1);

        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

        // 대각 이동에서 1을 이동하려면 직각 삼각형에서의 삼각공식을 생각하면 된다
        // 가로 세로가 0.7인 직각삼각형의 빗변이 1이 되므로 0.7을 곱해주어 크기를 동일하게 맞춰준다.
        if(horizontalMovement != 0f && verticalMovement != 0f)
        {
            direction *= 0.7f;
        }

        if (direction != Vector2.zero)
        {
            if (!rightMouseButtonDown)
            {
                _player._movementByVelocityEvent.CallMovementByVelocityEvent(direction, _moveSpeed);
            }
            else if(_playerRollCoolDownTimer <= 0f)
            {
                PlayerRoll((Vector3)direction);
            }
        }
        else
        {
            _player._idleEvent.CallIdleEvent();
        }
    }

    void PlayerRoll(Vector3 direction)
    {
        _playerRollCoroutine = StartCoroutine(PlayerRollCoroutine(direction));
    }

    IEnumerator PlayerRollCoroutine(Vector3 direction)
    {
        // 구르기를 위한 타겟 과의 최소 거리
        float minDistance = 0.2f;

        _isPlayerRolling = true;

        Vector3 targetPosition = _player.transform.position + direction * _movementDetails._rollDistance;

        while(Vector3.Distance(_player.transform.position, targetPosition) > minDistance)
        {
            _player._movementToPositionEvent.CallMovementToPositionEvent(targetPosition, _player.transform.position, _movementDetails._rollSpeed, direction, _isPlayerRolling);

            yield return _waitForFixedUpdate;
        }

        _isPlayerRolling = false;

        _playerRollCoolDownTimer = _movementDetails._rollCoolDownTime;

        _player.transform.position = targetPosition;
    }

    void PlayerRollCoolDownTimer()
    {
        if(_playerRollCoolDownTimer >= 0f)
        {
            _playerRollCoolDownTimer -= Time.deltaTime;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StopPlayerRollRoutine();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        StopPlayerRollRoutine();
    }

    void StopPlayerRollRoutine()
    {
        if(_playerRollCoroutine != null)
        {
            StopCoroutine(_playerRollCoroutine);

            _isPlayerRolling = false;

            _playerRollCoroutine = null;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(_movementDetails), _movementDetails);
    }
#endif
}
