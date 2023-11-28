using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AimWeaponEvent))]
[DisallowMultipleComponent]
public class AimWeapon : MonoBehaviour
{
    [SerializeField] Transform _weaponRotationPointTransform;

    AimWeaponEvent _aimWeaponEvent;

    private void Awake()
    {
        _aimWeaponEvent = GetComponent<AimWeaponEvent>();
    }

    private void OnEnable()
    {
        _aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        _aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
    {
        Aim(aimWeaponEventArgs._aimDirection, aimWeaponEventArgs._aimAngle);
    }

    void Aim(AimDirection aimDirection, float aimAngle)
    {
        _weaponRotationPointTransform.eulerAngles = new Vector3(0f, 0f, aimAngle);

        switch (aimDirection)
        {
            case AimDirection.Left:
            case AimDirection.UpLeft:
                _weaponRotationPointTransform.localScale = new Vector3(1f, -1f, 0);
                break;
            case AimDirection.Up:
            case AimDirection.Right:
            case AimDirection.UpRight:
            case AimDirection.Down:
                _weaponRotationPointTransform.localScale = new Vector3(1f, 1f, 0);
                break;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(_weaponRotationPointTransform), _weaponRotationPointTransform);
    }
#endif
}
