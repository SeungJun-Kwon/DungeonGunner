using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    public static GameResources _instance;
    public static GameResources Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = Resources.Load<GameResources>("GameResources");
            }

            return _instance;
        }
    }

    [Space(10)]
    [Header("Dungeon")]
    public RoomNodeTypeListSO _roomNodeTypeList;

    [Space(10)]
    [Header("PLAYER")]
    public CurrentPlayerSO _currentPlayer;

    [Space(10)]
    [Header("Materials")]
    public Material _dimmedMaterial;
}
