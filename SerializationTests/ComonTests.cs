using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cpGames.Serialization.Tests
{
    [TestClass]
    public class ComonTests
    {
        #region Methods
        [TestMethod]
        public void IsTypeOrDerivedTest()
        {
            var list = new List<int>();
            Assert.IsTrue(list.GetType().IsTypeOrDerived(typeof (IList)));
        }
        #endregion
    }
}