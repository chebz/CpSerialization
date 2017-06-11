using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cpGames.Serialization.Tests
{
    [TestClass]
    public class DictionarySerializerTests
    {
        [TestMethod]
        public void DictionaryTest1()
        {
            var a = new TestClassA();
            a.SetValues();

            var serializedData_noMask = DictionarySerializer.Serialize(a);
            var a_NoMask = DictionarySerializer.Deserialize<TestClassA>(serializedData_noMask);

            var serializedData_Mask_1 = DictionarySerializer.Serialize(a, 1);
            var a_Mask_1 = DictionarySerializer.Deserialize<TestClassA>(serializedData_Mask_1);

            var serializedData_Mask_2 = DictionarySerializer.Serialize(a, 2);
            var a_Mask_2 = DictionarySerializer.Deserialize<TestClassA>(serializedData_Mask_2);

            var serializedData_Mask_1_2 = DictionarySerializer.Serialize(a, 1 | 2);
            var a_Mask_1_2 = DictionarySerializer.Deserialize<TestClassA>(serializedData_Mask_1_2);
        }

        [TestMethod]
        public void DictionaryTest2()
        {
            var a = new DerivedA();
            a.SetValues();

            var serializedData = DictionarySerializer.Serialize(a);
            var a1 = DictionarySerializer.Deserialize<Interface>(serializedData);
        }

        [TestMethod]
        public void DictionaryTest3()
        {
            var a = new TestStruct { a = 3, b = "testString" };
            var serializedData = DictionarySerializer.Serialize(a);
            var a_deserialized = DictionarySerializer.Deserialize<TestStruct>(serializedData);
        }
    }
}
