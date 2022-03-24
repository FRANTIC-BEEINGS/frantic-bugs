using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using Photon.Pun;
using ResourceManagment;
using Random = UnityEngine.Random;

public class MapGeneration : MonoBehaviour
{
    [SerializeField] private List<GameObject> _cardPrefabs;

    private enum CardId: ushort
    {
        Empty = 0,
        Enemy = 1,
        // 11 - 1 lvl enemy
        // 21 - 2 lvl enemy
        // 31 - 3 lvl enemy
        Resource = 2,
        Tree = 3,
        Spawn = 4
    }

    private int _numberOfPlayers;
    public List<List<int>> _playerIJPositions;
    // Map size in cards.
    private int _mapCardWidth;
    private int _mapCardHeight;
    private float _mapUnityWidth;
    private float _mapUnityHeight;
    private float _cardToCardDistance;
    // 0.05f recommended
    private float _fluctuatePosition;
    // 2.0f recommended
    private float _fluctuateAngle;
    // 3 recommended
    private int _fluctuateSpawn;
    // 3 recommended
    private int _peacefulRadius;

    private int _numberOfTrees;
    private int _numberOfAdditionalContent;

    private List<int> _additionalContentPercentage;

    private List<int> _numberOfEnemy;

    private List<List<int>> _mapId;
    private List<List<Card>> _mapCard;
    private List<List<GameObject>> _mapObject;
    public Action MapGenerated;

    private List<Tuple<int, Tuple<int, int>>> _nearestCellsPlayer1;
    private List<Tuple<int, Tuple<int, int>>> _nearestCellsPlayer2;
    private int _resourceAroundTreeRadius;

    private void Start()
    {
        //Initialize();
        //Initialize(Random.Range(0, 3), Random.Range(5, 10), Random.Range(5, 20), Random.Range(0.0f, 0.4f), Random.Range(0.0f, 0.1f), Random.Range(0f, 4f), Random.Range(0, 3));
    }

    // public void Initialize()
    // {
    //     Initialize(1, 10, 20, 0.2f, 0.05f, 2f, 3, 3);
    // }

    public void Initialize(int numberOfPlayers=2, int mapCardHeight=10, int mapCardWidth=20, float cardToCardDistance=0.2f, float fluctuatePosition=0.05f, float fluctuateAngle=2f, int fluctuateSpawn=3, int peacefulRadius=3, int resourceAroundTreeRadius=1)
    {

        _numberOfPlayers = numberOfPlayers;
        _mapCardWidth = mapCardWidth;
        _mapCardHeight = mapCardHeight;
        _cardToCardDistance = cardToCardDistance;
        _fluctuatePosition = fluctuatePosition;
        _fluctuateAngle = fluctuateAngle;
        _fluctuateSpawn = fluctuateSpawn;
        _peacefulRadius = peacefulRadius;
        _resourceAroundTreeRadius = resourceAroundTreeRadius;

        _mapUnityWidth  = (_mapCardWidth  - 1) * Constants.CARD_WIDTH  + (_mapCardWidth  - 1) * _cardToCardDistance;
        _mapUnityHeight = (_mapCardHeight - 1) * Constants.CARD_HEIGHT + (_mapCardHeight - 1) * _cardToCardDistance;

        if (!PhotonNetwork.IsMasterClient)
            return;
        ClearAll();
        SetAdditionalContentPercentage();
        SetNumberOfEachEnemy();
        SetNumberOfTrees();
        SetNumberOfAdditionalContent();
        SetMapId();
        InstantiateCards();
        // MapGenerated?.Invoke();
    }


    private void ClearAll()
    {
        if (_playerIJPositions != null)
        {
            _playerIJPositions.Clear();
        }
        if (_additionalContentPercentage != null)
        {
            _additionalContentPercentage.Clear();
        }
        if (_numberOfEnemy != null)
        {
            _numberOfEnemy.Clear();
        }
        if (_mapId != null)
        {
            _mapId.Clear();
        }
        if (_mapCard != null)
        {
            _mapCard.Clear();
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

        if (_nearestCellsPlayer1 != null)
        {
            _nearestCellsPlayer1.Clear();
        }

        if (_nearestCellsPlayer2 != null)
        {
            _nearestCellsPlayer2.Clear();
        }
    }

    private void SetAdditionalContentPercentage()
    {
        _additionalContentPercentage = new List<int>();

        _additionalContentPercentage.Add(0);  // Empty    (Recommended 0 )
        _additionalContentPercentage.Add(10); // Enemy    (Recommended 10)
        if (_numberOfPlayers == 1) {
            _additionalContentPercentage.Add(90); // Resource (Recommended 90 in singleplayer)
            _additionalContentPercentage.Add(0);  // Tree     (Recommended 0  in singleplayer)
        }
        else {
            _additionalContentPercentage.Add(80); // Resource (Recommended 80 in multiplayer)
            _additionalContentPercentage.Add(10); // Tree     (Recommended 10 in multiplayer)
        }
        _additionalContentPercentage.Add(0);  // Spawn    (Recommended 0 )
    }

    private void SetNumberOfEachEnemy()
    {
        _numberOfEnemy = new List<int>();
        _numberOfEnemy.Add(0);
        _numberOfEnemy.Add(2 * 2); // level 1
        _numberOfEnemy.Add(2 * 2); // level 2
        _numberOfEnemy.Add(2 * 2); // level 3
    }

    private void SetNumberOfTrees()
    {
        _numberOfTrees = (_mapCardWidth * _mapCardHeight) / 50;
    }

    private void SetNumberOfAdditionalContent()
    {
        _numberOfAdditionalContent = (_mapCardWidth * _mapCardHeight) / 6;
    }

    private void RecalculateNearestCells()
    {
        _nearestCellsPlayer1 = new List<Tuple<int, Tuple<int, int>>>();
        _nearestCellsPlayer2 = new List<Tuple<int, Tuple<int, int>>>();

        int _distance1, _distance2;
        for (int i = 0; i < _mapCardHeight; ++i)
        {
            for (int j = 0; j < _mapCardWidth; ++j)
            {
                _distance1 = Math.Abs(_playerIJPositions[0][0] - i) + Math.Abs(_playerIJPositions[0][1] - j);
                _distance2 = Math.Abs(_playerIJPositions[1][0] - i) + Math.Abs(_playerIJPositions[1][1] - j);
                if (_distance1 < _distance2)
                {
                    _nearestCellsPlayer1.Add(new Tuple<int, Tuple<int, int>>(_distance1, new Tuple<int, int>(i, j)));
                }
                else if (_distance2 < _distance1)
                {
                    _nearestCellsPlayer2.Add(new Tuple<int, Tuple<int, int>>(_distance2, new Tuple<int, int>(i, j)));
                }
            }
        }
        _nearestCellsPlayer1.Sort();
        _nearestCellsPlayer2.Sort();

        while (_nearestCellsPlayer1[_nearestCellsPlayer1.Count - 1].Item1 > _nearestCellsPlayer2[_nearestCellsPlayer2.Count - 1].Item1)
        {
            _nearestCellsPlayer1.RemoveAt(_nearestCellsPlayer1.Count - 1);
        }
        while (_nearestCellsPlayer1[_nearestCellsPlayer1.Count - 1].Item1 < _nearestCellsPlayer2[_nearestCellsPlayer2.Count - 1].Item1)
        {
            _nearestCellsPlayer2.RemoveAt(_nearestCellsPlayer2.Count - 1);
        }

        _nearestCellsPlayer1.Reverse();
        _nearestCellsPlayer2.Reverse();
    }

    private int FirstLowerOrEqual(List<Tuple<int, Tuple<int, int>>> _nearestCells, int _distance)
    {
        int left = -1;
        int right = _nearestCells.Count;
        int middle;
        while (right - left > 1)
        {
            middle = (left + right) / 2;
            if (_nearestCells[middle].Item1 > _distance)
            {
                left = middle;
            }
            else
            {
                right = middle;
            }
        }
        return right;
    }

    private bool CorrectCoordinates(int i, int j)
    {
        return ((0 <= i) && (i < _mapCardHeight) && (0 <= j) && (j < _mapCardWidth));
    }

    private void SetMapId()
    {
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

        if (_numberOfPlayers == 2)
        {
            RecalculateNearestCells();
        }


        for (int id = 0; id < _playerIJPositions.Count; ++id)
        {
            _mapId[_playerIJPositions[id][0]][_playerIJPositions[id][1]] = 4;
            for (int i = _playerIJPositions[id][0] - _peacefulRadius; i <= _playerIJPositions[id][0] + _peacefulRadius; ++i)
            {
                for (int j = _playerIJPositions[id][1] - _peacefulRadius; j <= _playerIJPositions[id][1] + _peacefulRadius; ++j)
                {
                    if (CorrectCoordinates(i, j) && _mapId[i][j] == -1 && Math.Abs(_playerIJPositions[id][0] - i) + Math.Abs(_playerIJPositions[id][1] - j) <= _peacefulRadius)
                    {
                        _mapId[i][j] = 0;
                    }
                }
            }
        }

        if (_numberOfPlayers == 2)
        {
            SetTrees();
        }

        if (_numberOfPlayers == 1)
        {
            SetSingleplayerEnemies();
        }
        if (_numberOfPlayers == 2)
        {
            SetMultiplayerEnemies();
        }

        SetAdditionalContent();

        for (int i = 0; i < _mapCardHeight; ++i) {
            for (int j = 0; j < _mapCardWidth; ++j) {
                if (_mapId[i][j] == -1) {
                    _mapId[i][j] = 0;
                }
            }
        }
    }

    private void SetSingleplayerSpawn()
    {
        _playerIJPositions = new List<List<int>>();
        int _spawnPosI = Random.Range(0, _mapCardHeight);
        int _spawnPosJ = Random.Range(0, _mapCardWidth);
        _playerIJPositions.Add(new List<int>());
        _playerIJPositions[0].Add(_spawnPosI);
        _playerIJPositions[0].Add(_spawnPosJ);
    }

    private void SetMultiplayerSpawns()
    {
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


        int _dI1 = Random.Range(-_fluctuateSpawn, _fluctuateSpawn);
        int _dJ1 = Random.Range(-_fluctuateSpawn, _fluctuateSpawn);
        while (!CorrectCoordinates(_firstSpawnPosI + _dI1, _firstSpawnPosJ + _dJ1))
        {
            _dI1 = Random.Range(-_fluctuateSpawn, _fluctuateSpawn);
            _dJ1 = Random.Range(-_fluctuateSpawn, _fluctuateSpawn);
        }
        _firstSpawnPosI += _dI1;
        _firstSpawnPosJ += _dJ1;
        int _dI2 = Random.Range(-_fluctuateSpawn, _fluctuateSpawn);
        int _dJ2 = Random.Range(-_fluctuateSpawn, _fluctuateSpawn);
        while (!CorrectCoordinates(_secondSpawnPosI + _dI2, _secondSpawnPosJ + _dJ2) && (_secondSpawnPosI + _dI2 != _firstSpawnPosI || _secondSpawnPosJ + _dJ2 != _firstSpawnPosJ))
        {
            _dI2 = Random.Range(-_fluctuateSpawn, _fluctuateSpawn);
            _dJ2 = Random.Range(-_fluctuateSpawn, _fluctuateSpawn);
        }
        _secondSpawnPosI += _dI2;
        _secondSpawnPosJ += _dJ2;

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("SetMultiplayerSpawnsRpc", RpcTarget.AllBuffered, _firstSpawnPosI, _firstSpawnPosJ, _secondSpawnPosI, _secondSpawnPosJ);
    }

    private void SetTrees()
    {
        float _e3 = (float)Math.Pow(Math.E, 3);
        float _s, _ix;
        int _distance;
        int p1l, p1r, p2l, p2r;
        int tree1i, tree1j, tree2i, tree2j;
        int rindex;

        int attempt;

        for (int tree = 0; tree < _numberOfTrees / 2; ++tree)
        {
            _s = Random.Range(0f, 1f);
            _ix = (float)Math.Log(1 / (_s / _e3 - _s + 1)) / 3;
            if (Random.Range(0, 2) % 2 == 0)
            {
                _distance = _nearestCellsPlayer1[(int)(_ix * _nearestCellsPlayer1.Count)].Item1;
            }
            else
            {
                _distance = _nearestCellsPlayer2[(int)(_ix * _nearestCellsPlayer2.Count)].Item1;
            }

            p1l = FirstLowerOrEqual(_nearestCellsPlayer1, _distance);
            p1r = FirstLowerOrEqual(_nearestCellsPlayer1, _distance + 1);
            p2l = FirstLowerOrEqual(_nearestCellsPlayer2, _distance);
            p2r = FirstLowerOrEqual(_nearestCellsPlayer2, _distance + 1);

            rindex = Random.Range(p1l, p1r);
            tree1i = _nearestCellsPlayer1[rindex].Item2.Item1;
            tree1j = _nearestCellsPlayer1[rindex].Item2.Item2;
            attempt = 0;
            while (_mapId[tree1i][tree1j] != -1 && attempt < 20)
            {
                attempt++;
                rindex = Random.Range(p1l, p1r);
                tree1i = _nearestCellsPlayer1[rindex].Item2.Item1;
                tree1j = _nearestCellsPlayer1[rindex].Item2.Item2;
            }
            if (attempt >= 20)
            {
                continue;
            }

            rindex = Random.Range(p2l, p2r);
            tree2i = _nearestCellsPlayer2[rindex].Item2.Item1;
            tree2j = _nearestCellsPlayer2[rindex].Item2.Item2;
            attempt = 0;
            while (_mapId[tree2i][tree2j] != -1 && attempt < 20)
            {
              attempt++;
                rindex = Random.Range(p1l, p1r);
                tree2i = _nearestCellsPlayer2[rindex].Item2.Item1;
                tree2j = _nearestCellsPlayer2[rindex].Item2.Item2;
            }
            if (attempt >= 20)
            {
                continue;
            }

            _mapId[tree1i][tree1j] = 3;
            _mapId[tree2i][tree2j] = 3;

            for (int i = tree1i - _resourceAroundTreeRadius; i <= tree1i + _resourceAroundTreeRadius; ++i)
            {
                for (int j = tree1j - _resourceAroundTreeRadius; j <= tree1j + _resourceAroundTreeRadius; ++j)
                {
                    if (CorrectCoordinates(i, j) && _mapId[i][j] == -1 && Math.Abs(i - tree1i) + Math.Abs(j - tree1j) <= _resourceAroundTreeRadius)
                    {
                        _mapId[i][j] = 2;
                    }
                }
            }

            for (int i = tree2i - _resourceAroundTreeRadius; i <= tree2i + _resourceAroundTreeRadius; ++i)
            {
                for (int j = tree2j - _resourceAroundTreeRadius; j <= tree2j + _resourceAroundTreeRadius; ++j)
                {
                    if (CorrectCoordinates(i, j) && _mapId[i][j] == -1 && Math.Abs(i - tree2i) + Math.Abs(j - tree2j) <= _resourceAroundTreeRadius)
                    {
                        _mapId[i][j] = 2;
                    }
                }
            }
        }
    }

    private void SetSingleplayerEnemies()
    {
        int _amountOfEnemies = 0;
        for (int i = 0; i < _numberOfEnemy.Count; ++i)
        {
            _amountOfEnemies += _numberOfEnemy[i];
        }
        List<Tuple<int, Tuple<int, int>>> _enemyInformation = new List<Tuple<int, Tuple<int, int>>>();
        for (int enemy = 0; enemy < _amountOfEnemies; ++enemy)
        {
            int i = Random.Range(0, _mapCardHeight);
            int j = Random.Range(0, _mapCardWidth);
            while (_mapId[i][j] != -1)
            {
                i = Random.Range(0, _mapCardHeight);
                j = Random.Range(0, _mapCardWidth);
            }
            _mapId[i][j] = 1;
            int _distance = Math.Abs(_playerIJPositions[0][0] - i) + Math.Abs(_playerIJPositions[0][1] - j);
            _enemyInformation.Add(new Tuple<int, Tuple<int, int>>(_distance, new Tuple<int, int>(i, j)));
        }
        _enemyInformation.Sort();
        int ind = 0;
        for (int _level = 0; _level < _numberOfEnemy.Count; ++_level)
        {
            for (int i = 0; i < _numberOfEnemy[_level]; ++i)
            {
                _mapId[_enemyInformation[ind].Item2.Item1][_enemyInformation[ind].Item2.Item2] = _level * 10 + 1;
                ind++;
            }
        }
    }

    private void SetMultiplayerEnemies()
    {
        int _amountOfEnemies = 0;
        for (int i = 0; i < _numberOfEnemy.Count; ++i)
        {
            _amountOfEnemies += _numberOfEnemy[i];
        }
        List<Tuple<Tuple<int, int>, Tuple<int, int>>> _enemyInformation = new List<Tuple<Tuple<int, int>, Tuple<int, int>>>();
        for (int enemy = 0; enemy < _amountOfEnemies; ++enemy)
        {
            int i1 = Random.Range(0, _mapCardHeight);
            int j1 = Random.Range(0, _mapCardWidth);
            int i2, j2;
            while (_mapId[i1][j1] != -1)
            {
                i1 = Random.Range(0, _mapCardHeight);
                j1 = Random.Range(0, _mapCardWidth);
            }

            int _distance1 = Math.Abs(_playerIJPositions[0][0] - i1) + Math.Abs(_playerIJPositions[0][1] - j1);
            int _distance2 = Math.Abs(_playerIJPositions[1][0] - i1) + Math.Abs(_playerIJPositions[1][1] - j1);

            int p1i, p1j, p2i, p2j, _distance;
            if (_distance1 < _distance2)
            {
                _distance = _distance1;
                p1i = _playerIJPositions[0][0];
                p1j = _playerIJPositions[0][1];
                p2i = _playerIJPositions[1][0];
                p2j = _playerIJPositions[1][1];
            }
            else
            {
                _distance = _distance2;
                p1i = _playerIJPositions[1][0];
                p1j = _playerIJPositions[1][1];
                p2i = _playerIJPositions[0][0];
                p2j = _playerIJPositions[0][1];
            }
            // i1, j1 -> p1i, p1j   i2, j2 -> p2i, p2j;

            i2 = i1;
            j2 = j1;
            int d2i, d2j;
            while ((i1 == i2 && j1 == j2) || !CorrectCoordinates(i2, j2) || _mapId[i2][j2] != -1)
            {
                d2i = Random.Range(-_distance, _distance + 1);
                i2 = p2i + d2i;
                d2j = _distance - Math.Abs(d2i);
                if (Random.Range(0, 2) == 1)
                {
                    d2j *= -1;
                }
                j2 = p2j + d2j;
            }
            _enemyInformation.Add(new Tuple<Tuple<int, int>, Tuple<int, int>>(new Tuple<int, int>(_distance, enemy * 2 + 1), new Tuple<int, int>(i1, j1)));
            _enemyInformation.Add(new Tuple<Tuple<int, int>, Tuple<int, int>>(new Tuple<int, int>(_distance, enemy * 2 + 2), new Tuple<int, int>(i2, j2)));

            _mapId[i1][j1] = 1;
            _mapId[i2][j2] = 1;
        }
        _enemyInformation.Sort();
        int ind = 0;
        for (int _level = 0; _level < _numberOfEnemy.Count; ++_level)
        {
            for (int i = 0; i < _numberOfEnemy[_level] * 2; ++i)
            {
                _mapId[_enemyInformation[ind].Item2.Item1][_enemyInformation[ind].Item2.Item2] = _level * 10 + 1;
                ind++;
            }
        }
    }

    private void SetAdditionalContent()
    {
        for (int _try = 0; _try < _numberOfAdditionalContent; ++_try)
        {
            int i = Random.Range(0, _mapCardHeight);
            int j = Random.Range(0, _mapCardWidth);
            if (CorrectCoordinates(i, j) && _mapId[i][j] == -1)
            {
                int _cardType = Random.Range(0, 101);
                for (int percentage = 0; percentage < _additionalContentPercentage.Count; ++percentage)
                {
                    if (_cardType <= _additionalContentPercentage[percentage])
                    {
                        _cardType = percentage;
                        break;
                    }
                    _cardType -= _additionalContentPercentage[percentage];
                }
                if (_cardType == 1)
                {
                    _cardType += Random.Range(1, 4) * 10;
                }
                _mapId[i][j] = _cardType;
            }
        }
    }


    [PunRPC]
    private void SetMultiplayerSpawnsRpc(int _firstSpawnPosI, int _firstSpawnPosJ, int _secondSpawnPosI, int _secondSpawnPosJ)
    {
        MapGeneration mg = gameObject.GetComponent<MapGeneration>();
        mg._playerIJPositions = new List<List<int>>();

        mg._playerIJPositions.Add(new List<int>());
        mg._playerIJPositions[0].Add(_firstSpawnPosI);
        mg._playerIJPositions[0].Add(_firstSpawnPosJ);
        mg._playerIJPositions.Add(new List<int>());
        mg._playerIJPositions[1].Add(_secondSpawnPosI);
        mg._playerIJPositions[1].Add(_secondSpawnPosJ);
    }

    void InstantiateCards()
    {

        for (int i = 0; i < _mapCardHeight; ++i)
        {
            for (int j = 0; j < _mapCardWidth; ++j)
            {
                float _posI = - _mapUnityWidth  / 2 + (_mapUnityWidth  / (_mapCardWidth  - 1)) * j;
                float _posJ = - _mapUnityHeight / 2 + (_mapUnityHeight / (_mapCardHeight - 1)) * i;

                float _deltaI = Random.Range(-_fluctuatePosition, _fluctuatePosition);
                float _deltaJ = Random.Range(-_fluctuatePosition, _fluctuatePosition);

                GameObject _newCardObject = PhotonNetwork.InstantiateSceneObject(_cardPrefabs[_mapId[i][j] % 10].name, new Vector3(_posI + _deltaI, _posJ + _deltaJ, 0f), Quaternion.identity);
                int _level = _mapId[i][j] / 10;
                _newCardObject.GetComponent<CardSpawner>().Initialize(i, j, _level, _mapCardWidth, Random.Range(3, 1488));
            }
        }
    }

    // For singleplayer
    public Vector3 GetSpawnCoords()
    {
        return new Vector3(-_mapUnityWidth  / 2 + (_mapUnityWidth  / (_mapCardWidth  - 1)) * _playerIJPositions[0][1],
                           -_mapUnityHeight / 2 + (_mapUnityHeight / (_mapCardHeight - 1)) * _playerIJPositions[0][0],
                           0f);
    }

    public Card GetSpawnCard()
    {
        return _mapCard[_playerIJPositions[0][0]][_playerIJPositions[0][1]];
    }

    // For multiplayer
    public Vector3 GetFirstSpawnCoords()
    {
        return new Vector3(-_mapUnityWidth  / 2 + (_mapUnityWidth  / (_mapCardWidth  - 1)) * _playerIJPositions[0][1],
                           -_mapUnityHeight / 2 + (_mapUnityHeight / (_mapCardHeight - 1)) * _playerIJPositions[0][0],
                           0f);
    }

    public Card GetFirstSpawnCard()
    {
        return _mapCard[_playerIJPositions[0][0]][_playerIJPositions[0][1]];
    }

    // For multiplayer
    public Vector3 GetSecondSpawnCoords()
    {
        return new Vector3(-_mapUnityWidth  / 2 + (_mapUnityWidth  / (_mapCardWidth  - 1)) * _playerIJPositions[1][1],
                           -_mapUnityHeight / 2 + (_mapUnityHeight / (_mapCardHeight - 1)) * _playerIJPositions[1][0],
                           0f);
    }

    public Card GetSecondSpawnCard()
    {
        return _mapCard[_playerIJPositions[1][0]][_playerIJPositions[1][1]];
    }

    // It is temporary. We should not use it.
    public List<List<Card>> GetMap()
    {
        return _mapCard;
    }

    public int GetMapCardWidth()
    {
        return _mapCardWidth;
    }

    public int GetMapCardHeight()
    {
        return _mapCardHeight;
    }

    public float GetMapUnityWidth()
    {
        return _mapUnityWidth;
    }

    public float GetMapUnityHeight()
    {
        return _mapUnityHeight;
    }

    public void AddCardToRow(int i, Card _newCard)
    {
        if (_mapCard == null) _mapCard = new List<List<Card>>();
        if (_mapObject == null) _mapObject = new List<List<GameObject>>();

        if (_mapCard.Count == i)
            _mapCard.Add(new List<Card>());
        else if (_mapCard.Count < i)
            return;

        if (_mapObject.Count == i)
            _mapObject.Add(new List<GameObject>());
        else if (_mapObject.Count < i)
            return;

        _mapCard[i].Add(_newCard);
        _mapObject[i].Add(_newCard.gameObject);
        if (_mapCard.Count == _mapCardHeight && _mapCard[i].Count == _mapCardWidth - 1)
        {
            Debug.Log("MapGenerated");
            MapGenerated.Invoke();
        }
    }
}
