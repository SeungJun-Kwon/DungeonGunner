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
[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerDetailsSO _playerDetails;
    [HideInInspector] public Health _health;
    [HideInInspector] public SpriteRenderer _spriteRenderer;
    [HideInInspector] public Animator _animator;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
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
