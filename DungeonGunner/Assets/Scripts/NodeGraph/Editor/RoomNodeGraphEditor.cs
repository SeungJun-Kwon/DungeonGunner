using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class RoomNodeGraphEditor : EditorWindow
{
    GUIStyle _roomNodeStyle;
    GUIStyle _roomNodeSelectedStyle;

    static RoomNodeGraphSO _currentRoomNodeGraph;

    RoomNodeTypeListSO _roomNodeTypeList;
    RoomNodeSO _currentRoomNode = null;

    Vector2 _graphOffset;
    Vector2 _graphDrag;

    const float _nodeWidth = 160f;
    const float _nodeHeight = 75f;
    const int _nodePadding = 25;
    const int _nodeBorder = 12;

    const float _connectingLineWidth = 3f;
    const float _connectingLineArrowSize = 6f;

    const float _gridLarge = 100f;
    const float _gridSmall = 25f;

    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("RoomNodeGraphEditor");
    }

    private void OnEnable()
    {
        // 인스펙터 선택이 바뀌었을 때 실행되는 델리게이트
        Selection.selectionChanged += InspectorSelectionChanged;

        _roomNodeStyle = new();
        _roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        _roomNodeStyle.normal.textColor = Color.white;
        _roomNodeStyle.padding = new RectOffset(_nodePadding, _nodePadding, _nodePadding, _nodePadding);
        _roomNodeStyle.border = new RectOffset(_nodeBorder, _nodeBorder, _nodeBorder, _nodeBorder);

        _roomNodeSelectedStyle = new();
        _roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        _roomNodeSelectedStyle.normal.textColor = Color.white;
        _roomNodeSelectedStyle.padding = new RectOffset(_nodePadding, _nodePadding, _nodePadding, _nodePadding);
        _roomNodeSelectedStyle.border = new RectOffset(_nodeBorder, _nodeBorder, _nodeBorder, _nodeBorder);

        _roomNodeTypeList = GameResources.Instance._roomNodeTypeList;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    // Room Node Graph 스크립터블 오브젝트를 더블클릭하면 에디터 창이 열리게 하는 함수
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if(roomNodeGraph != null)
        {
            OpenWindow();

            _currentRoomNodeGraph = roomNodeGraph;

            return true;
        }

        return false;
    }

    private void OnGUI()
    {
        if(_currentRoomNodeGraph != null)
        {
            DrawBackgroundGrid(_gridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(_gridLarge, 0.3f, Color.gray);

            ProcessEvent(Event.current);

            // Line을 먼저 그리는 이유 : RoomNode가 Line 앞에 나타나게 하기 위해
            DrawDraggedLine();

            DrawRoomConnections();

            DrawRoomNode();
        }

        if (GUI.changed)
            Repaint();
    }

    void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        _graphOffset += _graphDrag * 0.5f;

        Vector3 gridOffset = new Vector3(_graphOffset.x % gridSize, _graphOffset.y % gridSize, 0);

        for(int i = 0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset, new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
        }

        for(int i = 0; i < horizontalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * i, 0) + gridOffset, new Vector3(position.width + gridSize, gridSize * i, 0f) + gridOffset);
        }

        Handles.color = Color.white;
    }

    void ProcessEvent(Event currentEvent)
    {
        _graphDrag = Vector2.zero;

        if (_currentRoomNode == null || !_currentRoomNode._isLeftClickDragging)
            _currentRoomNode = IsMouseOverRoomNode(currentEvent);

        if (_currentRoomNode == null || _currentRoomNodeGraph._roomNodeToDrawLineFrom != null)
            ProcessRoomNodeGraphEvent(currentEvent);
        else
            _currentRoomNode.ProcessEvents(currentEvent);
    }

    void ProcessRoomNodeGraphEvent(Event currentEvent)
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
        // 0 : 왼쪽 클릭, 1 : 오른쪽 클릭
        if(currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
        else if(currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
    }

    void ProcessMouseDragEvent(Event currentEvent)
    {
        if(currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent.delta);
        }
        else  if(currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
    }

    void ProcessMouseUpEvent(Event currentEvent)
    {
        if(currentEvent.button == 1 && _currentRoomNodeGraph._roomNodeToDrawLineFrom != null)
        {
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);

            if(roomNode != null)
            {
                if(_currentRoomNodeGraph._roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode._id))
                {
                    roomNode.AddParentRoomNodeIDToRoomNode(_currentRoomNodeGraph._roomNodeToDrawLineFrom._id);
                }
            }

            ClearLineDrag();
        }
    }

    void ProcessLeftMouseDragEvent(Vector2 delta)
    {
        _graphDrag = delta;

        for (int i = 0; i < _currentRoomNodeGraph._roomNodeList.Count; i++)
        {
            _currentRoomNodeGraph._roomNodeList[i].DragMode(_graphDrag);
        }

        GUI.changed = true;
    }

    void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if(_currentRoomNodeGraph._roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    public void DragConnectingLine(Vector2 delta)
    {
        _currentRoomNodeGraph._linePosition += delta;
    }

    void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new();

        // 메소드 : 메뉴 항목을 클릭할 때 발생
        // 매개변수 : 함수의 매개변수로 전달
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);

        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Room Node"), false, SelectAllRoomNodes);

        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);

        menu.ShowAsContext();
    }

    RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for(int i = _currentRoomNodeGraph._roomNodeList.Count - 1; i >= 0; i--)
        {
            if (_currentRoomNodeGraph._roomNodeList[i]._rect.Contains(currentEvent.mousePosition))
            {
                return _currentRoomNodeGraph._roomNodeList[i];
            }
        }

        return null;
    }

    void CreateRoomNode(object mousePositionObject)
    {
        if(_currentRoomNodeGraph._roomNodeList.Count == 0)
        {
            Vector2 mousePosition = (Vector2)mousePositionObject;

            CreateRoomNode(new Vector2(mousePosition.x - 100f, mousePosition.y), _roomNodeTypeList._list.Find(x => x._isEntrance));
        }

        CreateRoomNode(mousePositionObject, _roomNodeTypeList._list.Find(x => x._isNone));
    }

    void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        _currentRoomNodeGraph._roomNodeList.Add(roomNode);

        roomNode.Initialise(new Rect(mousePosition, new Vector2(_nodeWidth, _nodeHeight)), _currentRoomNodeGraph, roomNodeType);

        // 스크립터블 오브젝트 에셋에 오브젝트를 추가하고 저장
        AssetDatabase.AddObjectToAsset(roomNode, _currentRoomNodeGraph);
        AssetDatabase.SaveAssets();

        _currentRoomNodeGraph.OnValidate();
    }

    void DeleteSelectedRoomNodeLinks()
    {
        // 1. 현재 룸 노드 중 선택된 룸 노드가 있다면
        // 2. 그리고 해당 노드의 자식 노드 중 선택된 노드가 있다면
        // 3. 서로의 링크를 끊는다(부모자식 종속관계 제거)
        foreach(var roomNode in _currentRoomNodeGraph._roomNodeList)
        {
            if(roomNode._isSelected && roomNode._childRoomNodeIDList.Count > 0)
            {
                for(int i = roomNode._childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    RoomNodeSO childRoomNode = _currentRoomNodeGraph.GetRoomNode(roomNode._childRoomNodeIDList[i]);

                    if(childRoomNode != null && childRoomNode._isSelected)
                    {
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode._id);

                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode._id);
                    }
                }
            }
        }

        ClearAllSelectedRoomNodes();
    }

    void DeleteSelectedRoomNodes()
    {
        // 1. 선택된 룸 노드 중 입구 노드가 아닌 노드를 제거 큐에 추가한다
        // 2. 해당 노드의 부모자식 종속관계를 제거한다
        // 3. 이후 큐에서 하나씩 Dequeue하면서 데이터를 정리한다

        Queue<RoomNodeSO> roomNodeDeleteQueue = new();

        foreach (var roomNode in _currentRoomNodeGraph._roomNodeList)
        {
            if (roomNode._isSelected && !roomNode._roomNodeType._isEntrance)
            {
                roomNodeDeleteQueue.Enqueue(roomNode);

                foreach (var childID in roomNode._childRoomNodeIDList)
                {
                    RoomNodeSO childRoomNode = _currentRoomNodeGraph.GetRoomNode(childID);

                    if (childRoomNode != null)
                    {
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode._id);
                    }
                }

                foreach (var parentID in roomNode._parentRoomNodeIDList)
                {
                    RoomNodeSO parentRoomNode = _currentRoomNodeGraph.GetRoomNode(parentID);

                    if (parentRoomNode != null)
                    {
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode._id);
                    }
                }
            }
        }

        while (roomNodeDeleteQueue.Count > 0)
        {
            RoomNodeSO roomNodeToDelete = roomNodeDeleteQueue.Dequeue();

            _currentRoomNodeGraph._roomNodeDic.Remove(roomNodeToDelete._id);

            _currentRoomNodeGraph._roomNodeList.Remove(roomNodeToDelete);

            DestroyImmediate(roomNodeToDelete, true);

            AssetDatabase.SaveAssets();
        }
    }

    void SelectAllRoomNodes()
    {
        foreach (var node in _currentRoomNodeGraph._roomNodeList)
            node._isSelected = true;

        GUI.changed = true;
    }

    public void ClearAllSelectedRoomNodes()
    {
        foreach(var node in _currentRoomNodeGraph._roomNodeList)
        {
            if(node._isSelected)
            {
                node._isSelected = false;

                GUI.changed = true;
            }
        }
    }

    void DrawDraggedLine()
    {
        if(_currentRoomNodeGraph._linePosition != Vector2.zero)
        {
            Handles.DrawBezier(_currentRoomNodeGraph._roomNodeToDrawLineFrom._rect.center, _currentRoomNodeGraph._linePosition,
                _currentRoomNodeGraph._roomNodeToDrawLineFrom._rect.center, _currentRoomNodeGraph._linePosition, Color.white, null, _connectingLineWidth);
        }
    }

    void ClearLineDrag()
    {
        _currentRoomNodeGraph._roomNodeToDrawLineFrom = null;
        _currentRoomNodeGraph._linePosition = Vector2.zero;
        GUI.changed = true;
    }

    void DrawRoomNode()
    {
        foreach(RoomNodeSO roomNode in _currentRoomNodeGraph._roomNodeList)
        {
            if (roomNode._isSelected)
                roomNode.Draw(_roomNodeSelectedStyle);
            else
                roomNode.Draw(_roomNodeStyle);
        }

        GUI.changed = true;
    }

    void DrawRoomConnections()
    {
        foreach(RoomNodeSO node in _currentRoomNodeGraph._roomNodeList)
        {
            if(node._childRoomNodeIDList.Count > 0)
            {
                foreach(string childID in node._childRoomNodeIDList)
                {
                    if(_currentRoomNodeGraph._roomNodeDic.ContainsKey(childID))
                    {
                        DrawConnectionLine(node, _currentRoomNodeGraph._roomNodeDic[childID]);

                        GUI.changed = true;
                    }
                }
            }
        }
    }

    void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        Vector2 startPosition = parentRoomNode._rect.center;
        Vector2 endPosition = childRoomNode._rect.center;
        Vector2 midPosition = (startPosition + endPosition) / 2f;
        Vector2 direction = endPosition - startPosition;

        // 선의 수직벡터를 구하고 중간 점으로부터 일정 위치에 점을 찍는다 -> 화살표의 끝부분이 될 점
        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * _connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * _connectingLineArrowSize;

        Vector2 arrowHeadPoint = midPosition + direction.normalized * _connectingLineWidth;

        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, _connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, _connectingLineWidth);

        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, _connectingLineWidth);

        GUI.changed = true;
    }

    void InspectorSelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

        if(roomNodeGraph != null)
        {
            _currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }
}
