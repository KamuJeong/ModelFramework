using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Kamu.ModelFramework;

namespace Kamu.ModelFrameworkTests
{
    [TestClass]
    public class ModelProviderFactoryTest
    {
        private ModelInventory Inventory;

        private Uri Name = new Uri("hello://here/?greeting");

        [TestInitialize]
        public void Initialize()
        {
            ModelProviderFactory.Register(typeof(EmptyMachine));
            Inventory = new ModelInventory();
        }

        [TestCleanup]
        public void CleanUp()
        {
            ModelProviderFactory.Reset();
        }

        [TestMethod]
        public void ShouldBeAbleToOpenAnyProviderThatHasSchemeAttribute()
        {   
            Assert.IsNotNull(Inventory.Get<Model>(Name));

            Assert.AreNotEqual(3, Kamu.ModelFramework.ModelProviderFactory.Schemes.Count());

            Assert.IsTrue(Kamu.ModelFramework.ModelProviderFactory.Schemes.Contains(Name.Scheme));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotBeProvidersWhichHaveTheSameSchemeAttribute()
        {
            ModelProviderFactory.Reset();
            Inventory.Get<Model>(Name);
        }

        [TestMethod]
        public void ShouldRegisterNestedPublicProvider()
        {
            Inventory.Get<Model>(Name);
            Assert.IsTrue(Kamu.ModelFramework.ModelProviderFactory.Schemes.Contains("nested.public"));
        }

        [TestMethod]
        public void ShouldRegisterNestedPrivateProvider()
        {
            Inventory.Get<Model>(Name);
            Assert.IsTrue(Kamu.ModelFramework.ModelProviderFactory.Schemes.Contains("nested.private"));
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ShouldNotRegisterProviderThatDoesNotHaveSchemeAttributeManually()
        {
            ModelProviderFactory.Register(typeof(NoSchemeProvider));
        }
    }
}