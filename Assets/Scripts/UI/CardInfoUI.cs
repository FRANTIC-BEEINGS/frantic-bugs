using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using GameLogic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoUI : MonoBehaviour
{
    [SerializeField] private Button actionButton;
    [SerializeField] private Text cardName;
    [SerializeField] private Text buttonText;

    public static Func<ResourceCard,bool> CheckEnergy;

    public void DisplayCardInfo(Card card)
    {
        if(card.GetCurrentUnit() == null && (card is EmptyCard || card is SpawnerCard))
        {
            gameObject.SetActive(false);
        }

        if (card.GetCurrentUnit() != null && !card.GetCurrentUnit().GetComponent<PhotonView>().IsMine)
        {
            cardName.text = "Unit | Level " + card.GetCurrentUnit().Level;
            actionButton.gameObject.SetActive(false);
            return;
        }
        
        if (card is ResourceCard resourceCard)
        {
            actionButton.gameObject.SetActive(!resourceCard.ResourceCollected);
            actionButton.interactable = false;
            if (resourceCard.GetCurrentUnit() != null)
            {
                if (CheckEnergy(resourceCard))
                    actionButton.interactable = true;
            }
            buttonText.text = resourceCard.GetCollectButtonText();
        }
        else
        {
            actionButton.gameObject.SetActive(false);
        }
        cardName.text = card.ToString();
    }
}
