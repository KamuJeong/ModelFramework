using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamu.ModelFramework;
using System;
using System.Linq;

namespace Kamu.ModelFrameworkTests
{
    [TestClass]
    public class ModelTest
    {
        #region [Cliche]

        public TestContext TestContext { get; set; }   

        private Uri Name = new Uri("hello://here/?greeting");
        private ModelInventory Inventory;

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

        #endregion

        #region [Simple model]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldRespondToSaveOrThrowInvalidOpertionExceptionIfNotValidSave()
        {
            var model = Inventory.Get<HelloModel>(Name);

            Action<string> test = (greet) => 
            {
                model.Greeting = greet;
                TestContext.Write(model.Greeting + " => ");
                model.Save();
                TestContext.WriteLine(model.Greeting);
                Assert.AreEqual(HelloMachine.Responses[greet.ToLower()], model.Greeting);
            };

            test("hi");
            test("How are you?");
            test("Nice to meet you");
        }

        [TestMethod]
        public void ShouldRespondToModelSpecificAction()
        {
            var model = Inventory.Get<HelloModel>(Name);
            model.WhoAreYou();

            Assert.AreEqual(2, model.ChangedCount);
            Assert.AreEqual(ChangingSource.Provider, model.ChangingEvents[1]);
            Assert.AreEqual("Hi! I'm a \'HelloMachine\'", model.Greeting);
        }

        #endregion

        #region [Complex model with the same provider]

        [TestMethod]
        public void ShouldLoadOtherModelFromSameProvider()
        {
            var good = Inventory.Get<GoodModel>(Name.Model("good"));
            
            Assert.AreEqual(1, Inventory.Count);
            good.Save();
            Assert.IsTrue(Inventory.Count > 1);
        }

        [TestMethod]
        public void ShouldCorrespondWithEachOtherModelFromSameProvider()
        {
            var model = Inventory.Get<HelloModel>(Name);

            Inventory.Get<GoodModel>(Name.Model("good")).Save();
            
            TestContext.WriteLine(model.Greeting);
            Assert.AreEqual("Good", model.Greeting.Split()[0]);
        }

        #endregion

        #region [Complex model with different providers]

        [TestMethod]
        public void ShouldLoadOtherModelFromDifferentProvider()
        {
            var empty = Inventory.Get<EmptyModel>(Name.Scheme("empty").Model("empty"));
            
            Assert.AreEqual(1, Inventory.Count);
            empty.Save();
            Assert.IsTrue(Inventory.Count > 1);
            Assert.AreEqual(2, Inventory.GetProviders().Count());
        }

        [TestMethod]
        public void ShouldCorrespondWithEachOtherModelFromDifferentProvider()
        {
            var model = Inventory.Get<HelloModel>(Name);

            Inventory.Get<EmptyModel>(Name.Scheme("empty").Model("empty")).Save();
            
            TestContext.WriteLine(model.Greeting);
            Assert.AreEqual("Anybody here?", model.Greeting);
        }

        #endregion
    }
}
