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

    Room _currentRoom;
    Room _previousRoom;
    PlayerDetailsSO _playerDetails;
    Player _player;

    [HideInInspector] public GameState _gameState;

    protected override void Awake()
    {
        base.Awake();

        _playerDetails = GameResources.Instance._currentPlayer._playerDetails;

        InstantiatePlayer();
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

    void InstantiatePlayer()
    {
        GameObject playerGameObject = Instantiate(_playerDetails._playerPrefab);

        playerGameObject.TryGetComponent(out _player);

        _player.Initialize(_playerDetails);
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

    public void SetCurrentRoom(Room room)
    {
        _previousRoom = _currentRoom;
        _currentRoom = room;
    }

    void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        bool dungeonBuiltSucessfully = DungeonBuilder.Instance.GenerateDungeon(_dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSucessfully)
        {
            Debug.Log("던전 빌드에 실패했습니다.");
            return;
        }

        _player.gameObject.transform.position = new Vector3((_currentRoom._lowerBounds.x + _currentRoom._upperBounds.x) / 2f,
            (_currentRoom._lowerBounds.y + _currentRoom._upperBounds.y) / 2f, 0f);

        _player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(_player.gameObject.transform.position);
    }

    public Room GetCurrentRoom()
    {
        return _currentRoom;
    }

    public Player GetPlayer()
    {
        return _player;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(_dungeonLevelList), _dungeonLevelList);  
    }
#endif
}
