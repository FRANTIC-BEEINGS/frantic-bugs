using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
этот скрипт должен сработать один раз, чтобы установить карточки.
*/

public class MapGeneration : MonoBehaviour  {

    // MapCardHeight и MapCardWidth - размеры поля в карточках
    public int MapCardHeight;
    public int MapCardWidth;
    // CardHeight и CardWidth - размеры одной карточки
    public float CardHeight;
    public float CardWidth;
    // CardToCardDistance - растояние между соседними карточками
    public float CardToCardDistance;

    [SerializeField] bool fluctuation = false;

    // MapId - то, как видит данное поле игрок (содержит в себе id карточек) (должно приходить от сервера)
    // CardPrefabs - список всевозможных карточек
    public List<GameObject> CardPrefabs;

    private List<List<GameObject>> Map;
    private List<List<int>> MapId;


    void Start() {
        Initialization();
        GetMapId();
        InstantiateCards();
    }

    void Initialization() {
        // получить всякие константы от сервера
    }

    void GetMapId() {
        MapId = new List<List<int>>();
        for (int i = 0; i < MapCardHeight; ++i) {
            MapId.Add(new List<int>());
            for (int j = 0; j < MapCardWidth; ++j) {
                MapId[i].Add(Random.Range(0, CardPrefabs.Count));
            }
        }
    }

    void InstantiateCards() {
        Map = new List<List<GameObject>>();
        for (int i = 0; i < MapCardHeight; ++i) {
            Map.Add(new List<GameObject>());
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

                GameObject NewCard = Instantiate(CardPrefabs[MapId[i][j]], new Vector3(PosX + deltaX, PosY + deltaY, 0f), Quaternion.identity);
                if (fluctuation) {
                    NewCard.transform.eulerAngles = new Vector3(0, 0, Random.Range(-2, 2));
                }
                NewCard.transform.parent = gameObject.transform;

                //Spawn();

                Map[i].Add(NewCard);
            }
        }
    }

    void Update() {

    }
}
