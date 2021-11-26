using UnityEngine;
using UnityEngine.Serialization;

namespace Cards
{
    public class Card : MonoBehaviour
    {
        private bool _isCaptured;
        private bool _isVisible = false;
        [SerializeField] protected Sprite FaceSprite;
        protected ulong CaptorId;
        private Unit _currentUnit;
        [SerializeField] public ulong id;

        //get/set card visibility (also calls method for flipping card on visibility change)
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                UpdateCardView(value);
                _isVisible = value;
            }
        }

        public bool IsCaptured
        {
            get => _isCaptured;
            set
            {
                _isCaptured = value;
                if (value)
                    IsVisible = true;
                //todo:add handling of situation when other player captures the card
            }
        }

        public void Initialize(Sprite face)
        {
            this.FaceSprite = face;
        }

        public bool StepOn(Unit unit)
        {
            if(!CanStepOn(unit)) return false;
            if (_currentUnit != null)
            {
                //fight with enemy unit
                //TODO:update current unit
            }
            unit.transform.parent = this.transform; //change parent of the unit in the hierarchy (check later)
            return true;
        }

        private bool CanStepOn(Unit unit)
        {
            return _currentUnit == null || unit.GetAllegiance() == _currentUnit.GetAllegiance();
        }

        //play flipping animation and change visibility value
        private void UpdateCardView(bool visibility)
        {
            if (visibility == _isVisible) return;
            //TODO: play animation
            _isVisible = visibility;
        }
    }
}
