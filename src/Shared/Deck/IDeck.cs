using Microsoft.Extensions.Logging;
using OpenDokoBlazor.Shared.Cards;
using OpenDokoBlazor.Shared.Mechanics;
using OpenDokoBlazor.Shared.Rules;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDokoBlazor.Shared.Deck
{
    public interface IDeck
    {

    }

    public interface IPlayerDeck
    {

    }

    public class Deck : IDeck
    {
        private const int ShuffleCount = 20;
        private readonly ILogger<Deck> _logger;

        public IImmutableList<ICard> Cards { get; private set; }


        /// <summary>
        /// The value of all cards in this deck
        /// </summary>
        public int Value => Cards.Sum(x => x.Value);

        protected internal Deck(IList<ICard> c, ILogger<Deck> logger)
        {
            Cards = Shuffle(c.ToArray());
            _logger = logger;
        }

        /// <summary>
        /// Fisher–Yates shuffle
        /// </summary>
        /// <remarks>
        /// https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        /// </remarks>
        /// <param name="array"></param>
        /// <returns></returns>
        private IImmutableList<ICard> Shuffle(ICard[] array)
        {
            System.Random random = new System.Random();
            for (int i = 0; i < array.Length; i++)
            {
                int j = random.Next(i, array.Length);
                ICard temp = array[i]; array[i] = array[j]; array[j] = temp;
            }

            return array.ToImmutableList();
        }

        public void Shuffle()
        {
            var arr = Cards.ToArray();
            Cards = Shuffle(arr).ToImmutableList();
        }
        
        public static Deck Create(IRules rules, ILoggerFactory loggerFactory)
        {
            DeckGenerator deckGenerator = new DeckGenerator(rules, loggerFactory.CreateLogger<DeckGenerator>());

            Deck deck = new Deck(deckGenerator.GetCards(), loggerFactory.CreateLogger<Deck>());
            
            return deck;
        }
    }

    public class DeckGenerator
    {
        private readonly IRules rules;
        private readonly ILogger<DeckGenerator> logger;

        public DeckGenerator(IRules rules, ILogger<DeckGenerator> logger)
        {
            this.rules = rules;
            this.logger = logger;
        }

        private void GetGenericCardSet(IList<ICard> cards)
        {
            logger.LogTrace("Create generic card set");
            // ace
            cards.Add(new AceCard(Suit.Clubs));
            cards.Add(new AceCard(Suit.Clubs));
            cards.Add(new AceCard(Suit.Diamonds));
            cards.Add(new AceCard(Suit.Diamonds));
            cards.Add(new AceCard(Suit.Hearts));
            cards.Add(new AceCard(Suit.Hearts));
            cards.Add(new AceCard(Suit.Spades));
            cards.Add(new AceCard(Suit.Spades));

            // ten
            cards.Add(new TenCard(Suit.Clubs));
            cards.Add(new TenCard(Suit.Clubs));
            cards.Add(new TenCard(Suit.Diamonds));
            cards.Add(new TenCard(Suit.Diamonds));
            cards.Add(new TenCard(Suit.Hearts));
            cards.Add(new TenCard(Suit.Hearts));
            cards.Add(new TenCard(Suit.Spades));
            cards.Add(new TenCard(Suit.Spades));

            // king
            cards.Add(new KingCard(Suit.Clubs));
            cards.Add(new KingCard(Suit.Clubs));
            cards.Add(new KingCard(Suit.Diamonds));
            cards.Add(new KingCard(Suit.Diamonds));
            cards.Add(new KingCard(Suit.Hearts));
            cards.Add(new KingCard(Suit.Hearts));
            cards.Add(new KingCard(Suit.Spades));
            cards.Add(new KingCard(Suit.Spades));

            // queen
            cards.Add(new QueenCard(Suit.Clubs));
            cards.Add(new QueenCard(Suit.Clubs));
            cards.Add(new QueenCard(Suit.Diamonds));
            cards.Add(new QueenCard(Suit.Diamonds));
            cards.Add(new QueenCard(Suit.Hearts));
            cards.Add(new QueenCard(Suit.Hearts));
            cards.Add(new QueenCard(Suit.Spades));
            cards.Add(new QueenCard(Suit.Spades));

            // jack
            cards.Add(new JackCard(Suit.Clubs));
            cards.Add(new JackCard(Suit.Clubs));
            cards.Add(new JackCard(Suit.Diamonds));
            cards.Add(new JackCard(Suit.Diamonds));
            cards.Add(new JackCard(Suit.Hearts));
            cards.Add(new JackCard(Suit.Hearts));
            cards.Add(new JackCard(Suit.Spades));
            cards.Add(new JackCard(Suit.Spades));

            // nine
            cards.Add(new NineCard(Suit.Clubs));
            cards.Add(new NineCard(Suit.Clubs));
            cards.Add(new NineCard(Suit.Diamonds));
            cards.Add(new NineCard(Suit.Diamonds));
            cards.Add(new NineCard(Suit.Hearts));
            cards.Add(new NineCard(Suit.Hearts));
            cards.Add(new NineCard(Suit.Spades));
            cards.Add(new NineCard(Suit.Spades));
        }

        public IList<ICard> GetCards()
        {
            List<ICard> cards = new List<ICard>();

            GetGenericCardSet(cards);

            return cards;
        }
    }
}
