using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DeckShuffling.Models
{
    public static class DeckRepository
    {
        private static readonly ConcurrentDictionary<string, Deck> Decks;
        
        static DeckRepository()
        {
            // Создадим для примера колоду из 52 карт:
            var sampleDeck = new Deck("Sample deck", CardValues.Ace, CardValues.King);
            Decks = new ConcurrentDictionary<string, Deck> {[sampleDeck.Name] = sampleDeck};
        }

        public static Deck GetDeckByName(string deckName)
            => Decks.ContainsKey(deckName) ? Decks[deckName] : null;
        

        public static void AddDeck(Deck newDeck)
        {
            Decks[newDeck.Name] = newDeck;
        }

        public static bool DeleteDeck(string deckName)
        {
            if (!Decks.ContainsKey(deckName)) return false;
            Decks.Remove(deckName, out _);

            return true;
        }

        public static List<string> GetDeckNameList() => new List<string>(Decks.Keys);

        public static void ShuffleDeck(string deckName, Func<List<Card>, List<Card>> shuffleMethod)
        {
            if (!Decks.ContainsKey (deckName)) return;
            var cards = Decks[deckName].Cards;
            
            Decks[deckName].Cards = shuffleMethod(cards);
        }
    }
}