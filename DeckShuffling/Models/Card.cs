using System;

namespace DeckShuffling.Models
{
    public enum CardSuits
    {
        Diamonds = 'd',
        Hearts = 'h',
        Clubs = 'c',
        Spades = 's'
    }

    public enum CardValues
    {
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
        
    }
    
    public class Card : IComparable<Card>
    {
        public string Value
        {
            get
            {
                if (_cardValue >= CardValues.Two && _cardValue <= CardValues.Ten)
                    return ((int) _cardValue).ToString();
                
                return _cardValue.ToString();
            }
        }

        private readonly CardValues _cardValue;
        
        public string Suit => _cardSuit.ToString();

        private readonly CardSuits _cardSuit;

        public Card(CardValues cardValue, CardSuits suit)
        {
            _cardValue = cardValue;
            _cardSuit = suit;
        }


        public int CompareTo(Card other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            
            var suitsAreEqual = _cardSuit == other._cardSuit;
            if (!suitsAreEqual) return _cardSuit > other._cardSuit ? 1 : -1;
            
            return _cardValue.CompareTo(other._cardValue);
        }
    }
}