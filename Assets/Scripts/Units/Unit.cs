using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Func<AllegianceType> GetAllegiance;
    [SerializeField] int moveEnergy;
    [SerializeField] int captureEnergy;
    [SerializeField] int force;
    [SerializeField] int fightEnergy;
    [SerializeField] int level;
    [SerializeField] int resourceEnergy;
    [SerializeField] double increaseCoef;
    [SerializeField] double decreaseCoef;
    [SerializeField] int sight;

    // ��������� ������, ���������� ���� � ���������� �������
    public void IncreaseLevel()
    {
        level += 1;
        force = (int)(force * increaseCoef);
        moveEnergy = (int)(moveEnergy * decreaseCoef);
        captureEnergy = (int)(captureEnergy * decreaseCoef);
        fightEnergy = (int)(fightEnergy * decreaseCoef);
        resourceEnergy = (int)(resourceEnergy * decreaseCoef);
    }

    // �������� � �����.
    // mobForce - ���� ����
    // allEnergy - ���������� ������� ������
    // levelupMob - ����� �� �������� ���� �������� ������� �����
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
            //todo �������� �����, ����� ������ �������

            if (levelupMob)
                IncreaseLevel();
        }
        else
        {
            Death();
        }
    }

    // ������ ����� �����
    public void CaptureCard(ref int allEnergy)
    {
        if (captureEnergy > allEnergy)
            return;

        allEnergy -= captureEnergy;
        // todo �������� �����, ��� ��� ���������
    }

    public void GetResource(ref int allEnergy)
    {
        if (resourceEnergy > allEnergy)
            return;

        allEnergy -= resourceEnergy;
        // todo �������� ��� ������� � �������� ������ � ����� �������
        // todo �������� �����, ����� ������ ������
    }

    private void Death() 
    {
        //todo
    }
}
