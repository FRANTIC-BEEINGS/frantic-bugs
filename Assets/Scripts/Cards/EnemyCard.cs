using System.Collections.Generic;
using Photon.Pun;
using ResourceManagment;
using UnityEngine;

namespace Cards
{
    public class EnemyCard : Card
    {
        [SerializeField] private string name = "Enemy";
        [SerializeField] private int _level;
        [SerializeField] private int _experience;
        [SerializeField] private Dictionary<ResourceType, int> _resources;
        private bool _isDefeated = false;

        public bool IsDefeated()
        {
            return _isDefeated;
        }

        public void Initialize(Sprite face, int level, int experience, Dictionary<ResourceType, int> resources)
        {
            base.FaceSprite = face;
            _level = level;
            _experience = experience;
            _resources = resources;
        }
        
        //use this in generation for setting default parameters
        public void Initialize(int level, int experience, Dictionary<ResourceType, int> resources)
        {
            _level = level;
            _experience = experience;
            _resources = resources;
        }

        public int GetExpReward()
        {
            return _experience;
        }

        public Dictionary<ResourceType, int> GetResourceReward()
        {
            return _resources;
        }

        public void OnDeath()
        {
            _isDefeated = true;
        }

        public int GetLevel()
        {
            return _level;
        }

        public override string ToString()
        {
            return name + " | Level " + _level;
        }
        
    }
}