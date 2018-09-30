using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cpGames.Serialization.Tests
{
    [TestClass]
    public class DocumentSerializerTests
    {
        #region Methods
        [TestMethod]
        public void DocumentTest1()
        {
            var a = new TestClassA();
            a.SetValues();

            var doc = DocumentSerializer.Serialize(a);

            var b = DocumentSerializer.Deserialize<TestClassA>(doc);
        }

        [TestMethod]
        public void DocumentTest2()
        {
            var a = new DerivedA();
            a.SetValues();

            var doc = DocumentSerializer.Serialize(a);

            var b = DocumentSerializer.Deserialize<Interface>(doc);
        }

        [TestMethod]
        public void DocumentTest3()
        {
            var a = new TestStruct { a = 3, b = "testString" };
            var doc = DocumentSerializer.Serialize(a);
            var a_deserialized = DocumentSerializer.Deserialize<TestStruct>(doc);
        }

        [TestMethod]
        public void ВщсгьутеDictionaryTest()
        {
            var a = new DictionaryClass();
            a.SetValues();

            var serializedData = DocumentSerializer.Serialize(a);
            var a1 = DocumentSerializer.Deserialize<DictionaryClass>(serializedData);
        }
        #endregion
    }
}