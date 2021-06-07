using System;
using System.Collections.Generic;
using System.Linq;

namespace DeckShuffling.Models
{
    public static class Shuffler
    {
        public static List<T> SimpleShuffle<T>(IList<T> items)
        {
            var rnd = new Random();

            return items.OrderBy(_ => rnd.Next(1_000_000)).ToList();
        }

        public static List<T> PokerStyleShuffle<T>(IList<T> items)
        {
            return DoPokerShuffle(DoPokerShuffle(DoPokerShuffle(items)));
        }

        private static List<T> DoPokerShuffle<T>(IList<T> items)
        {
            var accumulator = new List<T>();
            var leftPart = new Stack<T>();
            var rightPart = new Stack<T>();
            var centralIndex = items.Count / 2;
            for (var i = 0; i < centralIndex; i++)
                leftPart.Push(items[i]);
            for (var i = items.Count - 1; i >= centralIndex; i--)
                rightPart.Push(items[i]);
            var rnd = new Random();

            while (leftPart.Count > 0 || rightPart.Count > 0)
            {
                AddToAccumulator(leftPart, accumulator, rnd);
                AddToAccumulator(rightPart, accumulator, rnd);
            }

            return accumulator;
        }

        private static void AddToAccumulator<T>(Stack<T> deckPart, List<T> deckAccumulator, Random rnd)
        {
            if (deckPart.Count <= 0) return;
            var count = rnd.Next(1, deckPart.Count);
            for (var _ = 0; _ < count; _++)
                deckAccumulator.Add(deckPart.Pop());
        }
    }
}