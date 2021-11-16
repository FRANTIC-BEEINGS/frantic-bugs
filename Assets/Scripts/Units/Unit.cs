using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] int moveEnergy;
    [SerializeField] int captureEnergy;
    [SerializeField] int force;
    [SerializeField] int fightEnergy;
    [SerializeField] int level;
    [SerializeField] int resourceEnergy;
    [SerializeField] double increaseCoef;
    [SerializeField] double decreaseCoef;
    [SerializeField] int sight;

    // ѕовышение уровн€, увеличение силы и уменьшение энергий
    public void IncreaseLevel()
    {
        level += 1;
        force = (int)(force * increaseCoef);
        moveEnergy = (int)(moveEnergy * decreaseCoef);
        captureEnergy = (int)(captureEnergy * decreaseCoef);
        fightEnergy = (int)(fightEnergy * decreaseCoef);
        resourceEnergy = (int)(resourceEnergy * decreaseCoef);
    }

    // —ражение с мобом.
    // mobForce - сила моба
    // allEnergy - глобальна€ энерги€ игрока
    // levelupMob - будет ли убийство моба повышать уровень герою
    public void FightMob(int mobForce, ref int allEnergy, bool levelupMob)
    {
        if (mobForce < force)
        {
            allEnergy -= fightEnergy;
            if (allEnergy < 0)
            {
                Death();
                return;
            }
            //todo передать карте, чтобы стерла монстра

            if (levelupMob)
                IncreaseLevel();
        }
        else
        {
            Death();
        }
    }

    // «ахват одной карты
    public void CaptureCard(ref int allEnergy)
    {
        if (captureEnergy > allEnergy)
            return;

        allEnergy -= captureEnergy;
        // todo передать карте, что она захвачена
    }

    public void GetResource(ref int allEnergy)
    {
        if (resourceEnergy > allEnergy)
            return;

        allEnergy -= resourceEnergy;
        // todo получить тип ресурса и добавить ресурс в общие ресурсы
        // todo передать карте, чтобы стерла ресурс
    }

    private void Death() 
    {
        //todo
    }
}
