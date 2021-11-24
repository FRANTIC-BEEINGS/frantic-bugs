using UnityEngine;

namespace Cards
{
    public class Card : MonoBehaviour
    {
        private bool _isVisible = false;
        protected Sprite Face;
        private Unit currentUnit;

        public void Initialize(Sprite face)
        {
            Face = face;
        }
    
        public bool StepOn(Unit unit)
        {
            if(!CanStepOn(unit)) return false;
            if (currentUnit != null)
            {
                //fight with enemy unit
                //TODO:update current unit
            }
            unit.transform.parent = this.transform; //change parent of the unit in the hierarchy (check later)
            return true;
        }

        private bool CanStepOn(Unit unit)
        {
            return currentUnit == null || unit.GetAllegiance() == currentUnit.GetAllegiance();
        }
    
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

        //play flipping animation and change visibility value
        private void UpdateCardView(bool visibility)
        {
            if (visibility == _isVisible) return;
            //TODO: play animation
            _isVisible = visibility;
        }

        //assign card back image
        private void Start()
        {
        }
    }
}
