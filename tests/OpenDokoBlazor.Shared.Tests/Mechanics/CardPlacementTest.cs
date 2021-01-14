using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using OpenDokoBlazor.Shared.Cards;
using OpenDokoBlazor.Shared.Deck;
using OpenDokoBlazor.Shared.Game;
using OpenDokoBlazor.Shared.Player;
using OpenDokoBlazor.Shared.Rules;
using Shouldly;
using Xunit;

namespace OpenDokoBlazor.Shared.Tests.Mechanics
{
    public class CardPlacementTest
    {
        private Shared.Mechanics.Mechanics GetTestMechanics(Action<Mock<Game.IGame>> setupAction)
        {
            var rules = new Mock<IRules>();
            rules.Setup(rules1 => rules1.DulleBeatsDulle).Returns(true);
            var game = new Mock<Game.IGame>();
            game.Setup(game1 => game1.Rules).Returns(rules.Object);
            setupAction?.Invoke(game);

            Shared.Mechanics.Mechanics mechanics = new Shared.Mechanics.Mechanics(game.Object);

            return mechanics;
        }

        [Fact]
        public void CardOnEmptyTrick()
        {
            var mechanics = GetTestMechanics(mock =>
            {
                mock.Setup(game => game.CurrentTrick).Returns(new List<IPlayedCard>());
            });
            var player = new Mock<IPlayer>();
            PlayerDeck playerDeck = new PlayerDeck(player.Object, new List<ICard>());

            mechanics.CanCardBePlaced(new AceCard(Suit.Hearts), playerDeck).ShouldBeTrue();
            mechanics.CanCardBePlaced(new TenCard(Suit.Hearts), playerDeck).ShouldBeTrue();
            mechanics.CanCardBePlaced(new QueenCard(Suit.Diamonds), playerDeck).ShouldBeTrue();
            mechanics.CanCardBePlaced(new AceCard(Suit.Spades), playerDeck).ShouldBeTrue();
        }


        [Fact]
        public void CardOnTrick1()
        {
            // any card is possible when there are no cards in in the deck of the player
            var player = new Mock<IPlayer>();
            var playerId = Guid.NewGuid();
            player.Setup(player1 => player1.Id).Returns(playerId);
            var mechanics = GetTestMechanics(mock =>
            {
                var trick = new List<IPlayedCard>();
                trick.Add(new PlayedCard(player.Object, new AceCard(Suit.Hearts), 1));
                mock.Setup(game => game.CurrentTrick).Returns(trick);
            });
            
            PlayerDeck playerDeck = new PlayerDeck(player.Object, new List<ICard>());
            mechanics.CanCardBePlaced(new AceCard(Suit.Hearts), playerDeck).ShouldBeTrue();
            mechanics.CanCardBePlaced(new AceCard(Suit.Clubs), playerDeck).ShouldBeTrue();
            mechanics.CanCardBePlaced(new JackCard(Suit.Spades), playerDeck).ShouldBeTrue();
        }

        [Fact]
        public void CardOnTrick2()
        {
            // plain suit on trick and player has the correct suit on his deck
            var player = new Mock<IPlayer>();
            var playerId = Guid.NewGuid();
            player.Setup(player1 => player1.Id).Returns(playerId);
            var mechanics = GetTestMechanics(mock =>
            {
                var trick = new List<IPlayedCard>();
                trick.Add(new PlayedCard(player.Object, new AceCard(Suit.Hearts), 1));
                mock.Setup(game => game.CurrentTrick).Returns(trick);
            });

            List<ICard> cardList = new();
            cardList.Add(new AceCard(Suit.Hearts));
            cardList.Add(new JackCard(Suit.Hearts));
            cardList.Add(new KingCard(Suit.Clubs));
            
            PlayerDeck playerDeck = new PlayerDeck(player.Object, cardList);

            mechanics.CanCardBePlaced(new AceCard(Suit.Hearts), playerDeck).ShouldBeTrue();
            mechanics.CanCardBePlaced(new AceCard(Suit.Clubs), playerDeck).ShouldBeFalse();
            mechanics.CanCardBePlaced(new JackCard(Suit.Spades), playerDeck).ShouldBeFalse();
        }

        [Fact]
        public void CardOnTrick3()
        {
            // plain suit on trick and player has the not correct suit on his deck
            var player = new Mock<IPlayer>();
            var playerId = Guid.NewGuid();
            player.Setup(player1 => player1.Id).Returns(playerId);
            var mechanics = GetTestMechanics(mock =>
            {
                var trick = new List<IPlayedCard>();
                trick.Add(new PlayedCard(player.Object, new AceCard(Suit.Hearts), 1));
                mock.Setup(game => game.CurrentTrick).Returns(trick);
            });

            List<ICard> cardList = new();
            cardList.Add(new JackCard(Suit.Hearts));
            cardList.Add(new KingCard(Suit.Clubs));

            PlayerDeck playerDeck = new PlayerDeck(player.Object, cardList);

            mechanics.CanCardBePlaced(new AceCard(Suit.Clubs), playerDeck).ShouldBeTrue();
            mechanics.CanCardBePlaced(new JackCard(Suit.Spades), playerDeck).ShouldBeTrue();
        }

        [Fact]
        public void CardOnTrick4()
        {
            // trump suit on trick
            var player = new Mock<IPlayer>();
            var playerId = Guid.NewGuid();
            player.Setup(player1 => player1.Id).Returns(playerId);
            var mechanics = GetTestMechanics(mock =>
            {
                var trick = new List<IPlayedCard>();
                trick.Add(new PlayedCard(player.Object, new TenCard(Suit.Hearts), 1));
                mock.Setup(game => game.CurrentTrick).Returns(trick);
            });

            List<ICard> cardList = new();
            cardList.Add(new JackCard(Suit.Hearts));
            cardList.Add(new KingCard(Suit.Clubs));

            PlayerDeck playerDeck = new PlayerDeck(player.Object, cardList);

            mechanics.CanCardBePlaced(new AceCard(Suit.Clubs), playerDeck).ShouldBeFalse();
            mechanics.CanCardBePlaced(new JackCard(Suit.Spades), playerDeck).ShouldBeTrue();
        }

        [Fact]
        public void CardOnTrick5()
        {
            // trump suit on trick
            var player = new Mock<IPlayer>();
            var playerId = Guid.NewGuid();
            player.Setup(player1 => player1.Id).Returns(playerId);
            var mechanics = GetTestMechanics(mock =>
            {
                var trick = new List<IPlayedCard>();
                trick.Add(new PlayedCard(player.Object, new TenCard(Suit.Hearts), 1));
                mock.Setup(game => game.CurrentTrick).Returns(trick);
            });

            List<ICard> cardList = new();
            cardList.Add(new KingCard(Suit.Clubs));

            PlayerDeck playerDeck = new PlayerDeck(player.Object, cardList);

            mechanics.CanCardBePlaced(new AceCard(Suit.Clubs), playerDeck).ShouldBeTrue();
        }
    }
}
