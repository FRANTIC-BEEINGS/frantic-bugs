using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using Photon.Pun;
using ResourceManagment;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardSpawner : MonoBehaviour
{
    public void Initialize(int i, int j, int level, int mapCardWidth, int seed)
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("EditCard", RpcTarget.All, i, j, level, mapCardWidth, seed);
    }

    [PunRPC]
    private void EditCard(int i, int j, int _level, int _mapCardWidth, int seed)
    {
        Random.seed = seed;
        transform.eulerAngles = new Vector3(0, 180, Random.Range(-2, 2));
        GameObject map = GameObject.Find("Map");
        gameObject.transform.parent = map.transform;
        MapGeneration mapGeneration = map.GetComponent<MapGeneration>();

        Card _newCard = gameObject.GetComponent<Card>();
        if (_newCard is EnemyCard)
        {
            switch (_level)
            {
                case 1:
                    gameObject.transform.GetChild(2).gameObject.SetActive(true);
                    break;
                case 2:
                    gameObject.transform.GetChild(3).gameObject.SetActive(true);
                    break;
                case 3:
                    gameObject.transform.GetChild(4).gameObject.SetActive(true);
                    break;
            }

            ((EnemyCard) _newCard).Initialize(_level, 50,
                new Dictionary<ResourceType, int>()
                    {{ResourceType.Food, 10 * _level}, {ResourceType.Money, 100 * _level}});
        }
        else if (_newCard is ResourceCard)
        {
            var resourceTypes = Enum.GetValues(typeof(ResourceType));
            var resourceType = (ResourceType) resourceTypes.GetValue(Random.Range(0, resourceTypes.Length));
            switch (resourceType)
            {
                case ResourceType.Food:
                    gameObject.transform.GetChild(4).gameObject.SetActive(true);
                    ((ResourceCard) _newCard).Initialize(ResourceType.Food, Random.Range(5, 20));
                    break;
                case ResourceType.Energy:
                    gameObject.transform.GetChild(3).gameObject.SetActive(true);
                    ((ResourceCard) _newCard).Initialize(ResourceType.Energy, Random.Range(11, 15));
                    break;
                case ResourceType.Money:
                    gameObject.transform.GetChild(2).gameObject.SetActive(true);
                    ((ResourceCard) _newCard).Initialize(ResourceType.Money, Random.Range(180, 300));
                    break;
                default:
                    gameObject.transform.GetChild(2).gameObject.SetActive(true);
                    ((ResourceCard) _newCard).Initialize(ResourceType.Money, Random.Range(180, 300));
                    break;
            }
        }
        /*
        else if (_newCard is TreeCard)
        {
            ((TreeCard) _newCard).Initialize(4);
        }
        */

        BodyInformation _body = gameObject.transform.GetChild(0).GetComponent<BodyInformation>();
        _body.id = i * _mapCardWidth + j;

        mapGeneration.AddCardToRow(i, _newCard);
    }
}
