using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public string _id;
    public string _templateID;

    public GameObject _roomTileMapPrefab;
    public RoomNodeTypeSO _roomNodeType;

    public Vector2Int _lowerBounds;
    public Vector2Int _upperBounds;
    public Vector2Int[] _spawnPositionArray;

    public List<string> _childRoomIDList;
    public string _parentRoomID;

    public List<Doorway> _doorwayList;

    public bool _isPositioned = false;

    public InstantiatedRoom _instantiatedRoom;

    public bool _isLit = false;
    public bool _isClearedOffEnemies = false;
    public bool _isPreviousVisited = false;

    public Room()
    {
        _childRoomIDList = new();
        _doorwayList = new();
    }
}
