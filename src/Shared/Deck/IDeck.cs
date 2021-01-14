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
using OpenDokoBlazor.Shared.Game;
using OpenDokoBlazor.Shared.Player;

namespace OpenDokoBlazor.Shared.Deck
{
    public interface IDeck
    {
        public (IList<ICard> deck1, IList<ICard> deck2, IList<ICard> deck3, IList<ICard> deck4) GetDeckForEachPlayer();
    }

    public interface IPlayerDeck
    {
        public IPlayer Player { get; }
        public ImmutableSortedSet<ICard> Cards { get; }

        public void Sort(IGame game);

        public ICard GetCard(Guid id);
    }

    public class PlayerDeck : IPlayerDeck
    {
        public PlayerDeck(IPlayer player, IList<ICard> cards)
        {
            Player = player;
            Cards = cards.ToImmutableSortedSet(new CardDefaultComparer());
        }

        public IPlayer Player { get; }
        public ImmutableSortedSet<ICard> Cards { get; }
        public void Sort(IGame game)
        {
            
        }
        
        public ICard GetCard(Guid id)
        {
            return new NineCard(Suit.Hearts);
        }
    }

    public class Deck : IDeck
    {
        private const int ShuffleCount = 20;
        private readonly IRules _rules;
        private readonly ILogger<Deck> _logger;

        public IImmutableList<ICard> Cards { get; private set; }


        /// <summary>
        /// The value of all cards in this deck
        /// </summary>
        public int Value => Cards.Sum(x => x.Value);

        protected internal Deck(IList<ICard> c, IRules rules, ILogger<Deck> logger)
        {
            Cards = Shuffle(c.ToArray());
            _rules = rules;
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

            Deck deck = new(deckGenerator.GetCards(), rules, loggerFactory.CreateLogger<Deck>());

            return deck;
        }

        public (IList<ICard> deck1, IList<ICard> deck2, IList<ICard> deck3, IList<ICard> deck4) GetDeckForEachPlayer()
        {
            var numberOfCards = Cards.Count / 4;
            if (numberOfCards != _rules.CardsPerPlayer)
            {
                _logger.LogError(new EventId(1001, "InvalidCardsPerPlayer"), "Deck with wrong number of cards!");
                throw new InvalidOperationException("Deck with wrong number of cards!");
            }

            List<ICard> list1 = new List<ICard>();
            List<ICard> list2 = new List<ICard>();
            List<ICard> list3 = new List<ICard>();
            List<ICard> list4 = new List<ICard>();
            for (var i = 0; i < Cards.Count; i += 4)
            {
                list1.Add(Cards[i]);
                list2.Add(Cards[i + 1]);
                list3.Add(Cards[i + 2]);
                list4.Add(Cards[i + 3]);
            }

            return (list1, list2, list3, list4);
        }
    }

    public class DeckGenerator
    {
        private readonly IRules _rules;
        private readonly ILogger<DeckGenerator> _logger;

        public DeckGenerator(IRules rules, ILogger<DeckGenerator> logger)
        {
            this._rules = rules;
            this._logger = logger;
        }

        private void GetGenericCardSet(IList<ICard> cards)
        {
            _logger.LogTrace("Create generic card set");
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
