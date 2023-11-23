using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehavior<GameManager>
{
    [Space(10)]
    [Header("Dungeon Levels")]
    [SerializeField] List<DungeonLevelSO> _dungeonLevelList;

    [SerializeField] int _currentDungeonLevelListIndex = 0;

    [HideInInspector] public GameState _gameState;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        _gameState = GameState.gameStarted;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _gameState = GameState.gameStarted;
        }

        HandleGameState();
    }

    void HandleGameState()
    {
        switch (_gameState)
        {
            case GameState.gameStarted:
                PlayDungeonLevel(_currentDungeonLevelListIndex);
                _gameState = GameState.playingLevel;
                break;
        }
    }

    void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        bool dungeonBuiltSucessfully = DungeonBuilder.Instance.GenerateDungeon(_dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSucessfully)
        {
            Debug.Log("던전 빌드에 실패했습니다.");
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(_dungeonLevelList), _dungeonLevelList);  
    }
#endif
}
