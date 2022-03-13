using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Units
{
    public class UnitsStorage : MonoBehaviour, IPunObservable
    {
        private List<Unit> unitIdList;

        private void Start()
        {
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            
        }

        public void addUnitIdToList(Unit unit)
        {
            if (unitIdList.Contains(unit))
                return;
            unitIdList.Add(unit);
        }
        
        
        
    }
}