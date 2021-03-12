using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamu.ModelFramework;
using System;
using System.Linq;


namespace Kamu.ModelFrameworkTests
{
    [TestClass]
    public class ModelInventoryTest
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

        [TestMethod]
        public void ShouldMakeModelAvailableForURI()
        {   
            HelloModel model = Inventory.Get<HelloModel>(Name);

            Assert.AreEqual(Name, model.Uri);
            Assert.AreEqual(1, model.ChangedCount);
        }

        [TestMethod]
        public void ShouldBeSameModelAfterUpdateOrReload()
        {
            var model = Inventory.Get<HelloModel>(Name);
            model.Greeting = "hi";
            model.Save();

            var afterUpdate = Inventory.Get<Model>(Name);
            afterUpdate.Load();

            var afterReload = Inventory.Get<Model>(Name);

            Assert.AreSame(model, afterUpdate);
            Assert.AreSame(model, afterReload);
            Assert.AreEqual(3, model.ChangedCount);
        }

        [TestMethod]
        public void ShouldNotBeSameAfterDetaching()
        {
            var model = Inventory.Get<HelloModel>(Name);
            model.Detach();

            var after = Inventory.Get<HelloModel>(Name);

            Assert.IsFalse(model.IsProviderAttached);
            Assert.AreNotSame(model, after);         
        }

       [TestMethod]
        public void ShouldCloseProviderIfThereAreNotAttachedModels()
        {
            /// <see>
            /// model, model.Good => HelloMachine
            /// model.Empty => EmptyProvider
            /// </see>

            HelloModel model = Inventory.Get<HelloModel>(Name);
            Assert.AreEqual(2, Inventory.Providers.Count());

            model.Empty.Detach();
            Assert.AreEqual(1, Inventory.Providers.Count());
            model.Good.Detach();
            Assert.AreEqual(1, Inventory.Providers.Count());
            model.Detach();
            Assert.AreEqual(0, Inventory.Providers.Count());
        }

        [TestMethod]
        public void ShouldClearInventoryAfterDetaching()
        {
            HelloModel model = Inventory.Get<HelloModel>(Name);
            Assert.AreEqual(3, Inventory.Count);
            Assert.IsFalse(model.DetachedCallback);

            model.Empty.Detach();
            Assert.AreEqual(2, Inventory.Count);

            model.Good.Detach();
            Assert.AreEqual(1, Inventory.Count);

            model.Detach();
            Assert.AreEqual(0, Inventory.Count);
            Assert.IsTrue(model.DetachedCallback);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldCloseProviderIfItFailsToGetModelAndIsNotAttachedToAnyModel()
        {
            try
            {
                Inventory.Get<Model>(Name.Model("None"));
            }
            catch
            {
                Assert.AreEqual(0, Inventory.Providers.Count());
                throw;
            }
        }

        [TestMethod]
        public void ShouldClearInventoryWhenProviderCallsAbort()
        {
            HelloModel model = Inventory.Get<HelloModel>(Name);
            Assert.AreEqual(3, Inventory.Count);
            model.Good.Abort();
            Assert.AreEqual(1, Inventory.Count);    // because model.Empty is attached to the different provider.
            Assert.AreEqual(1, Inventory.Providers.Count());
            Assert.IsTrue(model.DetachedCallback);
        }
    }
}