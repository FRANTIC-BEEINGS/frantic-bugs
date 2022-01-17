using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace UI
{
    public class RecordTextInput : MonoBehaviour
    {
        [SerializeField] private Text _inputTextField;
        [SerializeField] private Text _displayText;
        private string _text;

        
        
        public void DisplayText()
        {
            _text = _inputTextField.text;
            _displayText.text = _text;
        }

        public void RecordPlayerName()
        {
            DisplayText();
            //todo: record stuff in game controller or on server or smth
        }
    
        public void RecordRoomName()
        {
            DisplayText();
            //todo: record stuff in game controller or on server or smth
        }
    }
}
