using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehavior<DungeonBuilder>
{
    public Dictionary<string, Room> _dungeonBuilderRoomDictionary = new();

    Dictionary<string, RoomTemplateSO> _roomTemplateDictionary = new();
    List<RoomTemplateSO> _roomTemplateList = null;
    RoomNodeTypeListSO _roomNodeTypeList;

    bool _dungeonBuildSuccessful;

    protected override void Awake()
    {
        base.Awake();

        LoadRoomTypeList();

        GameResources.Instance._dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    void LoadRoomTypeList()
    {
        _roomNodeTypeList = GameResources.Instance._roomNodeTypeList;
    }

    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        _roomTemplateList = currentDungeonLevel._roomTemplateList;

        LoadRoomTemplatesIntoDictionary();

        _dungeonBuildSuccessful = false;

        int dungeonBuildAttempts = 0;

        while(!_dungeonBuildSuccessful && dungeonBuildAttempts < Settings._maxDungeonBuildAttemps)
        {
            dungeonBuildAttempts++;

            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel._roomNodeGraphList);

            int dungeonRebuildAttemptsForNodeGraph = 0;
            _dungeonBuildSuccessful = false;

            while(!_dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings._maxDungeonRebuildAttempsForRoomGraph)
            {
                ClearDungeon();

                dungeonRebuildAttemptsForNodeGraph++;

                _dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
            }

            if (_dungeonBuildSuccessful)
            {
                InstantiateRoomGameObjects();
            }
        }

        return _dungeonBuildSuccessful;
    }

    void LoadRoomTemplatesIntoDictionary()
    {
        _roomTemplateDictionary.Clear();

        foreach (var roomTemplate in _roomTemplateList)
        {
            if (!_roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                _roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else
            {
                Debug.Log($"Room Template Key가 이미 {_roomTemplateList}에 존재합니다");
            }
        }
    }

    bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {
        Queue<RoomNodeSO> openRoomNodeQueue = new();

        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(_roomNodeTypeList._list.Find(x => x._isEntrance));

        if(entranceNode != null)
        {
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            Debug.Log($"{roomNodeGraph.name}에서 입구 노드를 찾을 수 없습니다.");
        }

        bool noRoomOverlaps = true;

        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        if(openRoomNodeQueue.Count == 0 && noRoomOverlaps)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
    {
        while(openRoomNodeQueue.Count > 0 && noRoomOverlaps == true)
        {
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            foreach(var childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            if (roomNode._roomNodeType._isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode._roomNodeType);

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                room._isPositioned = true;

                _dungeonBuilderRoomDictionary.Add(room._id, room);
            }
            else
            {
                Room parentRoom = _dungeonBuilderRoomDictionary[roomNode._parentRoomNodeIDList[0]];

                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }
        }

        return noRoomOverlaps;
    }

    bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
    {
        bool roomOverlaps = true;

        while (roomOverlaps)
        {
            List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableParentDoorways(parentRoom._doorwayList).ToList();

            if(unconnectedAvailableParentDoorways.Count == 0)
            {
                return false;
            }

            Doorway doorwayParent = unconnectedAvailableParentDoorways[Random.Range(0, unconnectedAvailableParentDoorways.Count)];

            RoomTemplateSO roomTemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);
            
            if(PlaceTheRoom(parentRoom, doorwayParent, room))
            {
                roomOverlaps = false;

                room._isPositioned = true;

                _dungeonBuilderRoomDictionary.Add(room._id, room);
            }
            else
            {
                roomOverlaps = true;
            }
        }

        return true;
    }

    // 만약 만들려는 방이 복도라면 부모의 출입구 방향을 알아야 하므로 나눠준다
    RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomTemplate = null;

        if (roomNode._roomNodeType._isCorridor)
        {
            switch (doorwayParent.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    roomTemplate = GetRandomRoomTemplate(_roomNodeTypeList._list.Find(x => x._isCorridorNS));
                    break;

                case Orientation.east:
                case Orientation.west:
                    roomTemplate = GetRandomRoomTemplate(_roomNodeTypeList._list.Find(x => x._isCorridorEW));
                    break;

                case Orientation.none:
                default:
                    break;
            }
        }
        else
        {
            roomTemplate = GetRandomRoomTemplate(roomNode._roomNodeType);
        }

        return roomTemplate;
    }

    bool PlaceTheRoom(Room parentRoom, Doorway parentDoorway, Room room)
    {
        // 연결된 통로의 반대방향 통로를 구한다 : 부모 Room에서 북쪽 방향으로 연결된 통로는
        // 자식 Room의 남쪽 방향 통로로 이어지기 때문
        Doorway doorway = GetOppositeDoorway(parentDoorway, room._doorwayList);

        if(doorway == null)
        {
            doorway.isUnavailable = true;

            return false;
        }

        Vector2Int parentDoorwayPosition = parentRoom._lowerBounds + parentDoorway.position - parentRoom._templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        switch (doorway.orientation)
        {
            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;
            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;
            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
                break;
            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;
            default:
                break;
        }

        room._lowerBounds = parentDoorwayPosition + adjustment + room._templateLowerBounds - doorway.position;
        room._upperBounds = room._lowerBounds + room._templateUpperBounds - room._templateLowerBounds;

        Room overlappingRoom = CheckForRoomOverlap(room);

        if(overlappingRoom == null)
        {
            parentDoorway.isConnected = true;
            parentDoorway.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isUnavailable = true;

            return true;
        }
        else
        {
            parentDoorway.isUnavailable = true;

            return false;
        }
    }

    Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList)
    {
        foreach(var doorway in doorwayList)
        {
            if(parentDoorway.orientation == Orientation.east && doorway.orientation == Orientation.west)
            {
                return doorway;
            }
            else if (parentDoorway.orientation == Orientation.west && doorway.orientation == Orientation.east)
            {
                return doorway;
            }
            else if (parentDoorway.orientation == Orientation.north && doorway.orientation == Orientation.south)
            {
                return doorway;
            }
            else if (parentDoorway.orientation == Orientation.south && doorway.orientation == Orientation.north)
            {
                return doorway;
            }
        }

        return null;
    }

    Room CheckForRoomOverlap(Room room)
    {
        foreach (var keyvaluepair in _dungeonBuilderRoomDictionary)
        {
            Room value = keyvaluepair.Value;

            if (room._id == value._id || !value._isPositioned)
                continue;

            if (IsOverlappingRoom(room, value))
            {
                return room;
            }
        }

        return null;
    }

    bool IsOverlappingRoom(Room room1, Room room2)
    {
        bool isOverlappingX = IsOverlappingInterval(room1._lowerBounds.x, room1._upperBounds.x, room2._lowerBounds.x, room2._upperBounds.x);
        bool isOverlappingY = IsOverlappingInterval(room1._lowerBounds.y, room1._upperBounds.y, room2._lowerBounds.y, room2._upperBounds.y);

        return isOverlappingX && isOverlappingY;
    }

    bool IsOverlappingInterval(int imin1, int imax1, int imin2, int imax2)
    {
        if(Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        Room room = new();

        room._templateID = roomTemplate.guid;
        room._id = roomNode._id;
        room._roomTileMapPrefab = roomTemplate.prefab;
        room._roomNodeType = roomTemplate.roomNodeType;
        room._upperBounds = roomTemplate.upperBounds;
        room._lowerBounds = roomTemplate.lowerBounds;
        room._spawnPositionArray = roomTemplate.spawnPositionArray;
        room._templateUpperBounds = roomTemplate.upperBounds;
        room._templateLowerBounds = roomTemplate.lowerBounds;

        room._childRoomIDList = CopyStringList(roomNode._childRoomNodeIDList);
        room._doorwayList = CopyDoorwayList(roomTemplate.doorwayList);

        if(roomNode._parentRoomNodeIDList.Count == 0)
        {
            room._parentRoomID = "";
            room._isPreviousVisited = true;
        }
        else
        {
            room._parentRoomID = roomNode._parentRoomNodeIDList[0];
        }

        return room;
    }

    RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
    {
        if(roomNodeGraphList.Count > 0)
        {
            return roomNodeGraphList[Random.Range(0, roomNodeGraphList.Count)];
        }
        else
        {
            Debug.Log("Room Node Graph List가 비었습니다.");
            return null;
        }
    }

    RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new();

        foreach(var roomTemplate in _roomTemplateList)
        {
            if(roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        if (matchingRoomTemplateList.Count == 0)
        {
            Debug.Log($"{_roomTemplateList}에 {roomNodeType}과 일치하는 타입의 방이 없습니다.");
            return null;
        }

        return matchingRoomTemplateList[Random.Range(0, matchingRoomTemplateList.Count)];
    }

    IEnumerable<Doorway> GetUnconnectedAvailableParentDoorways(List<Doorway> roomDoorwayList)
    {
        foreach(var doorway in roomDoorwayList)
        {
            if (!doorway.isConnected && !doorway.isUnavailable)
                yield return doorway;
        }
    }

    // List를 그냥 참조할 경우 값 변경에 있어서 자유롭지 못하다
    // 따라서 새로운 List를 만들어 할당하면 자유롭게 변경이 가능하다
    List<string> CopyStringList(List<string> oldStringList)
    {
        List<string> newStringList = new();

        foreach (var stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }

        return newStringList;
    }

    List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
    {
        List<Doorway> newDoorwayList = new();

        foreach(var doorway in oldDoorwayList)
        {
            Doorway newDoorway = new();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;

            newDoorwayList.Add(newDoorway);
        }

        return newDoorwayList;
    }

    void InstantiateRoomGameObjects()
    {
        foreach(var keyvaluepair in _dungeonBuilderRoomDictionary)
        {
            Room room = keyvaluepair.Value;

            Vector3 roomPosition = new Vector3(room._lowerBounds.x - room._templateLowerBounds.x, room._lowerBounds.y - room._templateLowerBounds.y, 0f);

            GameObject roomGameObject = Instantiate(room._roomTileMapPrefab, roomPosition, Quaternion.identity, transform);

            InstantiatedRoom instantiatedRoom = roomGameObject.GetComponentInChildren<InstantiatedRoom>();

            instantiatedRoom._room = room;

            instantiatedRoom.Initialise(roomGameObject);

            room._instantiatedRoom = instantiatedRoom;
        }
    }

    public RoomTemplateSO GetRoomTemplateByRoomTemplateID(string roomTemplateID)
    {
        if(_roomTemplateDictionary.TryGetValue(roomTemplateID, out var roomTemplate))
        {
            return roomTemplate;
        }
        else
        {
            return null;
        }
    }

    public Room GetRoomByRoomID(string roomID)
    {
        if (_dungeonBuilderRoomDictionary.TryGetValue(roomID, out var room))
        {
            return room;
        }
        else
        {
            return null;
        }
    }

    void ClearDungeon()
    {
        if(_dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach(var keyvaluepair in _dungeonBuilderRoomDictionary)
            {
                Room room = keyvaluepair.Value;

                if(room._instantiatedRoom != null)
                {
                    Destroy(room._instantiatedRoom.gameObject);
                }
            }

            _dungeonBuilderRoomDictionary.Clear();
        }
    }
}
