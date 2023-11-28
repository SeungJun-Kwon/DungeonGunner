using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class AnimatePlayer : MonoBehaviour
{
    Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        _player._idleEvent.OnIdle += IdleEvent_OnIdle;
        _player._aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
        _player._movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
        _player._movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }

    private void OnDisable()
    {
        _player._idleEvent.OnIdle -= IdleEvent_OnIdle;
        _player._aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
        _player._movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
        _player._movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        InitializeRollAnimationParameters();
        SetIdleAnimationParameters();
    }

    void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        InitializeAimAnimationParameters();
        InitializeRollAnimationParameters();

        SetAimWeaponAnimationParameters(aimWeaponEventArgs._aimDirection);
    }

    void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityEventArgs movementByVelocityEventArgs)
    {
        SetMovementAnimationParameters();
    }

    void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionEventArgs movementToPositionEventArgs)
    {
        InitializeAimAnimationParameters();
        InitializeRollAnimationParameters();
        SetMovementToPositionAnimationParameters(movementToPositionEventArgs);
    }

    void SetIdleAnimationParameters()
    {
        _player._animator.SetBool(Settings.isMoving, false);
        _player._animator.SetBool(Settings.isIdle, true);
    }

    void SetMovementAnimationParameters()
    {
        _player._animator.SetBool(Settings.isMoving, true);
        _player._animator.SetBool(Settings.isIdle, false);
    }

    void InitializeAimAnimationParameters()
    {
        _player._animator.SetBool(Settings.aimDown, false);
        _player._animator.SetBool(Settings.aimLeft, false);
        _player._animator.SetBool(Settings.aimRight, false);
        _player._animator.SetBool(Settings.aimUp, false);
        _player._animator.SetBool(Settings.aimUpLeft, false);
        _player._animator.SetBool(Settings.aimUpRight, false);
    }

    void InitializeRollAnimationParameters()
    {
        _player._animator.SetBool(Settings.rollDown, false);
        _player._animator.SetBool(Settings.rollLeft, false);
        _player._animator.SetBool(Settings.rollRight, false);
        _player._animator.SetBool(Settings.rollUp, false);
    }

    void SetAimWeaponAnimationParameters(AimDirection aimDirection)
    {
        switch (aimDirection)
        {
            case AimDirection.Up:
                _player._animator.SetBool(Settings.aimUp, true);
                break;
            case AimDirection.Down:
                _player._animator.SetBool(Settings.aimDown, true);
                break;
            case AimDirection.Left:
                _player._animator.SetBool(Settings.aimLeft, true);
                break;
            case AimDirection.Right:
                _player._animator.SetBool(Settings.aimRight, true);
                break;
            case AimDirection.UpLeft:
                _player._animator.SetBool(Settings.aimUpLeft, true);
                break;
            case AimDirection.UpRight:
                _player._animator.SetBool(Settings.aimUpRight, true);
                break;
        }
    }

    void SetMovementToPositionAnimationParameters(MovementToPositionEventArgs movementToPositionEventArgs)
    {
        if (movementToPositionEventArgs._isRolling)
        {
            if(movementToPositionEventArgs._moveDirection.x > 0f)
            {
                _player._animator.SetBool(Settings.rollRight, true);
            }
            else if(movementToPositionEventArgs._moveDirection.x < 0f)
            {
                _player._animator.SetBool(Settings.rollLeft, true);
            }
            else if(movementToPositionEventArgs._moveDirection.y > 0f)
            {
                _player._animator.SetBool(Settings.rollUp, true);
            }
            else if (movementToPositionEventArgs._moveDirection.y < 0f)
            {
                _player._animator.SetBool(Settings.rollDown, true);
            }
        }
    }
}
