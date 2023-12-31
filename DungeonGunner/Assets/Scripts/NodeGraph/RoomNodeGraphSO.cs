using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO _roomNodeTypeList;
    [HideInInspector] public List<RoomNodeSO> _roomNodeList = new();
    [HideInInspector] public Dictionary<string, RoomNodeSO> _roomNodeDic = new();

    private void Awake()
    {
        LoadRoomNodeDictionary();
    }

    void LoadRoomNodeDictionary()
    {
        _roomNodeDic.Clear();

        foreach(var node in _roomNodeList)
        {
            _roomNodeDic[node._id] = node;
        }
    }

    public RoomNodeSO GetRoomNode(RoomNodeTypeSO roomNodeType)
    {
        foreach(var roomNode in _roomNodeList)
        {
            if(roomNode._roomNodeType == roomNodeType)
            {
                return roomNode;
            }
        }

        return null;
    }

    public RoomNodeSO GetRoomNode(string roomNodeID)
    {
        if (_roomNodeDic.TryGetValue(roomNodeID, out var node))
            return node;

        return null;
    }

    public IEnumerable<RoomNodeSO> GetChildRoomNodes(RoomNodeSO parentRoomNode)
    {
        foreach(string childRoomNodeID in parentRoomNode._childRoomNodeIDList)
        {
            yield return GetRoomNode(childRoomNodeID);
        }
    }

#if UNITY_EDITOR

    [HideInInspector] public RoomNodeSO _roomNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 _linePosition;

    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }

    public void SetNodeToDrawConnectionLineFrom(RoomNodeSO node, Vector2 position)
    {
        _roomNodeToDrawLineFrom = node;
        _linePosition = position;
    }

#endif
}
