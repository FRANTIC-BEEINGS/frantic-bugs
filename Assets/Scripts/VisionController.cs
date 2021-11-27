using System.Collections.Generic;
using UnityEngine;
using Cards;

public class VisionController : MonoBehaviour
{
        [SerializeField] private MapGeneration m;
        List<List<Card>> map;
        [SerializeField] Card firstCard;

        public void OpenCardsInVision(int vision/*, Card firstCard*/)
        {
                UpdateCardsInVision(vision, true);
        }
        
        public void CloseCardsInVision(int vision/*, Card firstCard*/)
        {
                UpdateCardsInVision(vision, false);
        }
        
        private void UpdateCardsInVision(int vision, bool open/*, Card firstCard*/)
        {
                int x = 0;
                int y = 0;
                for (int i = 0; i < map.Count; i++)
                {
                        y = map[i].IndexOf(firstCard);
                        
                        if (y > -1)
                        {
                                x = i;
                                Debug.Log(x.ToString()+y.ToString());
                                break;
                        }
                        
                }

                for (int i = 0; i <= vision; i++)
                {
                        for (int j = 0; j <= vision - i; j++)
                        {
                                if (x + i < map.Count && y + j < map[x + i].Count && map[x + i][y + j].IsVisible != open)
                                        map[x + i][y + j].IsVisible = open;
                                if (x + i < map.Count && y - j >= 0 && map[x + i][y - j].IsVisible != open)
                                        map[x + i][y - j].IsVisible = open;
                                if (x - i >= 0 && y + j < map[x - i].Count && map[x - i][y + j].IsVisible != open)
                                        map[x - i][y + j].IsVisible = open;
                                if (x - i >= 0 && y - j >= 0 && map[x - i][y - j].IsVisible != open)
                                        map[x - i][y - j].IsVisible = open;
                        }
                }
        }
}
