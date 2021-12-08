using System.Collections;
using System.Collections.Generic;
using Cards;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoUI : MonoBehaviour
{
    [SerializeField] private List<Button> actionButtons;
    [SerializeField] private Text cardName;

    public void DisplayCardInfo(Card card)
    {
        if(card is EmptyCard)
        {
            gameObject.SetActive(false);
        }
        if (card is ResourceCard)
        {
            foreach (var button in actionButtons)
            {
                button.gameObject.SetActive(true);
                button.interactable = !(card.GetCurrentUnit() is null);
            }
        }
        else
        {
            foreach (var button in actionButtons)
            {
                button.gameObject.SetActive(false);
            }
        }
        cardName.text = card.ToString();
    }
}
