using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using ResourceManagment;
using Random = UnityEngine.Random;

// Change its name


public class MapGeneration : MonoBehaviour
{

    [SerializeField] private List<GameObject> _cardPrefabs;
    [SerializeField] private List<int> _cardTypeCnt;

    private enum CardId: ushort
    {
        Empty = 0,
        Enemy = 1,
        Resource = 2,
        Tree = 3,
        Spawn = 4
    }

    private int _numberOfPlayers = 0;
    private List<List<int>> _playerIJPositions;
    // Map size in cards.
    private int _mapCardHeight;
    private int _mapCardWidth;
    private float _cardToCardDistance;
    // 0.05f recommended
    private float _fluctuatePosition;
    // 2.0f recommended
    private float _fluctuateAngle;

    private List<List<int>> _mapId;
    private List<List<Card>> _mapObject;
    public Action MapGenerated;

    private void Start()
    {
        Initialize(2, 10, 20, 0.2f, 0.05f, 2f);
    }

    public void Initialize(int numberOfPlayers, int mapCardHeight, int mapCardWidth, float cardToCardDistance, float fluctuatePosition, float fluctuateAngle)
    {
        _numberOfPlayers = numberOfPlayers;
        _mapCardWidth = mapCardWidth;
        _mapCardHeight = mapCardHeight;
        _cardToCardDistance = cardToCardDistance;
        _fluctuatePosition = fluctuatePosition;
        _fluctuateAngle = fluctuateAngle;

        ClearAll();
        SetMapId();
        InstantiateCards();
        MapGenerated?.Invoke();
    }

    private void ClearAll()
    {
        if (_playerIJPositions != null)
        {
            _playerIJPositions.Clear();
        }
        if (_mapId != null)
        {
            _mapId.Clear();
        }
        if (_mapObject != null)
        {
            for (int i = 0; i < _mapObject.Count; ++i)
            {
                for (int j = 0; j < _mapObject[i].Count; ++j)
                {
                    Destroy(_mapObject[i][j]);
                }
            }
            _mapObject.Clear();
        }
    }

    private bool CorrectCoordinates(int i, int j)
    {
        return ((0 <= i) && (i < _mapCardHeight) && (0 <= j) && (j < _mapCardWidth));
    }

    private void SetMapId()
    {
        _cardTypeCnt[0] = _mapCardHeight * _mapCardWidth;
        for (int i = 1; i < _cardTypeCnt.Count; ++i)
        {
            _cardTypeCnt[0] -= _cardTypeCnt[i];
        }

        _mapId = new List<List<int>>();
        for (int i = 0; i < _mapCardHeight; ++i)
        {
            _mapId.Add(new List<int>());
            for (int j = 0; j < _mapCardWidth; ++j)
            {
                _mapId[i].Add(-1);
            }
        }

        if (_numberOfPlayers == 1)
        {
            SetSingleplayerSpawn();
        }
        else
        {
            SetMultiplayerSpawns();
        }

        int _peacefulRadius = 2;

        for (int id = 0; id < _playerIJPositions.Count; ++id)
        {
            _mapId[_playerIJPositions[id][0]][_playerIJPositions[id][1]] = 4;
            for (int i = _playerIJPositions[id][0] - _peacefulRadius; i <= _playerIJPositions[id][0] + _peacefulRadius; ++i)
            {
                for (int j = _playerIJPositions[id][1] - _peacefulRadius; j <= _playerIJPositions[id][1] + _peacefulRadius; ++j)
                {
                    if (CorrectCoordinates(i, j) && _mapId[i][j] == -1)
                    {
                        _mapId[i][j] = 0;
                    }
                }
            }
        }

        for (int cardId = 1; cardId < _cardTypeCnt.Count; ++cardId)
        {
            for (int cnt = 0; cnt < _cardTypeCnt[cardId]; ++cnt)
            {
                int i = Random.Range(0, _mapCardHeight);
                int j = Random.Range(0, _mapCardWidth);
                while (_mapId[i][j] != -1)
                {
                    i = Random.Range(0, _mapCardHeight);
                    j = Random.Range(0, _mapCardWidth);
                }
                _mapId[i][j] = cardId;
            }
        }
        for (int i = 0; i < _mapCardHeight; ++i) {
            for (int j = 0; j < _mapCardWidth; ++j) {
                if (_mapId[i][j] == -1) {
                    _mapId[i][j] = 0;
                }
            }
        }
    }

    private void SetSingleplayerSpawn() {
        _playerIJPositions = new List<List<int>>();
        int _spawnPosI = Random.Range(0, _mapCardHeight);
        int _spawnPosJ = Random.Range(0, _mapCardWidth);
        _playerIJPositions.Add(new List<int>());
        _playerIJPositions[0].Add(_spawnPosI);
        _playerIJPositions[0].Add(_spawnPosJ);
    }

    private void SetMultiplayerSpawns() {
        _playerIJPositions = new List<List<int>>();
        int _firstSpawnPosI, _firstSpawnPosJ, _secondSpawnPosI, _secondSpawnPosJ;

        int r;
        //*magic*
        r = Random.Range(0, 2);
        _firstSpawnPosI = Random.Range(0, _mapCardHeight);
        _firstSpawnPosJ = Random.Range(0, _mapCardWidth);
        _firstSpawnPosI *= r;
        _firstSpawnPosJ *= (1 - r);
        _secondSpawnPosI = _mapCardHeight - _firstSpawnPosI - 1;
        _secondSpawnPosJ = _mapCardWidth - _firstSpawnPosJ - 1;
        r = Random.Range(0, 2);
        if (r == 1)
        {
            r = _firstSpawnPosI;
            _firstSpawnPosI = _secondSpawnPosI;
            _secondSpawnPosI = r;
            r = _firstSpawnPosJ;
            _firstSpawnPosJ = _secondSpawnPosJ;
            _secondSpawnPosJ = r;
        }


        _playerIJPositions.Add(new List<int>());
        _playerIJPositions[0].Add(_firstSpawnPosI);
        _playerIJPositions[0].Add(_firstSpawnPosJ);
        _playerIJPositions.Add(new List<int>());
        _playerIJPositions[1].Add(_secondSpawnPosI);
        _playerIJPositions[1].Add(_secondSpawnPosJ);
    }

    void InstantiateCards()
    {
        _mapObject = new List<List<Card>>();
        for (int i = 0; i < _mapCardHeight; ++i)
        {
            _mapObject.Add(new List<Card>());
            for (int j = 0; j < _mapCardWidth; ++j)
            {
                float _mapUnityWidth  = (_mapCardWidth  - 1) * Constants.CARD_WIDTH  + (_mapCardWidth  - 1) * _cardToCardDistance;
                float _mapUnityHeight = (_mapCardHeight - 1) * Constants.CARD_HEIGHT + (_mapCardHeight - 1) * _cardToCardDistance;
                float _posI = - _mapUnityWidth  / 2 + (_mapUnityWidth  / (_mapCardWidth  - 1)) * j;
                float _posJ = - _mapUnityHeight / 2 + (_mapUnityHeight / (_mapCardHeight - 1)) * i;

                float _deltaI = Random.Range(-_fluctuatePosition, _fluctuatePosition);
                float _deltaJ = Random.Range(-_fluctuatePosition, _fluctuatePosition);

                GameObject _newCardObject = Instantiate(_cardPrefabs[_mapId[i][j]], new Vector3(_posI + _deltaI, _posJ + _deltaJ, 0f), Quaternion.identity);
                _newCardObject.transform.eulerAngles = new Vector3(0, 0, Random.Range(-2, 2));
                _newCardObject.transform.parent = gameObject.transform;

                //Spawn();
                Card _newCard = _newCardObject.GetComponent<Card>();
                if (_newCard is EnemyCard)
                {
                    int _level = Random.Range(1, 3);
                    ((EnemyCard)_newCard).Initialize(_level, 50,
                        new Dictionary<ResourceType, int>() {{ResourceType.Food, 10 * _level}, {ResourceType.Money, 100 * _level}});
                }
                else if (_newCard is ResourceCard)
                {
                    var resourceTypes = Enum.GetValues (typeof (ResourceType));
                    var resourceType = (ResourceType)resourceTypes.GetValue(Random.Range(0, resourceTypes.Length));
                    switch (resourceType)
                    {
                        case ResourceType.Food:
                            _newCardObject.transform.GetChild(4).gameObject.SetActive(true);
                            ((ResourceCard)_newCard).Initialize(ResourceType.Food, Random.Range(5, 20));
                            break;
                        case ResourceType.Energy:
                            _newCardObject.transform.GetChild(3).gameObject.SetActive(true);
                            ((ResourceCard)_newCard).Initialize(ResourceType.Energy, Random.Range(1, 5));
                            break;
                        case ResourceType.Money:
                            _newCardObject.transform.GetChild(2).gameObject.SetActive(true);
                            ((ResourceCard)_newCard).Initialize(ResourceType.Money, Random.Range(180, 300));
                            break;
                        default:
                            _newCardObject.transform.GetChild(2).gameObject.SetActive(true);
                            ((ResourceCard)_newCard).Initialize(ResourceType.Money, Random.Range(180, 300));
                            break;
                    }
                }
                BodyInformation _body = _newCardObject.transform.GetChild(0).GetComponent<BodyInformation>();
                _body.id = i * _mapCardWidth + j;
                _mapObject[i].Add(_newCard);
            }
        }
    }

    public List<List<Card>> GetMap()
    {
        return _mapObject;
    }

    public int GetMapCardWidth()
    {
        return _mapCardWidth;
    }

    public int GetMapCardHeight()
    {
        return _mapCardHeight;
    }
}
