using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;

/*
этот скрипт должен сработать один раз, чтобы установить карточки.
*/

enum CardId {
    Empty = 0,
    Enemy = 1,
    Resource = 2,
    Tree = 3
}

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


    void Start() {
        SetMapId();
        InstantiateCards();
    }


    void SetMapId() {
        CardTypeCnt[(int)CardId.Empty] = MapCardHeight * MapCardWidth;
        for (int i = 1; i < CardTypeCnt.Count; ++i) {
            CardTypeCnt[(int)CardId.Empty] -= CardTypeCnt[i];
        }

        MapId = new List<List<int>>();
        for (int i = 0; i < MapCardHeight; ++i) {
            MapId.Add(new List<int>());
            for (int j = 0; j < MapCardWidth; ++j) {
                MapId[i].Add(-1);
            }
        }

        for (int CardId = 0; CardId < CardTypeCnt.Count; ++CardId) {
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
                BodyInformation Body = NewCardObject.transform.GetChild(0).GetComponent<BodyInformation>();
                Body.id = i * MapCardWidth + j;
                Map[i].Add(NewCard);
            }
        }
    }


    void Update() {

    }
}
