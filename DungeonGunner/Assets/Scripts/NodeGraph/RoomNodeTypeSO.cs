using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string _roomNodeTypeName;

    [Space(10)]
    public bool _displayInNodeGraphEditor = true;
    [Space(10)]
    public bool _isCorridor;
    [Space(10)]
    public bool _isCorridorNS;
    [Space(10)]
    public bool _isCorridorEW;
    [Space(10)]
    public bool _isEntrance;
    [Space(10)]
    public bool _isBossRoom;
    [Space(10)]
    public bool _isNone;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(_roomNodeTypeName), _roomNodeTypeName);
    }
#endif
}