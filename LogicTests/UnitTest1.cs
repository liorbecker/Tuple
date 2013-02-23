using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using Tuple.Logic.Interfaces;
using Tuple.Logic.Mock;

namespace LogicTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void DeckShouldOpenEightyOneCards()
        {
            IDeck deck = DeckFactory();

            int count = 0;

            while (!deck.IsEmpty())
            {
                count++;
                deck.GetNextCard();
            }

            Assert.AreEqual(81, count, "Unexpected number of cards");
        }

        [TestMethod]
        public void DeckShouldOpenDistinctCards()
        {
            IDeck deck = DeckFactory();

            HashSet<ICard> cards = new HashSet<ICard>();

            while (!deck.IsEmpty())
            {
                var card = deck.GetNextCard();
                Assert.IsFalse(cards.Contains(card), "Card {0} found twice in deck", card);
                cards.Add(card);
            }
        }

        [TestMethod]
        public void DeckShouldBeShuffeled()
        {
            IDeck deck1 = DeckFactory();
            IDeck deck2 = DeckFactory();

            List<ICard> cards1 = new List<ICard>();
            List<ICard> cards2 = new List<ICard>();

            while (!deck1.IsEmpty())
            {
                cards1.Add(deck1.GetNextCard());
            }

            while (!deck2.IsEmpty())
            {
                cards2.Add(deck2.GetNextCard());
            }

            int sameCount = 0;
            foreach (var cardsPair in cards1.Zip(cards2, (c1, c2) => new { C1 = c1, C2 = c2 }))
            {
                if (cardsPair.C1.Equals(cardsPair.C2))
                {
                    sameCount++;
                }
            }

            Assert.IsTrue(sameCount < 9, "More than 8 cards are in the same position");
        }

        [TestMethod]
        public void GameShouldOpenAtLeastTwelveCardsAtStart()
        {
            IGame game = GameFactory();

            int count = 0;
            while (game.ShouldOpenCard())
            {
                game.OpenCard();
                count++;
            }

            Assert.IsTrue(count > 11, "Game starts with less then 12 cards ({0})", count);
        }

        [TestMethod]
        public void GameShouldOpenNoMoreThanEighttennCardsAtStart()
        {
            IGame game = GameFactory();

            int count = 0;
            while (game.ShouldOpenCard())
            {
                game.OpenCard();
                count++;
            }

            Assert.IsTrue(count < 19, "Game starts with more then 18 cards ({0})", count);
        }

        [TestMethod]
        public void GameShouldNotLetOpenMoreThanEightteenCards()
        {
            IGame game = GameFactory();

            for (int i = 0; i < 18; i++)
            {
                game.OpenCard();
            }

            Assert.ThrowsException<Exception>((Func<ICardWithPosition>)game.OpenCard, "The game shouldn't allow to open the 19th card");
        }

        [TestMethod]
        public void GameToStringTest()
        {
            IGame game = GameFactory();

            for (int i = 0; i < 5; i++)
            {
                game.OpenCard();
            }

            var s = game.ToString();

            Assert.Inconclusive("Check the value of String s if you want to know if it's any good...");
        }

        private IDeck DeckFactory()
        {
            return new Deck();
        }

        private IGame GameFactory()
        {
            return new Game();
        }
    }
}
