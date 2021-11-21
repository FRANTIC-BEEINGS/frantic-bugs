using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

*/

public class MapLocalGeneration : MonoBehaviour  {

    // MapCardHeight и MapCardWidth - размеры поля в карточках (должны приходить от сервера)
    public int MapCardHeight;
    public int MapCardWidth;
    // CardHeight и CardWidth - размеры одной карточки
    public float CardHeight;
    public float CardWidth;
    // CardToCardDistance - растояние между соседними карточками
    public float CardToCardDistance;
    // MapId - то, как видит данное поле игрок (содержит в себе id карточек) (должно приходить от сервера)
    // CardPrefabs - список всевозможных карточек
    public List<GameObject> CardPrefabs;
    public GameObject Prefab;

    private List<List<GameObject>> Map;
    private List<List<int>> MapId;


    private const bool fluctuation = false;

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
                MapId[i].Add(0);
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
                float PosZ = - MapUnitHeight / 2 + (MapUnitHeight / (MapCardHeight - 1)) * i;

                float deltaX = 0;
                float deltaZ = 0;
                if (fluctuation) {
                    deltaX = Random.Range(-0.05f, 0.05f);
                    deltaZ = Random.Range(-0.05f, 0.05f);
                }

                Map[i].Add(Instantiate(Prefab, new Vector3(PosX + deltaX, 0f, PosZ + deltaZ), Quaternion.identity));

                if (fluctuation) {
                    Map[i][j].transform.eulerAngles = new Vector3(0, Random.Range(-2, 2), 0);
                }
            }
        }
    }

    void Update() {

    }
}
