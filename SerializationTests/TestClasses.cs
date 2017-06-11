﻿using System;
using System.Collections.Generic;

namespace cpGames.Serialization.Tests
{
    public class TestClassA
    {
        public void SetValues()
        {
            guid = Guid.NewGuid();

            a_private = "private string";

            a = "this is a string";

            a_protected = "this is a protected string";

            n = 3;

            listOfStrings = new List<string>() { "a", "bc", "def" };

            listOfFloats = new List<float>() { 0.1f, 0.2f, 32.2f };

            arrayOfFloats = new float[] { 1, 2, 3.5f };

            arrayOfStrings = new string[] { "abc", "def" };

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
                new List<int> { 1, 2, 3},
                new List<int> { 5, 6, 7}
            };

            ignoreMe = "this should be ignored";

            onlyForPrivileged = "can you read that?";

            onlyForSuperPrivileged = "how about that?";

            forBoth = "or that?";
        }

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
    }

    public class TestClassB
    {
        public void SetValues()
        {
            b = "this is b";
            c = new TestClassC();
            c.SetValues();
        }

        public string b;

        public TestClassC c;
    }
    
    public class TestClassC
    {
        public void SetValues()
        {
            c = "I am nested all the way, but I have a pretty short name in serialization blob that consumes space, makes me readable, and kittens happy :)";
        }

        public string c;
    }

    public abstract class AbstractClass
    {
    }

    public interface Interface
    {

    }

    public class DerivedA : AbstractClass, Interface
    {
        public void SetValues()
        {
            a = "I am A";
        }

        public string a;
    }

    public class DerivedB : AbstractClass, Interface
    {
        public void SetValues()
        {
            b = "I am B";
        }

        public string b;
    }

    public class DictionaryClass
    {
        public Dictionary<string, float> dict;

        public void SetValues()
        {
            dict = new Dictionary<string, float>();
            dict.Add("test", 123);
        }
    }

    public struct TestStruct
    {
        public float a;

        public string b;
    }
}
