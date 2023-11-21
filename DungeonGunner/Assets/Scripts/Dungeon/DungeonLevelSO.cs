using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    [Space(10)]
    [Header("기본 레벨 정보")]
    public string _levelName;

    [Space(10)]
    [Header("Room Templates")]
    public List<RoomTemplateSO> _roomTemplateList;

    [Space(10)]
    [Header("Room Node Graphs")]
    public List<RoomNodeGraphSO> _roomNodeGraphList;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(_levelName), _levelName);

        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(_roomTemplateList), _roomTemplateList))
            return;

        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(_roomNodeGraphList), _roomNodeGraphList))
            return;

        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;

        foreach(var roomTemplateSO in _roomTemplateList)
        {
            if (roomTemplateSO == null)
                return;

            if (roomTemplateSO.roomNodeType._isCorridorEW)
                isEWCorridor = true;

            if (roomTemplateSO.roomNodeType._isCorridorNS)
                isNSCorridor = true;

            if (roomTemplateSO.roomNodeType._isEntrance)
                isEntrance = true;
        }

        if (!isEWCorridor)
            Debug.Log($"{this.name}에 동서방향 복도가 없습니다");

        if(!isNSCorridor)
            Debug.Log($"{this.name}에 남북방향 복도가 없습니다");

        if(!isEntrance)
            Debug.Log($"{this.name}에 입구가 없습니다");

        foreach(var roomNodeGraph in _roomNodeGraphList)
        {
            if (roomNodeGraph == null)
                return;

            foreach(var roomNodeSO in roomNodeGraph._roomNodeList)
            {
                if (roomNodeSO == null)
                    continue;

                if (roomNodeSO._roomNodeType._isEntrance || roomNodeSO._roomNodeType._isCorridorEW || roomNodeSO._roomNodeType._isCorridorNS ||
                    roomNodeSO._roomNodeType._isCorridor || roomNodeSO._roomNodeType._isNone)
                    continue;

                bool isRoomNodeTypeFound = false;

                foreach (var roomTemplateSO in _roomTemplateList)
                {
                    if (roomTemplateSO == null)
                        continue;

                    if (roomTemplateSO.roomNodeType == roomNodeSO._roomNodeType)
                    {
                        isRoomNodeTypeFound = true;
                        break;
                    }
                }

                if (!isRoomNodeTypeFound)
                    Debug.Log($"{this.name} : {roomNodeGraph.name}에 대한 유형의 Room Template를 찾을 수 없습니다.");
            }
        }
    }
#endif
}
