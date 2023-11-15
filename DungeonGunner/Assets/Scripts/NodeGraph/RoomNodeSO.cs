using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeSO : ScriptableObject
{
    public string _id;
    public List<string> _parentRoomNodeIDList = new();
    public List<string> _childRoomNodeIDList = new();
    [HideInInspector] public RoomNodeGraphSO _roomNodeGraph;
    [HideInInspector] public RoomNodeTypeListSO _roomNodeTypeList;

    public RoomNodeTypeSO _roomNodeType;

#if UNITY_EDITOR
    [HideInInspector] public Rect _rect;
    [HideInInspector] public bool _isLeftClickDragging = false;
    [HideInInspector] public bool _isSelected = false;

    public void Initialise(Rect rect, RoomNodeGraphSO roomNodeGraph, RoomNodeTypeSO roomNodeType)
    {
        _rect = rect;
        _id = Guid.NewGuid().ToString();
        name = "RoomNode";
        _roomNodeGraph = roomNodeGraph;
        _roomNodeType = roomNodeType;

        _roomNodeTypeList = GameResources.Instance._roomNodeTypeList;
    }

    public void Draw(GUIStyle nodeStyle)
    {
        GUILayout.BeginArea(_rect, nodeStyle);

        EditorGUI.BeginChangeCheck();

        if (_parentRoomNodeIDList.Count > 0 || _roomNodeType._isEntrance)
        {
            EditorGUILayout.LabelField(_roomNodeType._roomNodeTypeName);
        }
        else
        {
            // 현재 노드의 타입의 Index
            int selected = _roomNodeTypeList._list.FindIndex(x => x == _roomNodeType);

            // 에디터에 표시되는 타입 이름 리스트를 팝업 창에 추가하고 현재 룸 노드의 Index를 선택하게 해서 팝업 창을 띄운다
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

            // 에디터에서 자식 노드로 복도 타입 혹은 보스 타입 노드가 연결돼있던 노드의 부모 노드를 삭제할 경우
            // 해당 노드의 타입을 바꿀 수 있는 문제가 발생
            // 따라서 팝업을 선택할 때 해당 노드를 복도로 변경하고자 한다면
            // 자식 노드와의 연결을 지워야 함
            if(_roomNodeTypeList._list[selected]._isCorridor && !_roomNodeTypeList._list[selection] || 
                !_roomNodeTypeList._list[selected]._isCorridor && _roomNodeTypeList._list[selection]._isCorridor ||
                !_roomNodeTypeList._list[selected]._isBossRoom && _roomNodeTypeList._list[selection]._isBossRoom)
            {
                if(_childRoomNodeIDList.Count > 0)
                {
                    for(int i = _childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        RoomNodeSO childRoomNode = _roomNodeGraph.GetRoomNode(_childRoomNodeIDList[i]);

                        if(childRoomNode != null)
                        {
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode._id);

                            childRoomNode.RemoveParentRoomNodeIDFromRoomNode(_id);
                        }
                    }
                }
            }

            _roomNodeType = _roomNodeTypeList._list[selection];
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();
    }

    // RoomNodeType에서 DisplayInNodeGraphEditor 옵션이 true인 값들을 찾아서 반환
    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[_roomNodeTypeList._list.Count];

        for(int i = 0; i < +_roomNodeTypeList._list.Count; i++)
        {
            if(_roomNodeTypeList._list[i]._displayInNodeGraphEditor)
            {
                roomArray[i] = _roomNodeTypeList._list[i]._roomNodeTypeName;
            }
        }

        return roomArray;
    }

    public void ProcessEvents(Event currentEvent)
    {
        switch(currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            default:
                break;
        }
    }

    void ProcessMouseDownEvent(Event currentEvent)
    {
        if(currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        else if(currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;

         _isSelected = !_isSelected;
        /*
        if (_isSelected)
            _isSelected = false;
        else
            _isSelected = true;
        */
    }

    void ProcessRightClickDownEvent(Event currentEvent)
    {
        _roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }

    void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDragEvent(currentEvent);
        }
    }

    void ProcessLeftClickDragEvent(Event currentEvent)
    {
        _isLeftClickDragging = true;

        DragMode(currentEvent.delta);
        GUI.changed = true;
    }

    public void DragMode(Vector2 delta)
    {
        _rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }

    void ProcessLeftClickUpEvent()
    {
        if(_isLeftClickDragging)
            _isLeftClickDragging = false;
    }

    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        if (IsChildRoomValid(childID))
        {
            _childRoomNodeIDList.Add(childID);
            return true;
        }

        return false;
    }

    public bool IsChildRoomValid(string childID)
    {
        RoomNodeSO childRoomNode = _roomNodeGraph.GetRoomNode(childID);

        // 해당 노드가 보스룸이고 이미 연결이 됐는지
        bool isConnectedBossNodeAlready = false;
        foreach(var node in _roomNodeGraph._roomNodeList)
        {
            if (node._roomNodeType._isBossRoom && node._parentRoomNodeIDList.Count > 0)
                isConnectedBossNodeAlready = true;
        }

        if (childRoomNode._roomNodeType._isBossRoom && isConnectedBossNodeAlready)
            return false;

        // 해당 노드가 아무런 노드도 아닌지
        if (childRoomNode._roomNodeType._isNone)
            return false;

        // 해당 노드를 이미 연결했는지
        if (_childRoomNodeIDList.Contains(childID)) 
            return false;

        // 자기 자신은 제외
        if (_id == childID)
            return false;

        // 부모 노드를 자식으로 연결할 수는 없음(상호연결금지)
        if (_parentRoomNodeIDList.Contains(childID))
            return false;

        // 모든 노드는 하나의 부모만 가질 수 있음
        if (childRoomNode._parentRoomNodeIDList.Count > 0)
            return false;

        // 복도 끼리는 연결 불가능
        if (childRoomNode._roomNodeType._isCorridor && _roomNodeType._isCorridor)
            return false;

        // 방과 방 또한 연결 불가능
        if (!childRoomNode._roomNodeType._isCorridor && !_roomNodeType._isCorridor)
            return false;

        // 연결할 자식 노드가 복도인데 이미 최대 자식 복도 노드를 연결했을 경우
        if (childRoomNode._roomNodeType._isCorridor && _childRoomNodeIDList.Count >= Settings._maxChildCorridors)
            return false;

        // 입구는 항상 최상위 부모 노드여야만 함
        if (childRoomNode._roomNodeType._isEntrance)
            return false;

        // 연결할 자식 노드가 복도가 아닐 경우 현재 노드는 복도이므로 연결된 자식이 없어야 함
        if (!childRoomNode._roomNodeType._isCorridor && _childRoomNodeIDList.Count > 0)
            return false;

        return true;
    }

    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        _parentRoomNodeIDList.Add(parentID);
        return true;
    }

    public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
    {
        if(_childRoomNodeIDList.Contains(childID))
        {
            _childRoomNodeIDList.Remove(childID);
            return true;
        }

        return false;
    }

    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
    {
        if (_parentRoomNodeIDList.Contains(parentID))
        {
            _parentRoomNodeIDList.Remove(parentID);
            return true;
        }

        return false;
    }
#endif
}