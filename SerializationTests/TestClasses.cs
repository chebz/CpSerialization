using System;
using System.Collections.Generic;

namespace cpGames.Serialization.Tests
{
    public class TestClassA
    {
        #region Fields
        private string a_private;

        public string a;

        protected string a_protected;

        public int n;

        public Guid guid;

        public List<string> listOfStrings;

        public List<float> listOfFloats;

        public float[] arrayOfFloats;

        public string[] arrayOfStrings;

        public TestClassB b;

        public List<AbstractClass> abstractClasses;

        public List<Interface> interfaces;

        public List<List<int>> listOfLists;

        [CpIgnore]
        public string ignoreMe;

        [CpMask(1)]
        public string onlyForPrivileged;

        [CpMask(2)]
        public string onlyForSuperPrivileged;

        [CpMask(1 | 2)]
        public string forBoth;
        #endregion

        #region Methods
        public void SetValues()
        {
            guid = Guid.NewGuid();

            a_private = "private string";

            a = "this is a string";

            a_protected = "this is a protected string";

            n = 3;

            listOfStrings = new List<string> { "a", "bc", "def" };

            listOfFloats = new List<float> { 0.1f, 0.2f, 32.2f };

            arrayOfFloats = new[] { 1, 2, 3.5f };

            arrayOfStrings = new[] { "abc", "def" };

            b = new TestClassB();
            b.SetValues();

            abstractClasses = new List<AbstractClass>();
            var da = new DerivedA();
            da.SetValues();
            abstractClasses.Add(da);
            var db = new DerivedB();
            db.SetValues();
            abstractClasses.Add(db);

            interfaces = new List<Interface>();
            var ia = new DerivedA();
            ia.SetValues();
            interfaces.Add(ia);
            var ib = new DerivedB();
            ib.SetValues();
            interfaces.Add(ib);

            listOfLists = new List<List<int>>
            {
                new List<int> { 1, 2, 3 },
                new List<int> { 5, 6, 7 }
            };

            ignoreMe = "this should be ignored";

            onlyForPrivileged = "can you read that?";

            onlyForSuperPrivileged = "how about that?";

            forBoth = "or that?";
        }
        #endregion
    }

    public class TestClassB
    {
        #region Fields
        public string b;

        public TestClassC c;
        #endregion

        #region Methods
        public void SetValues()
        {
            b = "this is b";
            c = new TestClassC();
            c.SetValues();
        }
        #endregion
    }

    public class TestClassC
    {
        #region Fields
        public string c;
        #endregion

        #region Methods
        public void SetValues()
        {
            c = "I am nested all the way, but I have a pretty short name in serialization blob that consumes space, makes me readable, and kittens happy :)";
        }
        #endregion
    }

    public abstract class AbstractClass {}

    public interface Interface {}

    public class DerivedA : AbstractClass, Interface
    {
        #region Fields
        public string a;
        #endregion

        #region Methods
        public void SetValues()
        {
            a = "I am A";
        }
        #endregion
    }

    public class DerivedB : AbstractClass, Interface
    {
        #region Fields
        public string b;
        #endregion

        #region Methods
        public void SetValues()
        {
            b = "I am B";
        }
        #endregion
    }

    public class DictionaryClass
    {
        #region Fields
        public Dictionary<string, float> dict;
        #endregion

        #region Methods
        public void SetValues()
        {
            dict = new Dictionary<string, float>();
            dict.Add("test", 123);
        }
        #endregion
    }

    public struct TestStruct
    {
        public float a;
        public string b;
    }

    public class ListClass : List<string>
    {
        #region Constructors
        public ListClass() {}
        public ListClass(int capacity) : base(capacity) {}
        #endregion
    }

    public class ListContainerClass
    {
        public List<string> list;
    }
}