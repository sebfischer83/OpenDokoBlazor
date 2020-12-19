using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using OpenDokoBlazor.Shared.Cards;
using Xunit;

namespace OpenDokoBlazor.Shared.Tests.Mechanics
{
    public class DefaultMechanicsTest
    {
        [Fact]
        public void IsTrumpCardTest()
        {
            var clubsTen = new Mock<TenCard>();
            clubsTen.Setup(card => card.Suit).Returns(Suit.Clubs);
        }
    }
}
