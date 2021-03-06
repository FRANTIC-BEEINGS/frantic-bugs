using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using GameLogic;
using Photon.Pun;
using ResourceManagment;
using UnityEngine;

public class Unit : MonoBehaviour, IPunObservable
{
    [SerializeField] private AnimationCurve MoveCurve;
    [SerializeField] private AnimationCurve JumpCurve;
    [SerializeField] private float _jumpHeight;
    private Quaternion _upDirection;
    private Quaternion _downDirection;
    private Quaternion _leftDirection;
    private Quaternion _rightDirection;

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
    private Quaternion endRotation;
    private bool isStopMovement = false;
    public AllegianceType Allegiance = AllegianceType.A;
    private int experience = 0;
    [SerializeField] private int experienceLimit;
    public Action<Card> FinishedMovement;
    public Action<EnemyCard> FightEnemy;
    public Action OnDeath;
    public Action<int> OnLevelChange;
    private bool initialized;

    private GameController _gameController;

    public int Level
    {
        get => level;
        set
        {
            if (!PhotonNetwork.IsMasterClient)
                photonView.RPC("SetLevel", RpcTarget.MasterClient, value);
            level = value;
        }
    }
    public VisionController visionController;

    private PhotonView photonView;

    private void Start()
    {
        visionController = GetComponent<VisionController>();
        _upDirection = Quaternion.Euler(new Vector3(0, 0, 0));
        _downDirection = Quaternion.Euler(new Vector3(0, 0, 180));
        _leftDirection = Quaternion.Euler(new Vector3(0, 0, 90));
        _rightDirection = Quaternion.Euler(new Vector3(0, 0, 270));
        photonView = GetComponent<PhotonView>();
        if (photonView) photonView.ObservedComponents.Add(this);
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameLogic.GameController>();
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
        get => (int)(Level * forceCoef);
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
        Level += 1;
        OnLevelChange?.Invoke(Level);
        forceCoef = (int)(forceCoef * increaseCoef);
        moveEnergy = (int)(moveEnergy * decreaseCoef)>0?(int)(moveEnergy * decreaseCoef):1;
        captureEnergy = (int)(captureEnergy * decreaseCoef)>0?(int)(captureEnergy * decreaseCoef):1;
        fightEnergy = (int)(fightEnergy * decreaseCoef)>0?(int)(fightEnergy * decreaseCoef):1;
        resourceEnergy = (int)(resourceEnergy * decreaseCoef)>0?(int)(resourceEnergy * decreaseCoef):1;
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
        if (!photonView.IsMine)
            return;
        Debug.Log("MoveAlongPath" + resourceManager.GetResource(ResourceType.Energy));
        isStopMovement = false;
        StartCoroutine(Move(cards, resourceManager));
    }

    IEnumerator Move(List<Card> cards, ResourceManager resourceManager)
    {
        Card lastCard = null;
        for(int i = 1; i < cards.Count; i++)
        {
            lastCard = cards[i];
            // check if we can step on next card and if player want stop
            if (isStopMovement || resourceManager.GetResource(ResourceType.Energy) < moveEnergy || !cards[i].StepOn(this))
            {
                lastCard = cards[i-1];
                break;
                //todo
            }

            endPosition = cards[i].gameObject.transform.position; // find destination position
            resourceManager.AddResource(ResourceType.Energy, -moveEnergy);
            cards[i - 1].LeaveCard();

            visionController.OpenCardsInUnitVision(vision, cards[i], cards[i - 1]);

            // temporary crutch
            cards[i - 1].gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            //BI.SetHighlight(false);

            float _dx = endPosition.x - transform.position.x;
            float _dy = endPosition.y - transform.position.y;
            if (Math.Abs(_dx) > Math.Abs(_dy))
                if (_dx > 0)
                    endRotation = _rightDirection;
                else
                    endRotation = _leftDirection;
            else
                if (_dy > 0)
                    endRotation = _upDirection;
                else
                    endRotation = _downDirection;

            yield return StartCoroutine(MoveTo(endPosition, endRotation, Constants.STEP_DURATION)); //start one movement

            //if card is enemy break movement
            if (cards[i] is EnemyCard && !((EnemyCard) cards[i]).IsDefeated)
            {
                FightEnemy?.Invoke((EnemyCard)cards[i]);
                break;
                //todo
            }
        }
        for (int i = 0; i < cards.Count; i++) {
            cards[i].gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
        isStopMovement = false;
        FinishedMovement?.Invoke(lastCard);
        //
    }

    IEnumerator MoveTo(Vector3 endPosition, Quaternion endRotation, float duration)
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
        Quaternion startRotation = transform.rotation;

        //in every moment move to destination
        while(time < duration)
        {
            currentPosition = Vector3.Lerp(startXYPosition, endPosition, MoveCurve.Evaluate(time / duration));
            currentPosition += Vector3.Lerp(startZPosition, endZPosition, JumpCurve.Evaluate(time / duration));
            transform.position = currentPosition;
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, (float)Math.Pow(time / duration, 0.3f));

            time += Time.deltaTime;
            yield return null;
            transform.position = currentPosition;
        }
        transform.position = endPosition;
        transform.rotation = endRotation;
    }

    public void Death()
    {
        photonView.RPC("LossRpc", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
    }
    
    public void KillEnemyUnit()
    {
        photonView.RPC("WinRpc", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    #region PunCallbacks

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo messageInfo)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(level);
        }
        else if (stream.IsReading)
        {
            level = (int) stream.ReceiveNext();
        }
    }

    #endregion

    #region RPCs

    [PunRPC]
    protected void SetLevel(int value)
    {
        level = value;
    }

    [PunRPC]
    protected void LossRpc(Int32 playerId)
    {
        _gameController.Loss(playerId);
    }
    
    [PunRPC]
    protected void WinRpc(Int32 playerId)
    {
        _gameController.Win(playerId);
    }

    #endregion
}
