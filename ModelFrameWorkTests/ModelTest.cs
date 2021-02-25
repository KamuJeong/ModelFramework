using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamu.ModelFramework;
using System;

namespace Kamu.ModelFrameWorkTests
{
    [TestClass]
    public class ModelTest
    {
        #region [Cliche]

        public TestContext TestContext { get; set; }   

        private Uri Name = new Uri("hello://here/?greeting");
        private ModelInventory Inventory = new ModelInventory();

        [TestInitialize]
        public void Initialize()
        {
            Assert.IsTrue(Inventory.Open(Name.Provider()));
        }

        [TestCleanup]
        public void CleanUp()
        {
            Inventory.Close(Name.Provider());
        }

        #endregion

        #region [Simple model]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldRespondToUpdateOrThrowInvalidOpertionExceptionIfNotValidUpdate()
        {
            var model = Inventory.Get<HelloModel>(Name);

            Action<string> test = (greet) => 
            {
                model.Greeting = greet;
                TestContext.Write(model.Greeting + " => ");
                model.Update();
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
        public void ShouldLoadOtherModelIfUpdateItThatWouldBeCreatedBySameProvider()
        {
            var good = Inventory.Get<GoodModel>(Name.Model("good"));
            
            Assert.AreEqual(1, Inventory.Count);
            good.Update();
            Assert.IsTrue(Inventory.Count > 1);
        }

        [TestMethod]
        public void ShouldCorrespondWithEachOtherModelThatWasCreatedBySameProvider()
        {
            var model = Inventory.Get<HelloModel>(Name);

            Inventory.Get<GoodModel>(Name.Model("good")).Update();
            
            TestContext.WriteLine(model.Greeting);
            Assert.AreEqual("Good", model.Greeting.Split()[0]);
        }

        #endregion

        #region [Complex model with different providers]

        [TestMethod]
        public void ShouldLoadOtherModelIfUpdateItThatWouldBeCreatedByDifferentProvider()
        {
            Assert.IsTrue(Inventory.Open(Name.Scheme("empty").Provider()));

            var empty = Inventory.Get<EmptyModel>(Name.Scheme("empty").Model("empty"));
            
            Assert.AreEqual(1, Inventory.Count);
            empty.Update();
            Assert.IsTrue(Inventory.Count > 1);
        }

        [TestMethod]
        public void ShouldCorrespondWithEachOtherModelThatWasCreatedByDifferentProvider()
        {
            var model = Inventory.Get<HelloModel>(Name);

            Inventory.Get<EmptyModel>(Name.Scheme("empty").Model("empty")).Update();
            
            TestContext.WriteLine(model.Greeting);
            Assert.AreEqual("Anybody here?", model.Greeting);
        }

        #endregion
    }
}
