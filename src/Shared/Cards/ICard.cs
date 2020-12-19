namespace OpenDokoBlazor.Shared.Cards
{
    public interface ICard
    {
        public Suit Suit { get; }

        public int Value { get; }
    }
}