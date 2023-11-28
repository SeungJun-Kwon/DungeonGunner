using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AimWeaponEvent : MonoBehaviour
{
    public event Action<AimWeaponEvent, AimWeaponEventArgs> OnWeaponAim;

    public void CallAimWeaponEvent(AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        OnWeaponAim?.Invoke(this,
            new AimWeaponEventArgs() { _aimDirection = aimDirection, _aimAngle = aimAngle, _weaponAimAngle = weaponAimAngle, _weaponAimDirectionVector = weaponAimDirectionVector });


    }
}

public class AimWeaponEventArgs : EventArgs
{
    public AimDirection _aimDirection;
    public float _aimAngle;
    public float _weaponAimAngle;
    public Vector3 _weaponAimDirectionVector;
}