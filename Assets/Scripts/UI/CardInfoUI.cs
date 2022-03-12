using System.Collections;
using System.Collections.Generic;
using Cards;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoUI : MonoBehaviour
{
    [SerializeField] private Button actionButton;
    [SerializeField] private Text cardName;
    [SerializeField] private Text buttonText;

    public void DisplayCardInfo(Card card)
    {
        if (card is ResourceCard resourceCard)
        {
            actionButton.gameObject.SetActive(!resourceCard.ResourceCollected);
            actionButton.interactable = !(resourceCard.GetCurrentUnit() is null);
            buttonText.text = resourceCard.GetCollectButtonText();
        }
        else
        {
            actionButton.gameObject.SetActive(false);
        }
        cardName.text = card.ToString();
    }
}
