using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Kamu.ModelFramework;

namespace Kamu.ModelFrameWorkTests
{
    [TestClass]
    public class BasicTest
    {
        [TestMethod]
        public void ShouldBePossibleToUseCustomizedSchemeWithinURI()
        {
            Uri uri = new Uri("pump://10.10.10.10/");

            Assert.AreEqual("pump", uri.Scheme);
        }

        [TestMethod]
        public void ShouldGetProviderURIFromModelURI()
        {
            Uri providerUri = new Uri("pump://here/");
            Uri modelUri = new Uri("pump://here/?information");

            Assert.AreEqual(providerUri, modelUri.Provider());
        }

        [TestMethod]
        public void ShouldGetModelQueryFromModelURI()
        {
            Uri modelUri = new Uri("pump://here/?information");

            Assert.AreEqual("information", modelUri.Model());
        }

        [TestMethod]
        public void ShouldChangeSchemeOfModelURI()
        {
            Uri modelUri = new Uri("pump://here/?information");
            
            Assert.AreEqual(new Uri("oven://here/?information"), modelUri.Scheme("oven"));
        }

        [TestMethod]
        public void ShouldChangeModelQueryOfModelURI()
        {
            Uri modelUri = new Uri("pump://here/?information");
            
            Assert.AreEqual(new Uri("pump://here/?config"), modelUri.Model("config"));
        }

        [TestMethod]
        public void ShouldCompareLocationOfURIs()
        {
            Uri pumpInformation = new Uri("pump://here/?information");
            Uri pumpLinkConfiguration = new Uri("pump.link://here/?configuration");
            Uri pumpInformationThere = new  Uri("pump://there/?information");

            Assert.IsTrue(pumpInformation.AreSameLocation(pumpLinkConfiguration));
            Assert.IsFalse(pumpInformation.AreSameLocation(pumpInformationThere));
        }
    }
}
