using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork;
using SocialNetwork.Helpers;

namespace SocialNetwork.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestGetNetworks()
        {
            var result = VertexHelper.GetNetworks();
            Assert.AreEqual(result.Count, 504230);
        }

        [TestMethod]
        public void TestGetVertexes()
        {
            var result = VertexHelper.GetNetworks();
            var vertexesresult = VertexHelper.GetVertexs(result);
            Assert.AreEqual(vertexesresult.Count, 82168);
        }


        [TestMethod]
        public void TestGetGraphNodes()
        {
            var result = VertexHelper.GetNetworks();
            var vertexesresult = VertexHelper.GetVertexs(result);
            var graphnodereult = VertexHelper.GetGraphNodes(vertexesresult);
            Assert.AreEqual(graphnodereult.Count, 82168);
        }
    }
}
