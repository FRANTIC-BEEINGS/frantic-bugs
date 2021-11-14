using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : Unit
{
    int fightEnergy;

    public void Initialize()
    {
        moveEnergy = 5;
        force = 2;
        fightEnergy = 7;
    }
}
