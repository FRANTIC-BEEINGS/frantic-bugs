using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using ResourceManagment;
using UnityEngine;
using UnityEngine.Serialization;

public class Unit : MonoBehaviour
{
    [SerializeField] int moveEnergy;
    [SerializeField] int captureEnergy;
    [SerializeField] double forceCoef;
    [SerializeField] int fightEnergy;
    [SerializeField] int level = 1;
    [SerializeField] int resourceEnergy;
    [SerializeField] double increaseCoef;
    [SerializeField] double decreaseCoef;
    [SerializeField] int sight;
    [SerializeField] private Sprite sprite;
    private Vector3 endPosition;
    [SerializeField] private float movingTime = 0.5f;
    private bool isStopMovement = false;
    public AllegianceType Allegiance = AllegianceType.A;
    private int experience;
    [SerializeField] private int experienceLimit;
    public Action FinishedMovement;

    public int FightEnergy
    {
        get => fightEnergy;
    }

    public int Force
    {
        get => (int)(level * forceCoef);
    }

    public int ResourceEnergy
    {
        get => resourceEnergy;
    }

    public int CaptureEnergy
    {
        get => captureEnergy;
    }
    

    public void stopMovement()
    {
        isStopMovement = true;
    }

    // increase level and change characteristics 
    public void IncreaseLevel()
    {
        level += 1;
        forceCoef = (int)(forceCoef * increaseCoef);
        moveEnergy = (int)(moveEnergy * decreaseCoef);
        captureEnergy = (int)(captureEnergy * decreaseCoef);
        fightEnergy = (int)(fightEnergy * decreaseCoef);
        resourceEnergy = (int)(resourceEnergy * decreaseCoef);
    }

    public void IncreaseExperience(int exp)
    {
        experience += exp;
        if (experience > experienceLimit)
        {
            for (int i = 0; i < experience / experienceLimit; i++)
                IncreaseLevel();
            
            experience %= experienceLimit;
        }
    }

    public void MoveAlongPath(List<Card> cards, ResourceManager resourceManager)
    {
        StartCoroutine(Move(cards, resourceManager));
    }

    IEnumerator Move(List<Card> cards, ResourceManager resourceManager)
    {
        for(int i = 1; i < cards.Count; i++)
        {
            // check if we can step on next card and if player want stop
            if (isStopMovement || !cards[i].StepOn(this))
            {
                isStopMovement = false;
                break;
            }

            endPosition = cards[i].gameObject.transform.position; // find destination position
            if (resourceManager.GetResource(ResourceType.Energy) < moveEnergy)
            {
                isStopMovement = false;
                break;
            }
            resourceManager.AddResource(ResourceType.Energy, -moveEnergy);
            cards[i - 1].LeaveCard();
            yield return StartCoroutine(MoveTo(endPosition, movingTime)); //start one movement
            
            //if card is enemy break movement
            if (cards[i] is EnemyCard)
            {
                isStopMovement = false;
                break;
            }
        }
        isStopMovement = false;
        FinishedMovement?.Invoke();
    }

    IEnumerator MoveTo(Vector3 position, float time)
    {
        Vector3 start = transform.position;
        Vector3 end = new Vector3(position.x, position.y, start.z);
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

    public void Death() 
    {
        Destroy(this);
    }
}
