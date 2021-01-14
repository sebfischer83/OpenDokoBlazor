using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using OpenDokoBlazor.Shared.Cards;
using OpenDokoBlazor.Shared.Game;
using Xunit;
using OpenDokoBlazor.Shared.Mechanics;
using OpenDokoBlazor.Shared.Player;
using OpenDokoBlazor.Shared.Rules;
using Shouldly;

namespace OpenDokoBlazor.Shared.Tests.Mechanics
{
    public class DefaultMechanicsTest
    {
        [Fact]
        public void IsTrumpCardTest()
        {
            var rules = new Mock<IRules>();
            var game = new Mock<IGame>();
            game.Setup(game1 => game1.Rules).Returns(rules.Object);
            game.Setup(game1 => game1.GameType).Returns(GameType.Default);

            var card = (ICard) new TenCard(Suit.Clubs);
            card.IsTrumpCard(game.Object).ShouldBeFalse();

            card = new NineCard(Suit.Hearts);
            card.IsTrumpCard(game.Object).ShouldBeFalse();

            card = new NineCard(Suit.Diamonds);
            card.IsTrumpCard(game.Object).ShouldBeTrue();

            card = new TenCard(Suit.Hearts);
            card.IsTrumpCard(game.Object).ShouldBeTrue();

            card = new KingCard(Suit.Hearts);
            card.IsTrumpCard(game.Object).ShouldBeFalse();

            card = new JackCard(Suit.Spades);
            card.IsTrumpCard(game.Object).ShouldBeTrue();
        }

        [Fact(DisplayName = "Test for a trick with one trump and three plain suits")]
        public void DetermineTrickWinnerOneTrumpThreePlainSuitTest()
        {
            var rules = new Mock<IRules>();
            var game = new Mock<IGame>();
            game.Setup(game1 => game1.Rules).Returns(rules.Object);
            var mechanics = new Shared.Mechanics.Mechanics(game.Object);
            
            var playerCard1 = new Mock<IPlayedCard>();
            playerCard1.Setup(card => card.Order).Returns(1);
            playerCard1.Setup(card => card.Card).Returns(new AceCard(Suit.Hearts));
            
            var playerCard2 = new Mock<IPlayedCard>();
            playerCard2.Setup(card => card.Order).Returns(2);
            playerCard2.Setup(card => card.Card).Returns(new NineCard(Suit.Hearts));

            var playerCard3 = new Mock<IPlayedCard>();
            playerCard3.Setup(card => card.Order).Returns(3);
            playerCard3.Setup(card => card.Card).Returns(new AceCard(Suit.Diamonds));

            var playerCard4 = new Mock<IPlayedCard>();
            playerCard4.Setup(card => card.Order).Returns(4);
            playerCard4.Setup(card => card.Card).Returns(new KingCard(Suit.Hearts));

            var arr = new[] { playerCard1.Object, playerCard2.Object, playerCard3.Object, playerCard4.Object };
            var winner = mechanics.DetermineTrickWinner(arr.OrderBy(card => card.Order));
            winner.Order.ShouldBe(3);
            winner.Card.ShouldBeOfType<AceCard>();
            winner.Card.Suit.ShouldBe(Suit.Diamonds);
        }


        [Fact(DisplayName = "Test a trick with 4 plain suits, when 2 cards with the same value in the trick")]
        public void DetermineTrickWinnerFourPlainSuitTest1()
        {
            var rules = new Mock<IRules>();
            var game = new Mock<IGame>();
            game.Setup(game1 => game1.Rules).Returns(rules.Object);
            var mechanics = new Shared.Mechanics.Mechanics(game.Object);

            var playerCard1 = new Mock<IPlayedCard>();
            playerCard1.Setup(card => card.Order).Returns(1);
            playerCard1.Setup(card => card.Card).Returns(new AceCard(Suit.Hearts));

            var playerCard2 = new Mock<IPlayedCard>();
            playerCard2.Setup(card => card.Order).Returns(2);
            playerCard2.Setup(card => card.Card).Returns(new NineCard(Suit.Hearts));

            var playerCard3 = new Mock<IPlayedCard>();
            playerCard3.Setup(card => card.Order).Returns(3);
            playerCard3.Setup(card => card.Card).Returns(new AceCard(Suit.Hearts));

            var playerCard4 = new Mock<IPlayedCard>();
            playerCard4.Setup(card => card.Order).Returns(4);
            playerCard4.Setup(card => card.Card).Returns(new KingCard(Suit.Hearts));

            var arr = new[] { playerCard1.Object, playerCard2.Object, playerCard3.Object, playerCard4.Object };
            var winner = mechanics.DetermineTrickWinner(arr.OrderBy(card => card.Order));
            winner.Order.ShouldBe(1);
            winner.Card.ShouldBeOfType<AceCard>();
            winner.Card.Suit.ShouldBe(Suit.Hearts);
        }

        [Fact(DisplayName = "Test a trick with 4 plain suits.")]
        public void DetermineTrickWinnerFourPlainSuitTest2()
        {
            var rules = new Mock<IRules>();
            var game = new Mock<IGame>();
            game.Setup(game1 => game1.Rules).Returns(rules.Object);
            var mechanics = new Shared.Mechanics.Mechanics(game.Object);

            var playerCard1 = new Mock<IPlayedCard>();
            playerCard1.Setup(card => card.Order).Returns(1);
            playerCard1.Setup(card => card.Card).Returns(new AceCard(Suit.Hearts));

            var playerCard2 = new Mock<IPlayedCard>();
            playerCard2.Setup(card => card.Order).Returns(2);
            playerCard2.Setup(card => card.Card).Returns(new NineCard(Suit.Hearts));

            var playerCard3 = new Mock<IPlayedCard>();
            playerCard3.Setup(card => card.Order).Returns(3);
            playerCard3.Setup(card => card.Card).Returns(new AceCard(Suit.Clubs));

            var playerCard4 = new Mock<IPlayedCard>();
            playerCard4.Setup(card => card.Order).Returns(4);
            playerCard4.Setup(card => card.Card).Returns(new KingCard(Suit.Hearts));

            var arr = new[] { playerCard1.Object, playerCard2.Object, playerCard3.Object, playerCard4.Object };
            var winner = mechanics.DetermineTrickWinner(arr.OrderBy(card => card.Order));
            winner.Order.ShouldBe(3);
            winner.Card.ShouldBeOfType<AceCard>();
            winner.Card.Suit.ShouldBe(Suit.Clubs);
        }

        [Fact(DisplayName = "Test a trick with 4 trumps.")]
        public void DetermineTrickWinnerFourTrumpsTest1()
        {
            var rules = new Mock<IRules>();
            var game = new Mock<IGame>();
            game.Setup(game1 => game1.Rules).Returns(rules.Object);
            var mechanics = new Shared.Mechanics.Mechanics(game.Object);

            var playerCard1 = new Mock<IPlayedCard>();
            playerCard1.Setup(card => card.Order).Returns(1);
            playerCard1.Setup(card => card.Card).Returns(new AceCard(Suit.Diamonds));

            var playerCard2 = new Mock<IPlayedCard>();
            playerCard2.Setup(card => card.Order).Returns(2);
            playerCard2.Setup(card => card.Card).Returns(new JackCard(Suit.Hearts));

            var playerCard3 = new Mock<IPlayedCard>();
            playerCard3.Setup(card => card.Order).Returns(3);
            playerCard3.Setup(card => card.Card).Returns(new JackCard(Suit.Clubs));

            var playerCard4 = new Mock<IPlayedCard>();
            playerCard4.Setup(card => card.Order).Returns(4);
            playerCard4.Setup(card => card.Card).Returns(new QueenCard(Suit.Hearts));

            var arr = new[] { playerCard1.Object, playerCard2.Object, playerCard3.Object, playerCard4.Object };
            var winner = mechanics.DetermineTrickWinner(arr.OrderBy(card => card.Order));
            winner.Order.ShouldBe(4);
            winner.Card.ShouldBeOfType<QueenCard>();
            winner.Card.Suit.ShouldBe(Suit.Hearts);
        }

        [Fact(DisplayName = "Test a trick with 4 trumps.")]
        public void DetermineTrickWinnerFourTrumpsTest2()
        {
            var rules = new Mock<IRules>();
            var game = new Mock<IGame>();
            rules.Setup(rules1 => rules1.DulleBeatsDulle).Returns(false);
            game.Setup(game1 => game1.Rules).Returns(rules.Object);
            var mechanics = new Shared.Mechanics.Mechanics(game.Object);

            var playerCard1 = new Mock<IPlayedCard>();
            playerCard1.Setup(card => card.Order).Returns(1);
            playerCard1.Setup(card => card.Card).Returns(new TenCard(Suit.Hearts));

            var playerCard2 = new Mock<IPlayedCard>();
            playerCard2.Setup(card => card.Order).Returns(2);
            playerCard2.Setup(card => card.Card).Returns(new TenCard(Suit.Hearts));

            var playerCard3 = new Mock<IPlayedCard>();
            playerCard3.Setup(card => card.Order).Returns(3);
            playerCard3.Setup(card => card.Card).Returns(new JackCard(Suit.Clubs));

            var playerCard4 = new Mock<IPlayedCard>();
            playerCard4.Setup(card => card.Order).Returns(4);
            playerCard4.Setup(card => card.Card).Returns(new QueenCard(Suit.Hearts));

            var arr = new[] { playerCard1.Object, playerCard2.Object, playerCard3.Object, playerCard4.Object };
            var winner = mechanics.DetermineTrickWinner(arr.OrderBy(card => card.Order));
            winner.Order.ShouldBe(1);
            winner.Card.ShouldBeOfType<TenCard>();
            winner.Card.Suit.ShouldBe(Suit.Hearts);
        }


        [Fact(DisplayName = "Test a trick with 4 trumps.")]
        public void DetermineTrickWinnerFourTrumpsTest3()
        {
            var rules = new Mock<IRules>();
            var game = new Mock<IGame>();
            rules.Setup(rules1 => rules1.DulleBeatsDulle).Returns(true);
            game.Setup(game1 => game1.Rules).Returns(rules.Object);
            var mechanics = new Shared.Mechanics.Mechanics(game.Object);

            var playerCard1 = new Mock<IPlayedCard>();
            playerCard1.Setup(card => card.Order).Returns(1);
            playerCard1.Setup(card => card.Card).Returns(new TenCard(Suit.Hearts));

            var playerCard2 = new Mock<IPlayedCard>();
            playerCard2.Setup(card => card.Order).Returns(2);
            playerCard2.Setup(card => card.Card).Returns(new TenCard(Suit.Hearts));

            var playerCard3 = new Mock<IPlayedCard>();
            playerCard3.Setup(card => card.Order).Returns(3);
            playerCard3.Setup(card => card.Card).Returns(new JackCard(Suit.Clubs));

            var playerCard4 = new Mock<IPlayedCard>();
            playerCard4.Setup(card => card.Order).Returns(4);
            playerCard4.Setup(card => card.Card).Returns(new QueenCard(Suit.Hearts));

            var arr = new[] { playerCard1.Object, playerCard2.Object, playerCard3.Object, playerCard4.Object };
            var winner = mechanics.DetermineTrickWinner(arr.OrderBy(card => card.Order));
            winner.Order.ShouldBe(2);
            winner.Card.ShouldBeOfType<TenCard>();
            winner.Card.Suit.ShouldBe(Suit.Hearts);
        }

        [Fact(DisplayName = "Test a trick with 4 trumps.")]
        public void DetermineTrickWinnerFourTrumpsTest4()
        {
            var rules = new Mock<IRules>();
            var game = new Mock<IGame>();
            rules.Setup(rules1 => rules1.DulleBeatsDulle).Returns(false);
            game.Setup(game1 => game1.Rules).Returns(rules.Object);
            var mechanics = new Shared.Mechanics.Mechanics(game.Object);

            var playerCard1 = new Mock<IPlayedCard>();
            playerCard1.Setup(card => card.Order).Returns(1);
            playerCard1.Setup(card => card.Card).Returns(new KingCard(Suit.Diamonds));

            var playerCard2 = new Mock<IPlayedCard>();
            playerCard2.Setup(card => card.Order).Returns(2);
            playerCard2.Setup(card => card.Card).Returns(new TenCard(Suit.Diamonds));

            var playerCard3 = new Mock<IPlayedCard>();
            playerCard3.Setup(card => card.Order).Returns(3);
            playerCard3.Setup(card => card.Card).Returns(new KingCard(Suit.Diamonds));

            var playerCard4 = new Mock<IPlayedCard>();
            playerCard4.Setup(card => card.Order).Returns(4);
            playerCard4.Setup(card => card.Card).Returns(new AceCard(Suit.Diamonds));

            var arr = new[] { playerCard1.Object, playerCard2.Object, playerCard3.Object, playerCard4.Object };
            var winner = mechanics.DetermineTrickWinner(arr.OrderBy(card => card.Order));
            winner.Order.ShouldBe(4);
            winner.Card.ShouldBeOfType<AceCard>();
            winner.Card.Suit.ShouldBe(Suit.Diamonds);
        }
    }
}
