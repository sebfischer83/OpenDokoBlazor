using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OpenDokoBlazor.Shared.Cards;
using OpenDokoBlazor.Shared.ViewModels.Card;

namespace OpenDokoBlazor.Client.Components.Card
{
    [Flags]
    public enum CardStyleTypes
    {
        Image = 1,
        Svg = 2
    }

    public interface ICardStyle
    {
        bool SupportSvg { get; }
        string StyleName { get; }
        CardStyleTypes SupportedTypes { get; set; }
        (string Src, string Type)[] GetImageSrcSetLinks(CardViewModel cardViewModel);
        string GetImageSrc(CardViewModel cardViewModel);
        string GetSvgSrc(CardViewModel cardViewModel);
        (string Src, string Type)[] GetBackImageSrcSetLinks();

        string GetBackImageSrc();

        string GetBackSvgSrc();
    }

    public abstract class CardStyle : ICardStyle
    {
        public abstract bool SupportSvg { get; }

        public abstract string StyleName { get; }

        public CardStyleTypes SupportedTypes { get; set; }

        public abstract (string Src, string Type)[] GetImageSrcSetLinks(CardViewModel cardViewModel);

        public abstract string GetImageSrc(CardViewModel cardViewModel);

        public abstract string GetSvgSrc(CardViewModel cardViewModel);

        public abstract (string Src, string Type)[] GetBackImageSrcSetLinks();

        public abstract string GetBackImageSrc();

        public abstract string GetBackSvgSrc();
    }

    public class FrenchCardSryle : CardStyle
    {
        protected enum ImageType
        {
            Png,
            Webp,
            Svg
        }

        private string GetFileName(string part, Suit suit, ImageType imageType)
        {
            return $"img/assets/cards/set1/{Enum.GetName(typeof(ImageType), imageType)?.ToLowerInvariant()}/{part}_of_{Enum.GetName(typeof(Suit), suit)?.ToLowerInvariant()}.{Enum.GetName(typeof(ImageType), imageType)?.ToLowerInvariant()}";
        }

        private string GetFileName(string part, ImageType imageType)
        {
            return $"img/assets/cards/set1/{Enum.GetName(typeof(ImageType), imageType)?.ToLowerInvariant()}/{part}.{Enum.GetName(typeof(ImageType), imageType)?.ToLowerInvariant()}";
        }

        public override bool SupportSvg => true;
        public override string StyleName => "French";
        public override (string Src, string Type)[] GetImageSrcSetLinks(CardViewModel cardViewModel)
        {
            switch (cardViewModel.Type)
            {
                case CardViewModelType.Nine:
                    return new (string Src, string Type)[] { new(GetFileName("9", cardViewModel.Suit, ImageType.Png), "image/png"),
                        new(GetFileName("9", cardViewModel.Suit, ImageType.Webp), "image/webp") };
                case CardViewModelType.Ten:
                    return new (string Src, string Type)[] { new(GetFileName("10", cardViewModel.Suit, ImageType.Png), "image/png"),
                        new(GetFileName("10", cardViewModel.Suit, ImageType.Webp), "image/webp") };
                case CardViewModelType.Ace:
                    return new (string Src, string Type)[] { new(GetFileName("ace", cardViewModel.Suit, ImageType.Png), "image/png"),
                        new(GetFileName("ace", cardViewModel.Suit, ImageType.Webp), "image/webp") };
                case CardViewModelType.Jack:
                    return new (string Src, string Type)[] { new(GetFileName("jack", cardViewModel.Suit, ImageType.Png), "image/png"),
                        new(GetFileName("jack", cardViewModel.Suit, ImageType.Webp), "image/webp") };
                case CardViewModelType.King:
                    return new (string Src, string Type)[] { new(GetFileName("king", cardViewModel.Suit, ImageType.Png), "image/png"),
                        new(GetFileName("king", cardViewModel.Suit, ImageType.Webp), "image/webp") };
                case CardViewModelType.Queen:
                    return new (string Src, string Type)[] { new(GetFileName("queen", cardViewModel.Suit, ImageType.Png), "image/png"),
                        new(GetFileName("queen", cardViewModel.Suit, ImageType.Webp), "image/webp") };
            }

            throw new ArgumentOutOfRangeException(nameof(cardViewModel));
        }

        public override string GetImageSrc(CardViewModel cardViewModel)
        {
            switch (cardViewModel.Type)
            {
                case CardViewModelType.Nine:
                    return GetFileName("9", cardViewModel.Suit, ImageType.Png);
                case CardViewModelType.Ten:
                    return GetFileName("10", cardViewModel.Suit, ImageType.Png);
                case CardViewModelType.Ace:
                    return GetFileName("ace", cardViewModel.Suit, ImageType.Png);
                case CardViewModelType.Jack:
                    return GetFileName("jack", cardViewModel.Suit, ImageType.Png);
                case CardViewModelType.King:
                    return GetFileName("king", cardViewModel.Suit, ImageType.Png);
                case CardViewModelType.Queen:
                    return GetFileName("queen", cardViewModel.Suit, ImageType.Png);
            }

            throw new ArgumentOutOfRangeException(nameof(cardViewModel));
        }

        public override string GetSvgSrc(CardViewModel cardViewModel)
        {
            switch (cardViewModel.Type)
            {
                case CardViewModelType.Nine:
                    return GetFileName("9", cardViewModel.Suit, ImageType.Svg);
                case CardViewModelType.Ten:
                    return GetFileName("10", cardViewModel.Suit, ImageType.Svg);
                case CardViewModelType.Ace:
                    return GetFileName("ace", cardViewModel.Suit, ImageType.Svg);
                case CardViewModelType.Jack:
                    return GetFileName("jack", cardViewModel.Suit, ImageType.Svg);
                case CardViewModelType.King:
                    return GetFileName("king", cardViewModel.Suit, ImageType.Svg);
                case CardViewModelType.Queen:
                    return GetFileName("queen", cardViewModel.Suit, ImageType.Svg);
            }

            throw new ArgumentOutOfRangeException(nameof(cardViewModel));
        }

        public override (string Src, string Type)[] GetBackImageSrcSetLinks()
        {
            return new (string Src, string Type)[] { new(GetFileName("back", ImageType.Png), "image/png"),
                new(GetFileName("back", ImageType.Webp), "image/webp") };
        }

        public override string GetBackImageSrc()
        {
            return GetFileName("back", ImageType.Png);
        }

        public override string GetBackSvgSrc()
        {
            return GetFileName("back", ImageType.Svg);
        }
    }
}
