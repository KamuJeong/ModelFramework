using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamu.ModelFramework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        public void CouldRespondToSave()
        {
            var model = Inventory.Get<HelloModel>(Name);

            Action<string> test = (greet) => 
            {
                model.Greeting = greet;
                TestContext.Write(model.Greeting + " => ");
                if(model.Save())
                {
                    TestContext.WriteLine(model.Greeting);
                    Assert.AreEqual(HelloMachine.Responses[greet.ToLower()], model.Greeting);
                }
                else
                {
                    TestContext.WriteLine("pardon?");
                }
            };

            test("hi");
            test("How are you?");
            test("Nice to meet you");
        }

        [TestMethod]
        public async Task CouldRespondToSaveAsync()
        {
            var model = await Inventory.GetAsync<HelloModel>(Name);

            Func<string, Task> test = async (greet) => 
            {
                model.Greeting = greet;
                TestContext.Write(model.Greeting + " [" + Thread.CurrentThread.ManagedThreadId + "] => ");
                if(await model.SaveAsync())
                {
                    TestContext.WriteLine(model.Greeting + " [" + Thread.CurrentThread.ManagedThreadId + "]");
                    Assert.AreEqual(HelloMachine.Responses[greet.ToLower()], model.Greeting);
                }
                else
                {
                    TestContext.WriteLine("pardon?" + " [" + Thread.CurrentThread.ManagedThreadId + "]");
                }
            };

            await test("hi");
            await test("How are you?");
            await test("Nice to meet you");
        }

        [TestMethod]
        public async Task ShouldExcuteAsyncMethodsSynchronously()
        {
            int count = 30;
            long expected = 5 + 2;
            expected = Enumerable.Range(0, count).Aggregate(expected, (acc, i) => (i&1)==0? acc + 2 : acc * 3);

            var model = await Inventory.GetAsync<CalculatorModel>(new Uri("calculator://here/?note"));

            await Task.WhenAll(Enumerable.Range(0, count).Select(i => (i&1)==0? model.LoadAsync().AsTask() : model.SaveAsync().AsTask()));

            Assert.AreEqual(expected, model.Result);

            long value = 7;
            await Task.WhenAll(Enumerable.Range(0, count).Select(i => (i&1) == 0 ? Task.Run(() => value += 2) : Task.Run(() => value *= 3)));

            TestContext.WriteLine($"Asynchronous Execution Test : Expected : {expected}, Actual : {value}");
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
            Assert.AreEqual(2, Inventory.Providers.Count());
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
