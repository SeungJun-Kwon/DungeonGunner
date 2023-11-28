using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/MovementDetails")]
public class MovementDetailsSO : ScriptableObject
{
    [Header("MOVEMENT DETAILS")]
    public float _minMoveSpeed = 8f;
    public float _maxMoveSpeed = 8f;

    public float _rollSpeed;
    public float _rollDistance;
    public float _rollCoolDownTime;

    public float GetMoveSpeed()
    {
        if(_minMoveSpeed == _maxMoveSpeed)
        {
            return _minMoveSpeed;
        }
        else
        {
            return Random.Range(_minMoveSpeed, _maxMoveSpeed);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(_minMoveSpeed), _minMoveSpeed, nameof(_maxMoveSpeed), _maxMoveSpeed, false);

        HelperUtilities.ValidateCheckPositiveValue(this, nameof(_rollSpeed), _rollSpeed, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(_rollDistance), _rollDistance, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(_rollCoolDownTime), _rollCoolDownTime, false);
    }
#endif
}
