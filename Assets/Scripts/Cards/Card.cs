using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cards
{
    public class Card : MonoBehaviour
    {
        private bool _isCaptured;
        private bool _isVisible = true;
        [SerializeField] protected Sprite FaceSprite;
        protected ulong CaptorId;
        private Unit _currentUnit;
        private Coroutine _rotateCard;
        private float _rotationTime = 0.5f;
        public bool isTreeVisible;  //whether tree gives vision on the card
        
        public Unit GetCurrentUnit()
        {
            return _currentUnit;
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

            _currentUnit = unit;
            unit.transform.parent = this.transform; //change parent of the unit in the hierarchy (check later)
            return true;
        }

        public void LeaveCard()
        {
            _currentUnit = null;
        }

        private bool CanStepOn(Unit unit)
        {
            return _currentUnit == null || unit.Allegiance == _currentUnit.Allegiance;
        }

        //play flipping animation and change visibility value
        private void UpdateCardView(bool visibility)
        {
            if (visibility == _isVisible) return;
            //todo: debugging?
            if(_rotateCard!=null)
                StopCoroutine(_rotateCard);
            _rotateCard = StartCoroutine(RotateCardY(Quaternion.Euler(transform.eulerAngles + 180f * Vector3.up), _rotationTime));
            _isVisible = visibility;
        }

        IEnumerator RotateCardY(Quaternion endValue, float duration)
        {
            Debug.Log("rotating");
            float time = 0;
            Quaternion startValue = transform.rotation;

            while (time < duration)
            {
                transform.rotation = Quaternion.Lerp(startValue, endValue, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            transform.rotation = endValue;
        }
    }
}
