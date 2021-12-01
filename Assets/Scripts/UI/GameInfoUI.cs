using System.Collections;
using System.Collections.Generic;
using Cards;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoUI : MonoBehaviour
{
    [SerializeField] private Text level;
    [SerializeField] private Text foodGoal;
    [SerializeField] private Text moneyGoal;

    public void UpdateLevel(int newLevel)
    {
        level.text = newLevel.ToString();
    }

    public void SetGameGoals(int newFoodGoal, int newMoneyGoal)
    {
        foodGoal.text = newFoodGoal.ToString();
        moneyGoal.text = newMoneyGoal.ToString();
    }
}
