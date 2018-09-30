using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace cpGames.Serialization.Tests
{
    [TestClass]
    public class DictionarySerializerTests
    {
        #region Methods
        [TestMethod]
        public void DictionaryTest1()
        {
            var a = new TestClassA();
            a.SetValues();

            var serializedData_noMask = DictionarySerializer.Serialize(a);
            var a_NoMask = DictionarySerializer.Deserialize<TestClassA>(serializedData_noMask);

            var serializedData_Mask_1 = DictionarySerializer.Serialize(a, SerializationMaskType.Public);
            var a_Mask_1 = DictionarySerializer.Deserialize<TestClassA>(serializedData_Mask_1);

            var serializedData_Mask_2 = DictionarySerializer.Serialize(a, SerializationMaskType.Private);
            var a_Mask_2 = DictionarySerializer.Deserialize<TestClassA>(serializedData_Mask_2);

            var serializedData_Mask_1_2 = DictionarySerializer.Serialize(a, SerializationMaskType.Public | SerializationMaskType.Private);
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

        [TestMethod]
        public void ListTest()
        {
            //var lIn = new ListClass();
            var lIn = new ListContainerClass { list = new List<string>() };
            lIn.list.Add("Hello");
            lIn.list.Add("My name is");
            lIn.list.Add("Bob!");
            var serializedData = DictionarySerializer.Serialize(lIn);
            DictionarySerializer.DeserializeList<ListClass>(new object[] { serializedData });
            //var lOut = DictionarySerializer.Deserialize<ListClass>(serializedData);
            var lOut = DictionarySerializer.Deserialize<ListContainerClass>(serializedData);
        }

        [TestMethod]
        public void DictionaryDictionaryTest()
        {
            var a = new DictionaryClass();
            a.SetValues();

            var serializedData = DictionarySerializer.Serialize(a);
            var a1 = DictionarySerializer.Deserialize<DictionaryClass>(serializedData);
        }

        [TestMethod]
        public void ReadonlyListTest()
        {
            //var lIn = new ListClass();
            var lIn = new ListContainerClassReadonly();
            lIn.list.Add("Hello");
            lIn.list.Add("My name is");
            lIn.list.Add("Bob!");
            var serializedData = DictionarySerializer.Serialize(lIn);
            var lOut = DictionarySerializer.Deserialize<ListContainerClassReadonly>(serializedData);
        }
        #endregion
    }
}