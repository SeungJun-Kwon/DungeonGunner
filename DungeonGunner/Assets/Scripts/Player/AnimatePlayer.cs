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
    }

    void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        SetIdleAnimationParameters();
    }

    void SetIdleAnimationParameters()
    {
        _player._animator.SetBool(Settings.isMoving, false);
        _player._animator.SetBool(Settings.isIdle, true);
    }

    void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        InitializeAimAnimationParameters();

        SetAimWeaponAnimationParameters(aimWeaponEventArgs._aimDirection);
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
}
