using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDokoBlazor.Shared.Cards;
using OpenDokoBlazor.Shared.ViewModels.Card;
using Shouldly;
using Xunit;

namespace OpenDokoBlazor.Shared.Tests.Card
{
    public class CardExtensionsTest
    {
        [Fact]
        public void ToViewModelTest()
        {
            TenCard card = new TenCard(Suit.Hearts);

            var result = card.ToViewModel();
            result.ShouldBeOfType<CardViewModel>();
            result.Suit.ShouldBe(Suit.Hearts);
        }
    }
}
