using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OpenDokoBlazor.Shared.Deck;
using OpenDokoBlazor.Shared.Rules;
using Shouldly;
using Xunit;
using Xunit.Sdk;

namespace OpenDokoBlazor.Shared.Tests
{
    public class DeckTest
    {
        [Fact]
        public void DefaultTest()
        {
            var rules = new Mock<IRules>();
            DeckGenerator deckGenerator = new DeckGenerator(rules.Object, new NullLogger<DeckGenerator>());
            var cards = deckGenerator.GetCards();
            Deck.Deck deck = Deck.Deck.Create(rules.Object, NullLoggerFactory.Instance);
            
            deck.Value.ShouldBe(240);
            deck.Cards.Count.ShouldBe(48);
            var byType = deck.Cards.GroupBy(card => card.GetType()).ToList();
            // six types of cards
            byType.Count.ShouldBe(6);
            // eight cards per type
            byType.Any(grouping => grouping.Count() == 8).ShouldBeTrue();
            var bySuit  = deck.Cards.GroupBy(x => x.Suit).ToList();
            // 4 suits
            bySuit.Count.ShouldBe(4);
            // 12 cards per suit
            bySuit.Any(grouping => grouping.Count() == 12).ShouldBeTrue();
        }

        [Fact]
        public void ShuffleTest()
        {
            var rules = new Mock<IRules>();
            DeckGenerator deckGenerator = new DeckGenerator(rules.Object, new NullLogger<DeckGenerator>());
            var cards = deckGenerator.GetCards();
            Deck.Deck deck = Deck.Deck.Create(rules.Object, NullLoggerFactory.Instance);

            List<List<int>> list = new List<List<int>> {deck.Cards.Select(card => card.GetHashCode()).ToList()};
            for (int i = 0; i < 500; i++)
            {
                deck.Shuffle();
                list.Add(deck.Cards.Select(card => card.GetHashCode()).ToList());
            }

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (i == j)
                        continue;
                    if (list[i].SequenceEqual(list[j]))
                    {
                        throw new Exception("Not all are different!");
                    }
                }
            }
        }
    }
}
