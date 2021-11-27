using System.Collections.Generic;
using ResourceManagment;
using UnityEngine;

namespace Cards
{
    public class EnemyCard : Card
    {
        [SerializeField] private int _level;
        [SerializeField] private int _experience;
        [SerializeField] private Dictionary<ResourceType, int> _resources;

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
            
        }
    }
}