using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MessageLogUI : MonoBehaviour
    {
        [SerializeField] private GameObject messageContainer;
        [SerializeField] private GameObject messageItem;
        private List<string> _systemMessages = new List<string>();

        public void Refresh()
        {
        
        }

        public void AddMessage(string message)
        {
            _systemMessages.Add(message);
            GameObject newMessage = Instantiate(messageItem, messageContainer.transform);
            newMessage.GetComponent<Text>().text = message;
        }
    }
}
