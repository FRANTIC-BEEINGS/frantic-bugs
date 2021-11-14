using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int moveEnergy;
    int captureEnergy;
    public int force;

    public void Initialize()
    {
        captureEnergy = 10;
    }

}
