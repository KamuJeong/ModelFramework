using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kamu.ModelFramework;
using System;
using System.Linq;


namespace Kamu.ModelFrameWorkTests
{
    [TestClass]
    public class ModelInventoryTest
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
            model.Update();

            var afterUpdate = Inventory.Get<Model>(Name);
            afterUpdate.Reload();

            var afterReload = Inventory.Get<Model>(Name);

            Assert.AreSame(model, afterUpdate);
            Assert.AreSame(model, afterReload);
            Assert.AreEqual(3, model.ChangedCount);
        }

        [TestMethod]
        public void ShouldNotBeSameAfterReOpen()
        {
            var model = Inventory.Get<HelloModel>(Name);
            
            Inventory.Close(Name.Provider());
            Inventory.Open(Name.Provider());

            var after = Inventory.Get<HelloModel>(Name);

            Assert.IsFalse(model.IsProviderAttached);
            Assert.AreNotSame(model, after);         
        }

        [TestMethod]
        public void ShouldClearInventoryAfterCloseProvider()
        {
            int counterOffset = Inventory.Count;

            HelloModel model = Inventory.Get<HelloModel>(Name);
            Assert.IsTrue(counterOffset < Inventory.Count);
            Assert.IsFalse(model.DetachedCallback);

            Inventory.Close(Name.Provider());

            Assert.AreEqual(counterOffset, Inventory.Count);
            Assert.IsTrue(model.DetachedCallback);
        }
    }
}