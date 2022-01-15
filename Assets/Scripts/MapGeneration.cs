using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using ResourceManagment;
using Random = UnityEngine.Random;

/*
этот скрипт должен сработать один раз, чтобы установить карточки.
*/

public class MapGeneration : MonoBehaviour  {

    // MapCardHeight и MapCardWidth - размеры поля в карточках
    [SerializeField] public int MapCardHeight;
    [SerializeField] public int MapCardWidth;
    // CardHeight и CardWidth - размеры одной карточки
    [SerializeField] private float CardHeight;
    [SerializeField] private float CardWidth;
    // CardToCardDistance - растояние между соседними карточками
    [SerializeField] private float CardToCardDistance;

    [SerializeField] private bool fluctuation = false;

    // MapId - то, как видит данное поле игрок (содержит в себе id карточек) (должно приходить от сервера)
    // CardPrefabs - список всевозможных карточек
    [SerializeField] private List<GameObject> CardPrefabs;
    [SerializeField] private List<int> CardTypeCnt;

    public List<List<Card>> Map;
    private List<List<int>> MapId;

    public Action MapGenerated;

    public void StartMapGeneration()
    {
        SetMapId();
        InstantiateCards();
        MapGenerated?.Invoke();
    }
    
    void Start() {
        SetMapId();
        InstantiateCards();
        MapGenerated?.Invoke();
    }

    bool CorrectCoordinates(int x, int y) {
        return ((0 <= x) && (x < MapCardHeight) && (0 <= y) && (y < MapCardWidth));
    }

    void SetMapId() {
        CardTypeCnt[0] = MapCardHeight * MapCardWidth;
        for (int i = 1; i < CardTypeCnt.Count; ++i) {
            CardTypeCnt[0] -= CardTypeCnt[i];
        }

        MapId = new List<List<int>>();
        for (int i = 0; i < MapCardHeight; ++i) {
            MapId.Add(new List<int>());
            for (int j = 0; j < MapCardWidth; ++j) {
                MapId[i].Add(-1);
            }
        }

        int MainUnitPosx = 0;
        int MainUnitPosy = MapCardWidth / 2;

        int PeacefulRadius = 2;
        for (int x = MainUnitPosx - PeacefulRadius; x <= MainUnitPosx + PeacefulRadius; ++x) {
            for (int y = MainUnitPosy - PeacefulRadius; y <= MainUnitPosy + PeacefulRadius; ++y) {
                if (CorrectCoordinates(x, y)) {
                    MapId[x][y] = 0;
                }
            }
        }

        for (int CardId = 1; CardId < CardTypeCnt.Count; ++CardId) {
            for (int cnt = 0; cnt < CardTypeCnt[CardId]; ++cnt) {
                int i = Random.Range(0, MapId.Count);
                int j = Random.Range(0, MapId[i].Count);
                while (MapId[i][j] != -1) {
                    i = Random.Range(0, MapId.Count);
                    j = Random.Range(0, MapId[i].Count);
                }
                MapId[i][j] = CardId;
            }
        }
        for (int i = 0; i < MapCardHeight; ++i) {
            for (int j = 0; j < MapCardWidth; ++j) {
                if (MapId[i][j] == -1) {
                    MapId[i][j] = 0;
                }
            }
        }
    }


    void InstantiateCards() {
        Map = new List<List<Card>>();
        for (int i = 0; i < MapCardHeight; ++i) {
            Map.Add(new List<Card>());
            for (int j = 0; j < MapCardWidth; ++j) {
                float MapUnitWidth  = (MapCardWidth  - 1) * CardWidth  + (MapCardWidth  - 1) * CardToCardDistance;
                float MapUnitHeight = (MapCardHeight - 1) * CardHeight + (MapCardHeight - 1) * CardToCardDistance;
                float PosX = - MapUnitWidth  / 2 + (MapUnitWidth  / (MapCardWidth  - 1)) * j;
                float PosY = - MapUnitHeight / 2 + (MapUnitHeight / (MapCardHeight - 1)) * i;

                float deltaX = 0;
                float deltaY = 0;
                if (fluctuation) {
                    deltaX = Random.Range(-0.05f, 0.05f);
                    deltaY = Random.Range(-0.05f, 0.05f);
                }

                GameObject NewCardObject = Instantiate(CardPrefabs[MapId[i][j]], new Vector3(PosX + deltaX, PosY + deltaY, 0f), Quaternion.identity);
                if (fluctuation) {
                    NewCardObject.transform.eulerAngles = new Vector3(0, 0, Random.Range(-2, 2));
                }
                NewCardObject.transform.parent = gameObject.transform;

                //Spawn();
                Card NewCard = NewCardObject.GetComponent<Card>();
                if (NewCard is EnemyCard)
                {
                    int level = Random.Range(1, 3);
                    ((EnemyCard)NewCard).Initialize(level, 50,
                        new Dictionary<ResourceType, int>() {{ResourceType.Food, 10*level}, {ResourceType.Money, 100*level}});
                }
                else if (NewCard is ResourceCard)
                {
                    var resourceTypes = Enum.GetValues (typeof (ResourceType));
                    var resourceType = (ResourceType)resourceTypes.GetValue(Random.Range(0, resourceTypes.Length));
                    switch (resourceType)
                    {
                        case ResourceType.Food:
                            NewCardObject.transform.GetChild(4).gameObject.SetActive(true);
                            ((ResourceCard)NewCard).Initialize(ResourceType.Food, Random.Range(5, 20));
                            break;
                        case ResourceType.Energy:
                            NewCardObject.transform.GetChild(3).gameObject.SetActive(true);
                            ((ResourceCard)NewCard).Initialize(ResourceType.Energy, Random.Range(1, 5));
                            break;
                        case ResourceType.Money:
                            NewCardObject.transform.GetChild(2).gameObject.SetActive(true);
                            ((ResourceCard)NewCard).Initialize(ResourceType.Money, Random.Range(180, 300));
                            break;
                        default:
                            NewCardObject.transform.GetChild(2).gameObject.SetActive(true);
                            ((ResourceCard)NewCard).Initialize(ResourceType.Money, Random.Range(180, 300));
                            break;
                    }
                }
                BodyInformation Body = NewCardObject.transform.GetChild(0).GetComponent<BodyInformation>();
                Body.id = i * MapCardWidth + j;
                Map[i].Add(NewCard);
            }
        }
    }


    void Update() {

    }
}
