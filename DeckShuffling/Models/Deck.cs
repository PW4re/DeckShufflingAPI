using System;
using System.Collections.Generic;
using System.Linq;

namespace DeckShuffling.Models
{
    public class Deck
    {
        public string Name { get; }

        public List<Card> Cards { get; set; }

        public Deck(string name, SortedSet<CardValues> possibleCardValues)
        {
            Name = name;
            Cards = GenerateCardList(possibleCardValues);
        }

        public Deck(string name, CardValues from, CardValues to)
        {
            Name = name;
            if (from > to && to != CardValues.Ace)
                throw new ArgumentException("Parameter 'to' must be greater than 'from' or equal to it.");
            Cards = GenerateCardList(
                new SortedSet<CardValues>(((CardValues[]) Enum.GetValues(typeof(CardValues)))
                    .Where(
                        cVal => to != CardValues.Ace && cVal >= from && cVal <= to || 
                                cVal >= from || cVal == to
                        )
                    )
                );
        }

        private List<Card> GenerateCardList(SortedSet<CardValues> possibleCardValues)
        {
            var result = new List<Card>();

            foreach (CardSuits suit in Enum.GetValues(typeof(CardSuits)))
            {
                foreach (var cardValue in possibleCardValues)
                {
                    result.Add(new Card(cardValue, suit));
                }
            }

            return result;
        }
    }
}