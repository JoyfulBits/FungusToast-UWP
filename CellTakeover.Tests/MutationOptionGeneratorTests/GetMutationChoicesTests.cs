using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;

namespace Logic.Tests.MutationOptionGeneratorTests
{
    [TestClass]
    public class GetMutationChoicesTests
    {
        private readonly MutationOptionGenerator _mutationOptionGenerator = new MutationOptionGenerator();

        [TestMethod]
        public void It_Returns_IncreaseMutationChance_As_An_Option()
        {
            //--arrange
            var playerMock = new Mock<IPlayer>();

            //--act
            var mutationChoices = _mutationOptionGenerator.GetMutationChoices(playerMock.Object, null);

            //--assert
            mutationChoices.IncreaseMutationChance.ShouldBeTrue();
        }
    }
}
