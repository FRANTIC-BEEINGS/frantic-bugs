using System.Collections.Generic;
using UnityEngine;
using Cards;

public class VisionController : MonoBehaviour
{
        [SerializeField] private MapGeneration m;
        List<List<Card>> map;
        //[SerializeField] Card firstCard;

        public void Initialize(MapGeneration mapGeneration)
        {
                m = mapGeneration;
                map = m.GetMap();
        }

        //first card - seed
        //previous card - where unit was before the step
        public void OpenCardsInUnitVision(int vision, Card firstCard, Card previousCard)
        {
                HashSet<Card> closeCards = GetCardsInVision(vision, previousCard);
                HashSet<Card> openCards = GetCardsInVision(vision, firstCard);

                foreach (Card card in openCards)
                {
                        closeCards.Remove(card);
                }

                // open cards in new vision
                foreach (Card card in openCards)
                {
                        if (!card.IsVisible)
                        {
                                card.IsVisible = true;
                        }
                }

                // close cards in previous vision and not in new vision
                foreach (Card card in closeCards)
                {
                        if (card.IsVisible && !card.isTreeVisible)
                        {
                                card.IsVisible = false;
                        }
                }
        }

        public void OpenCardsInTreeVision(int vision, Card firstCard)
        {
                HashSet<Card> openCards = GetCardsInVision(vision, firstCard);
                foreach (Card card in openCards)
                {
                        if (!card.IsVisible)
                        {
                                card.IsVisible = true;
                        }
                        card.isTreeVisible = true;
                }
        }

        // get list of cards in vision
        public HashSet<Card> GetCardsInVision(int vision, Card firstCard)
        {
                Vector2 card = FindCardOnMap(firstCard);
                int x = (int) card.x;
                int y = (int) card.y;
                HashSet<Card> cards = new HashSet<Card>();

                for (int i = 0; i <= vision; i++)
                {
                        for (int j = 0; j <= vision - i; j++)
                        {
                                if (x + i < map.Count && y + j < map[x + i].Count)
                                        cards.Add(map[x + i][y + j]);
                                if (x + i < map.Count && y - j >= 0)
                                        cards.Add(map[x + i][y - j]);
                                if (x - i >= 0 && y + j < map[x - i].Count)
                                        cards.Add(map[x - i][y + j]);
                                if (x - i >= 0 && y - j >= 0)
                                        cards.Add(map[x - i][y - j]);
                        }
                }

                return cards;
        }

        // return coordinates of card in map
        private Vector2 FindCardOnMap(Card firstCard)
        {
                int x = 0;
                int y = 0;
                for (int i = 0; i < map.Count; i++)
                {
                        y = map[i].IndexOf(firstCard);

                        if (y > -1)
                        {
                                x = i;
                                break;
                        }
                }

                return new Vector2(x, y);
        }
}
