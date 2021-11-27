using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CountDown : NetworkBehaviour
{
    //todo add ability to stop timer

    // Вызывается каждую секунду, если таймер запущен
    public Action<int> UpdateTimer;
    public Action TimerOver;
    Coroutine timerCoroutine;

    public void StartTimer(int timeToWait)
    {
        if (IsServer)
        {
            StartCoro(timeToWait);
        }
    }
    
    private IEnumerator WaitAndUpdateTimer(int timeToWait, int secondsAfterTimeOver = 0)
    {
        int counter = timeToWait;
        UpdateTimeClientRpc(counter);
        while (counter >= 0) {
            yield return new WaitForSeconds (1);
            counter--;
            UpdateTimeClientRpc(counter);
        }
        UpdateTimeClientRpc(0);
        yield return new WaitForSeconds (secondsAfterTimeOver);
        TimerOver?.Invoke();
    }
    
    private void StartCoro(int timeToWait)
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(WaitAndUpdateTimer(timeToWait));
    }
    
    [ClientRpc]
    private void UpdateTimeClientRpc(int timeLeft)
    {
        UpdateTimer?.Invoke(timeLeft);
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
    }
}
