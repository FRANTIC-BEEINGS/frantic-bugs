using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cards
{
    public class Card : MonoBehaviour
    {
        private AnimationCurve RotationCurve;
        private AnimationCurve JumpCurve;
        private Quaternion _openCard;
        private Quaternion _closedCard;
        private Vector3 _downPosition;
        private Vector3 _upPosition;
        private float _jumpHeight;

        private void Start ()
        {
            _jumpHeight = 1.5f;
            RotationCurve = gameObject.transform.GetChild(0).gameObject.GetComponent<BodyInformation>().GetRotationCurve();
            JumpCurve = gameObject.transform.GetChild(0).gameObject.GetComponent<BodyInformation>().GetJumpCurve();
            _openCard = Quaternion.Euler(new Vector3(0, 0, 0));
            _closedCard = Quaternion.Euler(new Vector3(0, 180f, 0));
            _downPosition = transform.position;
            _upPosition = _downPosition;
            _upPosition.z -= _jumpHeight;
            this.IsVisible = false;
        }

        private bool _isCaptured;
        private bool _isVisible = true;
        [SerializeField] protected Sprite FaceSprite;
        protected ulong CaptorId;
        private Unit _currentUnit;
        private Coroutine _rotateCard;
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
            if (visibility == true)
            {
                _rotateCard = StartCoroutine(RotateCardY(_openCard, Constants.STEP_DURATION));
            }
            else
            {
                _rotateCard = StartCoroutine(RotateCardY(_closedCard, Constants.STEP_DURATION));
            }
            _isVisible = visibility;
        }

        IEnumerator RotateCardY(Quaternion endRotationValue, float duration)
        {
            float time = 0;
            Quaternion startRotationValue = transform.rotation;
            Vector3 startJumpValue = transform.position;

            while (time < duration)
            {
                transform.rotation = Quaternion.Lerp(startRotationValue, endRotationValue, RotationCurve.Evaluate(time / duration));
                transform.position = Vector3.Lerp(startJumpValue, _upPosition, JumpCurve.Evaluate(time / duration));
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = _downPosition;
        }
    }
}
