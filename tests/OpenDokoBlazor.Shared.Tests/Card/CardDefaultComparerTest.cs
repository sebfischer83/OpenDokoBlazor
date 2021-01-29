using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDokoBlazor.Shared.Cards;
using Shouldly;
using Xunit;

namespace OpenDokoBlazor.Shared.Tests.Card
{
    public class CardDefaultComparerTest
    {
        [Fact]
        public void SortSimpleTest()
        {
            List<ICard> cards = new List<ICard>();
            CardDefaultComparer cardDefaultComparer = new CardDefaultComparer();
            
            cards.Add(new NineCard(Suit.Clubs));
            cards.Add(new TenCard(Suit.Clubs));
            cards.Add(new KingCard(Suit.Clubs));
            cards.Sort(cardDefaultComparer);
            
            // from low to high
            cards[0].Value.ShouldBe(0);
            cards[1].Value.ShouldBe(4);
            cards[2].Value.ShouldBe(10);
        }

        [Fact]
        public void SortMixedTest()
        {
            List<ICard> cards = new List<ICard>();
            CardDefaultComparer cardDefaultComparer = new CardDefaultComparer();

            cards.Add(new NineCard(Suit.Clubs));
            cards.Add(new AceCard(Suit.Hearts));
            cards.Add(new TenCard(Suit.Clubs));
            cards.Add(new JackCard(Suit.Clubs));
            cards.Add(new KingCard(Suit.Clubs));
            cards.Sort(cardDefaultComparer);

            // from low to high
            cards[0].Value.ShouldBe(11);
            cards[1].Value.ShouldBe(0);
            cards[2].Value.ShouldBe(4);
            cards[3].Value.ShouldBe(10);
            cards[4].Value.ShouldBe(2);
        }

        [Fact]
        public void SortMixed2Test()
        {
            List<ICard> cards = new List<ICard>();
            CardDefaultComparer cardDefaultComparer = new CardDefaultComparer();

            cards.Add(new NineCard(Suit.Clubs));
            cards.Add(new TenCard(Suit.Hearts));
            cards.Add(new AceCard(Suit.Hearts));
            cards.Add(new TenCard(Suit.Clubs));
            cards.Add(new JackCard(Suit.Clubs));
            cards.Add(new KingCard(Suit.Clubs));
            cards.Sort(cardDefaultComparer);

            // from low to high
            cards[0].Value.ShouldBe(11);
            cards[1].Value.ShouldBe(0);
            cards[2].Value.ShouldBe(4);
            cards[3].Value.ShouldBe(10);
            cards[4].Value.ShouldBe(2);
            cards[5].Value.ShouldBe(10);
            cards[5].Suit.ShouldBe(Suit.Hearts);
        }

        [Fact]
        public void SortTrumpTest()
        {
            List<ICard> cards = new List<ICard>();
            CardDefaultComparer cardDefaultComparer = new CardDefaultComparer();

            cards.Add(new TenCard(Suit.Hearts));
            cards.Add(new JackCard(Suit.Clubs));
            cards.Add(new JackCard(Suit.Hearts));
            cards.Add(new JackCard(Suit.Spades));
            cards.Add(new QueenCard(Suit.Spades));
            cards.Add(new JackCard(Suit.Diamonds));
            cards.Sort(cardDefaultComparer);

            // from low to high
            cards[0].Value.ShouldBe(2);
            cards[0].Suit.ShouldBe(Suit.Diamonds);
            cards[1].Value.ShouldBe(2);
            cards[1].Suit.ShouldBe(Suit.Hearts);
            cards[2].Value.ShouldBe(2);
            cards[2].Suit.ShouldBe(Suit.Spades);
            cards[3].Value.ShouldBe(2);
            cards[3].Suit.ShouldBe(Suit.Clubs);
            cards[4].Value.ShouldBe(3);
            cards[5].Value.ShouldBe(10);
            cards[5].Suit.ShouldBe(Suit.Hearts);
        }

        [Fact]
        public void SortFullTest()
        {
            List<ICard> cards = new List<ICard>();
            CardDefaultComparer cardDefaultComparer = new CardDefaultComparer();

            cards.Add(new TenCard(Suit.Hearts));
            cards.Add(new TenCard(Suit.Clubs));
            cards.Add(new TenCard(Suit.Clubs));
            cards.Add(new QueenCard(Suit.Clubs));
            cards.Add(new JackCard(Suit.Hearts));
            cards.Add(new JackCard(Suit.Spades));
            cards.Add(new QueenCard(Suit.Spades));
            cards.Add(new JackCard(Suit.Diamonds));
            cards.Add(new AceCard(Suit.Diamonds));
            cards.Add(new KingCard(Suit.Diamonds));
            cards.Add(new NineCard(Suit.Diamonds));
            cards.Add(new NineCard(Suit.Hearts));
            
            cards.Sort(cardDefaultComparer);

            // from low to high
            cards[0].Value.ShouldBe(0);
            cards[0].Suit.ShouldBe(Suit.Hearts);
            cards[1].Value.ShouldBe(10);
            cards[1].Suit.ShouldBe(Suit.Clubs);
            cards[2].Value.ShouldBe(10);
            cards[2].Suit.ShouldBe(Suit.Clubs);
            cards[3].Value.ShouldBe(0);
            cards[3].Suit.ShouldBe(Suit.Diamonds);
            cards[4].Value.ShouldBe(4);
            cards[4].Suit.ShouldBe(Suit.Diamonds);
            cards[5].Value.ShouldBe(11);
            cards[5].Suit.ShouldBe(Suit.Diamonds);
            cards[6].Value.ShouldBe(2);
            cards[6].Suit.ShouldBe(Suit.Diamonds);
            cards[7].Value.ShouldBe(2);
            cards[7].Suit.ShouldBe(Suit.Hearts);
            cards[8].Value.ShouldBe(2);
            cards[8].Suit.ShouldBe(Suit.Spades);
            cards[9].Value.ShouldBe(3);
            cards[9].Suit.ShouldBe(Suit.Spades);
            cards[10].Value.ShouldBe(3);
            cards[10].Suit.ShouldBe(Suit.Clubs);
            cards[11].Value.ShouldBe(10);
            cards[11].Suit.ShouldBe(Suit.Hearts);
        }

        [Fact]
        public void SortFullTest2()
        {
            List<ICard> cards = new List<ICard>();
            CardDefaultComparer cardDefaultComparer = new CardDefaultComparer();

            cards.Add(new TenCard(Suit.Hearts));
            cards.Add(new TenCard(Suit.Clubs));
            cards.Add(new TenCard(Suit.Clubs));
            cards.Add(new QueenCard(Suit.Clubs));
            cards.Add(new JackCard(Suit.Hearts));
            cards.Add(new JackCard(Suit.Spades));
            cards.Add(new QueenCard(Suit.Spades));
            cards.Add(new JackCard(Suit.Diamonds));
            cards.Add(new AceCard(Suit.Diamonds));
            cards.Add(new KingCard(Suit.Diamonds));
            cards.Add(new NineCard(Suit.Diamonds));
            cards.Add(new NineCard(Suit.Hearts));

            cards.Sort(cardDefaultComparer);

            // from low to high
            cards[0].Value.ShouldBe(0);
            cards[0].Suit.ShouldBe(Suit.Hearts);
            cards[1].Value.ShouldBe(10);
            cards[1].Suit.ShouldBe(Suit.Clubs);
            cards[2].Value.ShouldBe(10);
            cards[2].Suit.ShouldBe(Suit.Clubs);
            cards[3].Value.ShouldBe(0);
            cards[3].Suit.ShouldBe(Suit.Diamonds);
            cards[4].Value.ShouldBe(4);
            cards[4].Suit.ShouldBe(Suit.Diamonds);
            cards[5].Value.ShouldBe(11);
            cards[5].Suit.ShouldBe(Suit.Diamonds);
            cards[6].Value.ShouldBe(2);
            cards[6].Suit.ShouldBe(Suit.Diamonds);
            cards[7].Value.ShouldBe(2);
            cards[7].Suit.ShouldBe(Suit.Hearts);
            cards[8].Value.ShouldBe(2);
            cards[8].Suit.ShouldBe(Suit.Spades);
            cards[9].Value.ShouldBe(3);
            cards[9].Suit.ShouldBe(Suit.Spades);
            cards[10].Value.ShouldBe(3);
            cards[10].Suit.ShouldBe(Suit.Clubs);
            cards[11].Value.ShouldBe(10);
            cards[11].Suit.ShouldBe(Suit.Hearts);
        }
    }
}
