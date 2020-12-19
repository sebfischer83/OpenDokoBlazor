using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDokoBlazor.Shared.Cards
{
    public record NineCard : BaseCard
    {
        public NineCard(Suit suit) : base(suit)
        {
        }

        public override int Value => 0;
    }

    public record TenCard : BaseCard
    {
        public TenCard(Suit suit) : base(suit)
        {
        }

        public override int Value => 10;
    }

    public record AceCard : BaseCard
    {
        public AceCard(Suit suit) : base(suit)
        {
        }

        public override int Value => 11;
    }

    public record KingCard : BaseCard
    {
        public KingCard(Suit suit) : base(suit)
        {
        }

        public override int Value => 4;
    }

    public record QueenCard : BaseCard
    {
        public QueenCard(Suit suit) : base(suit)
        {
        }

        public override int Value => 3;
    }

    public record JackCard : BaseCard
    {
        public JackCard(Suit suit) : base(suit)
        {
        }

        public override int Value => 2;
    }
}
