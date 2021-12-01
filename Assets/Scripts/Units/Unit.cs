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
    public Action<EnemyCard> FightEnemy;
    public Action OnDeath;
    public Action<int> OnLevelChange;

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
        OnLevelChange(level);
        forceCoef = (int)(forceCoef * increaseCoef);
        moveEnergy = (int)(moveEnergy * decreaseCoef);
        captureEnergy = (int)(captureEnergy * decreaseCoef);
        fightEnergy = (int)(fightEnergy * decreaseCoef);
        resourceEnergy = (int)(resourceEnergy * decreaseCoef);
    }

    public void IncreaseExperience(int exp)
    {
        experience += exp;
        if (experience >= experienceLimit)
        {
            for (int i = 0; i < experience / experienceLimit; i++)
                IncreaseLevel();

            experience %= experienceLimit;
        }
    }

    public void MoveAlongPath(List<Card> cards, ResourceManager resourceManager)
    {
        isStopMovement = false;
        StartCoroutine(Move(cards, resourceManager));
    }

    IEnumerator Move(List<Card> cards, ResourceManager resourceManager)
    {
        for(int i = 1; i < cards.Count; i++)
        {
            // check if we can step on next card and if player want stop
            if (isStopMovement || !cards[i].StepOn(this) || resourceManager.GetResource(ResourceType.Energy) < moveEnergy)
            {
                break;
            }

            endPosition = cards[i].gameObject.transform.position; // find destination position
            resourceManager.AddResource(ResourceType.Energy, -moveEnergy);
            cards[i - 1].LeaveCard();

            // temporary crutch
            cards[i - 1].gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            //BI.SetHighlight(false);

            yield return StartCoroutine(MoveTo(endPosition, movingTime)); //start one movement

            //if card is enemy break movement
            if (cards[i] is EnemyCard && !((EnemyCard) cards[i]).IsDefeated())
            {
                FightEnemy?.Invoke((EnemyCard)cards[i]);
                break;
            }
        }
        for (int i = 0; i < cards.Count; i++) {
            cards[i].gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
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
        this.enabled = false;
        OnDeath?.Invoke();
    }
}
