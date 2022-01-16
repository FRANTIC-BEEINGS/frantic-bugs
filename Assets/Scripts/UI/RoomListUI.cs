using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace UI
{
    public class RoomListUI : MonoBehaviourPunCallbacks
    {
        [SerializeField] private RoomItemUI roomItemPrefab;
        private List<RoomItemUI> _roomItemsList;
        public Transform container;

        private Coroutine _updateRoomListCoro;
        
        private void Awake()
        {
            _roomItemsList = new List<RoomItemUI>();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            _updateRoomListCoro = StartCoroutine(IEUpdateRoomList(roomList));
        }

        private IEnumerator IEUpdateRoomList(List<RoomInfo> roomList)
        {
            yield return new WaitForSeconds(1);
            foreach (var item in _roomItemsList)
            {
                Destroy(item);
            }
            _roomItemsList.Clear();

            foreach (var room in roomList)
            {
                RoomItemUI newRoomItem = Instantiate(roomItemPrefab, container);
                newRoomItem.SetRoomName(room.Name);
                newRoomItem.SetPlayerCount(room.PlayerCount);
                _roomItemsList.Add(newRoomItem);
            }
        }
        
        private void UpdateRoomList(List<RoomInfo> roomList)
        {
            foreach (var item in _roomItemsList)
            {
                Destroy(item);
            }
            _roomItemsList.Clear();

            foreach (var room in roomList)
            {
                RoomItemUI newRoomItem = Instantiate(roomItemPrefab, container);
                newRoomItem.SetRoomName(room.Name);
                newRoomItem.SetPlayerCount(room.PlayerCount);
                _roomItemsList.Add(newRoomItem);
            }
        }
    }
}
