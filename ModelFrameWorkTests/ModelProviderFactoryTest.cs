using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Kamu.ModelFramework;

namespace Kamu.ModelFrameworkTests
{
    [TestClass]
    public class ModelProviderFactoryTest
    {
        private ModelInventory Inventory = new ModelInventory();

        [TestInitialize]
        public void Initialize()
        {
            ModelProviderFactory.Register(typeof(EmptyMachine));
        }

        [TestCleanup]
        public void CleanUp()
        {
            ModelProviderFactory.Reset();
        }

        [TestMethod]
        public void ShouldBeAbleToOpenAnyProviderThatHasSchemeAttribute()
        {   
            Uri Name = new Uri("hello://here/");

            Assert.IsTrue(Inventory.Open(Name));

            Assert.AreNotEqual(0, Kamu.ModelFramework.ModelProviderFactory.Schemes.Count());

            Assert.IsTrue(Kamu.ModelFramework.ModelProviderFactory.Schemes.Contains(Name.Scheme));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotBeProvidersWhichHaveTheSameSchemeAttribute()
        {
            ModelProviderFactory.Reset();
            Inventory.Open(new Uri("empty://here/")); 
        }

        [TestMethod]
        public void ShouldRegisterNestedPublicProvider()
        {
            Uri Name = new Uri("nested.public://here/");

            Assert.IsTrue(Inventory.Open(Name));
            Assert.IsTrue(Kamu.ModelFramework.ModelProviderFactory.Schemes.Contains(Name.Scheme));
        }

        [TestMethod]
        public void ShouldRegisterNestedPrivateProvider()
        {
            Uri Name = new Uri("nested.private://here/");

            Assert.IsTrue(Inventory.Open(Name));
            Assert.IsTrue(Kamu.ModelFramework.ModelProviderFactory.Schemes.Contains(Name.Scheme));
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ShouldNotRegisterProviderThatDoesNotHaveSchemeAttributeManually()
        {
            ModelProviderFactory.Register(typeof(NoSchemeProvider));
        }
    }
}