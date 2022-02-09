using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using ResourceManagment;
using UnityEngine;
using UnityEngine.Serialization;

public class Unit : MonoBehaviour
{
    [SerializeField] private AnimationCurve MoveCurve;
    [SerializeField] private AnimationCurve JumpCurve;
    [SerializeField] private float _jumpHeight;

    [SerializeField] int moveEnergy;
    [SerializeField] int captureEnergy;
    [SerializeField] double forceCoef;
    [SerializeField] int fightEnergy;
    [SerializeField] int level = 1;
    [SerializeField] int resourceEnergy;
    [SerializeField] double increaseCoef;
    [SerializeField] double decreaseCoef;
    [SerializeField] int vision;
    public int Vision => vision;
    [SerializeField] private Sprite sprite;
    private Vector3 endPosition;
    private bool isStopMovement = false;
    public AllegianceType Allegiance = AllegianceType.A;
    private int experience = 0;
    [SerializeField] private int experienceLimit;
    public Action FinishedMovement;
    public Action<EnemyCard> FightEnemy;
    public Action OnDeath;
    public Action<int> OnLevelChange;
    private bool initialized;

    public VisionController visionController;

    private void Start()
    {
        visionController = GetComponent<VisionController>();
    }

    public void Initialize(MapGeneration mapGeneration)
    {
        if (initialized)
            return;
        visionController.Initialize(mapGeneration);
        initialized = true;
    }

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

            visionController.OpenCardsInUnitVision(vision, cards[i], cards[i - 1]);

            // temporary crutch
            cards[i - 1].gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            //BI.SetHighlight(false);

            yield return StartCoroutine(MoveTo(endPosition, Constants.STEP_DURATION)); //start one movement

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

    IEnumerator MoveTo(Vector3 endPosition, float duration)
    {
        float time = 0;
        Vector3 currentPosition = transform.position;
        Vector3 startXYPosition = transform.position;
        startXYPosition.z = 0;
        Vector3 startZPosition = transform.position;
        startZPosition.x = 0;
        startZPosition.y = 0;
        Vector3 endZPosition = startZPosition;
        endZPosition.z -= _jumpHeight;

        //in every moment move to destination
        while(time < duration)
        {
            currentPosition = Vector3.Lerp(startXYPosition, endPosition, MoveCurve.Evaluate(time / duration));
            currentPosition += Vector3.Lerp(startZPosition, endZPosition, JumpCurve.Evaluate(time / duration));
            transform.position = currentPosition;
            time += Time.deltaTime;
            yield return null;
            transform.position = currentPosition;
        }
        transform.position = endPosition;
    }

    public void Death()
    {
        this.enabled = false;
        OnDeath?.Invoke();
    }
}
