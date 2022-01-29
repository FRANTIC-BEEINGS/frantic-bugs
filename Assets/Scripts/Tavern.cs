using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tavern : MonoBehaviour
{
    private const float EPS = 0.0001f;
    private float _mapWidth, _mapHeight;
    private const float _borderX = 15, _borderY = 10;
    private float _requiredWidth, _requiredHeight;
    private float _fieldWidth, _fieldHeight;

    [SerializeField] private GameObject TablePrefab;
    private float _tableWidth = 29.7f * 2, _tableHeight = 21 * 2;

    [SerializeField] private GameObject MapCollider;
    [SerializeField] private GameObject LeftBorderCollider;
    [SerializeField] private GameObject RightBorderCollider;
    [SerializeField] private GameObject UpBorderCollider;
    [SerializeField] private GameObject DownBorderCollider;

    [SerializeField] private List<GameObject> Stuff;

    void Start()
    {
        Initialize(156.15f, 61.193f);
    }

    public void Initialize(float mapUnityWidth, float mapUnityHeight)
    {
        _mapWidth = mapUnityWidth + Constants.CARD_WIDTH;
        _mapHeight = mapUnityHeight + Constants.CARD_HEIGHT;

        _requiredWidth = _mapWidth + 2 * _borderX;
        _requiredHeight = _mapHeight + 2 * _borderY;
        Debug.Log(_requiredWidth);
        Debug.Log(_requiredHeight);
        Debug.Log(EPS);
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

        //if (_fieldTableWidth * _fieldTableHeight <= _fieldRotatedTableWidth * _fieldRotatedTableHeight) // do not rotate the table
        if (Math.Max(_fieldTableWidth * _tableWidth - _requiredWidth, _fieldTableHeight * _tableHeight - _requiredHeight) < Math.Max(_fieldRotatedTableWidth * _tableHeight - _requiredWidth, _fieldRotatedTableHeight * _tableWidth - _requiredHeight))
        {
            Debug.Log("Normal");
            Debug.Log(_fieldTableWidth);
            Debug.Log(_fieldTableHeight);
            _fieldWidth = _fieldTableWidth * _tableWidth;
            _fieldHeight = _fieldTableHeight * _tableHeight;
            for (int i = 0; i < _fieldTableHeight; ++i)
            {
                for (int j = 0; j < _fieldTableWidth; ++j)
                {
                    float _posX = -_fieldWidth  / 2 + _tableWidth  / 2 + _tableWidth  * j;
                    float _posY = -_fieldHeight / 2 + _tableHeight / 2 + _tableHeight * i;
                    GameObject Table = Instantiate(TablePrefab, new Vector3(_posX, _posY, 0f), Quaternion.identity);
                }
            }
        }
        else // rotate the table
        {
            Debug.Log("Rotated");
            Debug.Log(_fieldRotatedTableWidth);
            Debug.Log(_fieldRotatedTableHeight);
            _fieldWidth = _fieldRotatedTableWidth * _tableHeight;
            _fieldHeight = _fieldRotatedTableHeight * _tableWidth;
            for (int i = 0; i < _fieldRotatedTableHeight; ++i)
            {
                for (int j = 0; j < _fieldRotatedTableWidth; ++j)
                {
                    float _posX = -_fieldWidth  / 2 + _tableHeight / 2 + _tableHeight * j;
                    float _posY = -_fieldHeight / 2 + _tableWidth  / 2 + _tableWidth  * i;
                    GameObject Table = Instantiate(TablePrefab, new Vector3(_posX, _posY, 0f), Quaternion.Euler(0, 0, 90));
                }
            }
        }
    }

    private void SetStuff()
    {
        MapCollider.transform.localScale = new Vector3(_mapWidth, _mapHeight, 1);
        LeftBorderCollider.transform.position = new Vector3(-_fieldWidth / 2, 0, 0);
        LeftBorderCollider.transform.localScale = new Vector3(1, _fieldHeight, 1);
        RightBorderCollider.transform.position = new Vector3(_fieldWidth / 2, 0, 0);
        RightBorderCollider.transform.localScale = new Vector3(1, _fieldHeight, 1);
        UpBorderCollider.transform.position = new Vector3(0, _fieldHeight / 2, 0);
        UpBorderCollider.transform.localScale = new Vector3(_fieldWidth, 1, 1);
        DownBorderCollider.transform.position = new Vector3(0, -_fieldHeight / 2, 0);
        DownBorderCollider.transform.localScale = new Vector3(_fieldWidth, 1, 1);

        

        /* * /
        MapCollider.Destroy();
        LeftBorderCollider.Destroy();
        RightBorderCollider.Destroy();
        UpBorderCollider.Destroy();
        DownBorderCollider.Destroy();
        */
    }
}
