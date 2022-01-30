using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TavernGeneration : MonoBehaviour
{
    private const float COIN_HEIGHT = 0.21f;
    private const float CARD_HEIGHT = 0.1f;
    private const float CARD_ANGLE_FLUCTUATION = 10f;
    private const float EPS = 0.0001f;
    private const float _borderX = 15, _borderY = 10;

    private float _mapWidth, _mapHeight;
    private float _requiredWidth, _requiredHeight;
    private float _fieldWidth, _fieldHeight;

    [SerializeField] private GameObject TablePrefab;
    private float _tableWidth = 29.7f * 2, _tableHeight = 21 * 2;

    [SerializeField] private List<GameObject> StuffPrefabs;
    [SerializeField] private List<int> AmountOfObjects;
    private List<GameObject> StuffObjects;
    private List<GameObject> TablesObjects;


    public void Initialize(float mapUnityWidth, float mapUnityHeight)
    {
        if (StuffObjects != null)
        {
            for (int i = 0; i < StuffObjects.Count; ++i)
            {
                Destroy(StuffObjects[i]);
            }
            StuffObjects.Clear();
        }
        if (TablesObjects != null)
        {
            for (int i = 0; i < TablesObjects.Count; ++i)
            {
                Destroy(TablesObjects[i]);
            }
            TablesObjects.Clear();
        }
        StuffObjects = new List<GameObject>();
        _mapWidth = mapUnityWidth + Constants.CARD_WIDTH;
        _mapHeight = mapUnityHeight + Constants.CARD_HEIGHT;
        _requiredWidth = _mapWidth + 2 * _borderX;
        _requiredHeight = _mapHeight + 2 * _borderY;
        SetTables();
        SetStuff();
    }

    private void SetTables()
    {
        int _fieldTableWidth, _fieldTableHeight;
        int _fieldRotatedTableWidth, _fieldRotatedTableHeight;
        _fieldTableWidth  = (int)((_requiredWidth  + _tableWidth  - EPS) / _tableWidth );
        _fieldTableHeight = (int)((_requiredHeight + _tableHeight - EPS) / _tableHeight);
        _fieldRotatedTableWidth  = (int)((_requiredWidth  + _tableHeight - EPS) / _tableHeight);
        _fieldRotatedTableHeight = (int)((_requiredHeight + _tableWidth  - EPS) / _tableWidth );

        if (Math.Max(_fieldTableWidth * _tableWidth - _requiredWidth, _fieldTableHeight * _tableHeight - _requiredHeight) < Math.Max(_fieldRotatedTableWidth * _tableHeight - _requiredWidth, _fieldRotatedTableHeight * _tableWidth - _requiredHeight))
        {
            _fieldWidth = _fieldTableWidth * _tableWidth;
            _fieldHeight = _fieldTableHeight * _tableHeight;
            for (int i = 0; i < _fieldTableHeight; ++i)
            {
                for (int j = 0; j < _fieldTableWidth; ++j)
                {
                    float _posX = -_fieldWidth  / 2 + _tableWidth  / 2 + _tableWidth  * j;
                    float _posY = -_fieldHeight / 2 + _tableHeight / 2 + _tableHeight * i;
                    GameObject Table = Instantiate(TablePrefab, new Vector3(_posX, _posY, 0f), Quaternion.identity);
                    TablesObjects.Add(Table);
                    Table.transform.parent = gameObject.transform;
                }
            }
        }
        else // rotate the table
        {
            _fieldWidth = _fieldRotatedTableWidth * _tableHeight;
            _fieldHeight = _fieldRotatedTableHeight * _tableWidth;
            for (int i = 0; i < _fieldRotatedTableHeight; ++i)
            {
                for (int j = 0; j < _fieldRotatedTableWidth; ++j)
                {
                    float _posX = -_fieldWidth  / 2 + _tableHeight / 2 + _tableHeight * j;
                    float _posY = -_fieldHeight / 2 + _tableWidth  / 2 + _tableWidth  * i;
                    GameObject Table = Instantiate(TablePrefab, new Vector3(_posX, _posY, 0f), Quaternion.Euler(0, 0, 90));
                    Table.transform.parent = gameObject.transform;
                }
            }
        }
    }

    private bool GoodPosition(float x, float y, int i)
    {
        float _radius = StuffPrefabs[i].GetComponent<SphereCollider>().radius * StuffPrefabs[i].transform.localScale.x;
        if (x + _radius > _fieldWidth / 2 || x - _radius < -_fieldWidth / 2 || y + _radius > _fieldHeight / 2 || y - _radius < -_fieldHeight / 2) // not on tables
        {
            return false;
        }
        if (-_mapWidth / 2 - _radius < x && x < _mapWidth / 2 + _radius && -_mapHeight / 2 - _radius < y && y < _mapHeight / 2 + _radius) // on cards
        {
            return false;
        }
        for (int j = 0; j < StuffObjects.Count; ++j)
        {
            if (StuffObjects[j].GetComponent<SphereCollider>().radius * StuffObjects[j].transform.localScale.x + _radius > Vector3.Distance(new Vector3(x, y, 0), StuffObjects[j].transform.position)) // collision
            {
                return false;
            }
        }
        return true;
    }

    private void SetStuff()
    {
        for (int i = 0; i < StuffPrefabs.Count; ++i)
        {
            for (int j = 0; j < AmountOfObjects[i]; ++j)
            {

                int attempts = 20;

                float _rx = 0, _ry = 0, _ra = 0;

                while (attempts > 0 && !GoodPosition(_rx, _ry, i))
                {
                    _rx = Random.Range(-_fieldWidth / 2, _fieldWidth / 2);
                    _ry = Random.Range(-_fieldHeight / 2, _fieldHeight / 2);
                    _ra = Random.Range(0f, 360f);
                    attempts--;
                }

                if (attempts > 0) // GoodPosition(_rx, _ry, i) == true
                {
                    GameObject _newStuff = Instantiate(StuffPrefabs[i], new Vector3(_rx, _ry, 0), Quaternion.Euler(0, 0, _ra));
                    StuffObjects.Add(_newStuff);
                    _newStuff.transform.parent = gameObject.transform;
                    if (_newStuff.name == "Coin(Clone)") // stack of coins
                    {
                        int _amountOfCoins = 1;
                        float _addCoin = Random.Range(0f, 1f);
                        while (_amountOfCoins < 15 && _addCoin < 0.66f)
                        {
                            _ra = Random.Range(0f, 360f);
                            GameObject _newCoin = Instantiate(StuffPrefabs[i], new Vector3(_rx, _ry, _amountOfCoins * -COIN_HEIGHT), Quaternion.Euler(0, 0, _ra));
                            StuffObjects.Add(_newCoin);
                            _newCoin.transform.parent = gameObject.transform;
                            _addCoin = Random.Range(0f, 1f);
                            _amountOfCoins++;
                        }
                    }
                    else if (_newStuff.name == "Card(Clone)") // card deck
                    {
                        int _amountOfCards = 1;
                        float _addCard = Random.Range(0f, 1f);
                        while (_amountOfCards < 10 && _addCard < 0.66f)
                        {
                            GameObject _newCard = Instantiate(StuffPrefabs[i], new Vector3(_rx, _ry, _amountOfCards * -CARD_HEIGHT), Quaternion.Euler(0, 0, _ra + Random.Range(-CARD_ANGLE_FLUCTUATION, CARD_ANGLE_FLUCTUATION)));
                            StuffObjects.Add(_newCard);
                            _newCard.transform.parent = gameObject.transform;
                            _addCard = Random.Range(0f, 1f);
                            _amountOfCards++;
                        }
                    }
                }
            }
        }
        for (int i = 0; i < StuffObjects.Count; ++i)
        {
            Destroy(StuffObjects[i].GetComponent<SphereCollider>());
        }
    }
}
