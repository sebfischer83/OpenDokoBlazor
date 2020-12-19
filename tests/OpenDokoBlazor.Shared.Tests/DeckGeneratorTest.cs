using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OpenDokoBlazor.Shared.Deck;
using OpenDokoBlazor.Shared.Rules;
using System;
using Xunit;
using Shouldly;
using System.Linq;

namespace OpenDokoBlazor.Shared.Tests
{
    public class DeckGeneratorTest
    {
        [Fact]
        public void CreateDefaultDeck()
        {            
            var rules = new Mock<IRules>();
            DeckGenerator deckGenerator = new DeckGenerator(rules.Object, new NullLogger<DeckGenerator>());
            var cards = deckGenerator.GetCards();

            cards.ShouldNotBeNull();
            cards.Count.ShouldBe(48);
            cards.Sum(x => x.Value).ShouldBe(240);
        }
    }
}
