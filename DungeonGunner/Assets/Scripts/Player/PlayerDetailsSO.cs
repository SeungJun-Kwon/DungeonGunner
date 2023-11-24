using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails", menuName = "Scriptable Objects/Player/Player Details")]
public class PlayerDetailsSO : ScriptableObject
{
    [Header("PLAYER BASE DETAILS")]
    public string _playerCharacterName;
    public GameObject _playerPrefab;
    public RuntimeAnimatorController _runtimeAnimatorController;

    [Header("HEALTH")]
    public int _playerHealthAmount;

    [Header("OTHER")]
    public Sprite _playerMiniMapIcon;
    public Sprite _playerHandSprite;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(_playerCharacterName), _playerCharacterName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(_playerPrefab), _playerPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(_playerHealthAmount), _playerHealthAmount, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(_playerMiniMapIcon), _playerMiniMapIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(_playerHandSprite), _playerHandSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(_runtimeAnimatorController), _runtimeAnimatorController);
    }
#endif
}
