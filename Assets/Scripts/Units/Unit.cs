using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Func<AllegianceType> GetAllegiance;
    [SerializeField] int moveEnergy;
    [SerializeField] int captureEnergy;
    [SerializeField] int force;
    [SerializeField] int fightEnergy;
    [SerializeField] int level;
    [SerializeField] int resourceEnergy;
    [SerializeField] double increaseCoef;
    [SerializeField] double decreaseCoef;
    [SerializeField] int sight;
    [SerializeField] private Sprite sprite;
    private Vector3 endPosition;
    [SerializeField] private float movingTime = 1f;
    private bool isStopMovement = false;

    public void stopMovement()
    {
        isStopMovement = true;
    }

    // increase level and change characteristics 
    public void IncreaseLevel()
    {
        level += 1;
        force = (int)(force * increaseCoef);
        moveEnergy = (int)(moveEnergy * decreaseCoef);
        captureEnergy = (int)(captureEnergy * decreaseCoef);
        fightEnergy = (int)(fightEnergy * decreaseCoef);
        resourceEnergy = (int)(resourceEnergy * decreaseCoef);
    }

    //for movement test
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (GUILayout.Button("Stop moving")) stopMovement();
        if (GUILayout.Button("Start again")) Start();
        GUILayout.EndArea();
    }
    

    IEnumerator Move(List<Card> cards, int energy)
    {
        foreach (Card card in cards)
        {
            if (isStopMovement)
            {
                isStopMovement = false;
                break;
            }

            endPosition = card.gameObject.transform.position; // find destination position
            if (energy < moveEnergy)
                break;
            energy -= moveEnergy;
            yield return StartCoroutine(MoveTo(endPosition, movingTime)); //start one movement
        }
    }
    
    // for movement test
    IEnumerator MoveTest(List<Vector3> cards, int energy)
    {
        foreach (Vector3 card in cards)
        {
            if (isStopMovement)
            {
                isStopMovement = false;
                break;
            }
            
            endPosition = card;
            if (energy < moveEnergy)
                break;
            energy -= moveEnergy;
            yield return StartCoroutine(MoveTo(endPosition, movingTime));
        }
    }
    
    
    IEnumerator MoveTo(Vector3 position, float time)
    {
        Vector3 start = transform.position;
        Vector3 end = position;
        float t = 0;
        
        //in every moment move to destination
        while(t < 1)
        {
            yield return null;
            t += Time.deltaTime / time;
            transform.position = Vector3.Lerp(start, end, t);
        }
        transform.position = end;  
    }

    private void Start()
    {
        //test movement
        List<Vector3> cards = new List<Vector3>()
        {
            new Vector3(5, -2, 0), new Vector3(4, 1, 0), new Vector3(-3, 2, 0), 
            new Vector3(0, 0, 0), new Vector3(5, -2, 0)
        };
        StartCoroutine(MoveTest(cards, 100));
    }

    private void Death() 
    {
        Destroy(this);
    }
}
