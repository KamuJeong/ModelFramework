using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Kamu.ModelFramework;

namespace Kamu.ModelFrameWorkTests
{
    [TestClass]
    public class ModelProviderFactoryTest
    {
        private Uri Name = new Uri("hello://here/");
        private ModelInventory Inventory = new ModelInventory();

        [TestMethod]
        public void ShouldBeAbleToOpenAnyProviderWhichRegistersItsSchemeInStaticConstructor()
        {   
            Assert.IsTrue(Inventory.Open(Name));

            Assert.AreNotEqual(0, Kamu.ModelFramework.ModelProviderFactory.Schemes.Count());

            Assert.IsTrue(Kamu.ModelFramework.ModelProviderFactory.Schemes.Contains(Name.Scheme));
        }
    }
}