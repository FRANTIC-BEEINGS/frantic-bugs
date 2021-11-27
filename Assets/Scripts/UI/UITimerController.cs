using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimerController : MonoBehaviour
{
    public bool showHoursIfZero;
    public bool showMinutesIfZero;
    public bool showOnlyInSeconds;
    public bool showInMinutesIfNonZeroElseInSeconds;
    public bool addZerosIfLengthLessTwo;
    public string timeSeparator = ":";
    
    private Text timerText;
    private CountDown timer;
    void Awake()
    {
        timer = GetComponent<CountDown>();
        timerText = GetComponent<Text>();
        timer.UpdateTimer += UpdateTime;
    }

    private void UpdateTime(int timeInSeconds)
    {
        if (showOnlyInSeconds)
        {
            // time in seconds
            timerText.text = timeInSeconds.ToString();
            return;
        }
        
        if (showInMinutesIfNonZeroElseInSeconds)
        {
            int minutesLeft = timeInSeconds / 60;
            if (minutesLeft > 0)
            {
                timerText.text = minutesLeft.ToString();
                return;
            }
            timerText.text = timeInSeconds.ToString();
            return;
        }
        
        int seconds = timeInSeconds % 60;
        int minutes = ((timeInSeconds - seconds) % 3600) / 60;
        int hours = timeInSeconds / 3600;
        UpdateTime(hours, minutes, seconds);
    }

    private void UpdateTime(int hours, int minutes, int seconds)
    {
        if (showHoursIfZero)
        {
            // hh:mm:ss or 00:mm:ss or 00:00:ss
            timerText.text = timeToString(hours) + timeSeparator + timeToString(minutes) + timeSeparator + timeToString(seconds);
            return;
        }

        if (showMinutesIfZero)
        {
            if (hours != 0)
            {
                //hh:mm:ss
                timerText.text = timeToString(hours) + timeSeparator + timeToString(minutes) + timeSeparator + timeToString(seconds);
                return;
            }
            //mm:ss or 00:ss
            timerText.text = timeToString(minutes) + timeSeparator + timeToString(seconds);
            return;
        }
        if (hours != 0)
        {
            //hh:mm:ss
            timerText.text = timeToString(hours) + timeSeparator + timeToString(minutes) + timeSeparator + timeToString(seconds);
            return;
        }
        
        if (minutes != 0)
        {
            // mm:ss
            timerText.text = timeToString(minutes) + timeSeparator + timeToString(seconds);
            return;
        }
        // ss
        timerText.text = timeToString(seconds);
        return;
    }

    private string timeToString(int time, int minLength = 2)
    {
        string timeString = time.ToString();
        if (addZerosIfLengthLessTwo)
        {
            int delta = minLength - timeString.Length;
            if (delta > 0)
            {
                string prefix = "";
                for (int i = 0; i < delta; i++)
                {
                    prefix += "0";
                }

                timeString = prefix + timeString;
            }
        }

        return timeString;
    }
}
