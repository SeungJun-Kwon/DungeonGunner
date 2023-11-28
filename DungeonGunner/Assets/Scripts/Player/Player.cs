using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(MovementToPositionEvent))]
[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerDetailsSO _playerDetails;
    [HideInInspector] public Health _health;
    [HideInInspector] public SpriteRenderer _spriteRenderer;
    [HideInInspector] public Animator _animator;
    [HideInInspector] public IdleEvent _idleEvent;
    [HideInInspector] public AimWeaponEvent _aimWeaponEvent;
    [HideInInspector] public MovementByVelocityEvent _movementByVelocityEvent;
    [HideInInspector] public MovementToPositionEvent _movementToPositionEvent;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _idleEvent = GetComponent<IdleEvent>();
        _aimWeaponEvent = GetComponent<AimWeaponEvent>();
        _movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        _movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    public void Initialize(PlayerDetailsSO playerDetails)
    {
        _playerDetails = playerDetails;

        SetPlayerHealth();
    }

    void SetPlayerHealth()
    {
        _health.SetStartingHealth(_playerDetails._playerHealthAmount);
    }
}
