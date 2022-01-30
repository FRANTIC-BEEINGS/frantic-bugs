using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace UI
{
    public class ShowNickname : MonoBehaviour
    {
        void Awake()
        {
            GetComponent<Text>().text = PhotonNetwork.NickName;
        }
    }
}
