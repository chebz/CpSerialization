using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace cpGames.Serialization
{
    public static class Common
    {
        #region Fields
        public const string TYPE_KEY = "type";
        #endregion

        #region Methods
        public static Type GetElementType(Type type)
        {
            var elementType = type.GetElementType() ?? type.GetGenericArguments()[0];
            return elementType;
        }

        public static List<Type> FindAllDerivedTypes<T>()
        {
            var type = typeof (T);
            var assembly = Assembly.GetAssembly(type);
            var derivedType = type;
            return assembly
                .GetTypes()
                .Where(t =>
                    (t != derivedType) &&
                    derivedType.IsAssignableFrom(t)
                ).ToList();
        }

        public static List<Type> FindAllDerivedTypes<T>(Assembly assembly)
        {
            var type = typeof (T);
            var derivedType = type;
            return assembly
                .GetTypes()
                .Where(t =>
                    (t != derivedType) &&
                    derivedType.IsAssignableFrom(t)
                ).ToList();
        }

        public static bool IsTypeOrDerived(Type baseType, Type derivedType)
        {
            return baseType == derivedType || derivedType.IsSubclassOf(baseType);
        }

        public static bool IsTypeOrDerived(object baseObj, object derivedObj)
        {
            return IsTypeOrDerived(baseObj.GetType(), derivedObj.GetType());
        }

        public static bool IsTypeOrDerived(Type baseType, object derivedObj)
        {
            return IsTypeOrDerived(baseType, derivedObj.GetType());
        }

        public static object InvokeGeneric<T>(string methodName, Type t, object[] data)
        {
            var method = typeof (T).GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var generic = method.MakeGenericMethod(t);
            return generic.Invoke(null, data);
        }

        public static object InvokeGeneric<T>(string methodName, Type t, object data)
        {
            var method = typeof (T).GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var generic = method.MakeGenericMethod(t);
            return generic.Invoke(null, new[] { data });
        }

        public static object InvokeGeneric<T>(string methodName, Type t)
        {
            var method = typeof (T).GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var generic = method.MakeGenericMethod(t);
            return generic.Invoke(null, null);
        }

        public static object InvokeGeneric<T>(object source, string methodName, Type t)
        {
            var method = typeof (T).GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var generic = method.MakeGenericMethod(t);
            return generic.Invoke(source, null);
        }

        public static IEnumerable<FieldInfo> GetFields(Type type)
        {
            var fields =
                type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(x => !x.HasCpAttribute<CpIgnoreAttribute>());
            return fields;
        }

        public static T Factory<T>(Func<Type, object> factoryMethod, Type type) where T : class
        {
            T res = null;

            if (factoryMethod != null)
            {
                res = (T)factoryMethod(type);
            }
            if (res == null)
            {
                var ctor = type.GetConstructor(Type.EmptyTypes);
                res = (T)ctor.Invoke(null);
            }
            return res;
        }

        public static T GetCpAttribute<T>(this FieldInfo field)
        {
            return (T)field.GetCustomAttributes(typeof (T), true).FirstOrDefault();
        }

        public static bool HasCpAttribute<T>(this FieldInfo field)
        {
            return field.GetCpAttribute<T>() != null;
        }
        #endregion
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class CpMaskAttribute : Attribute
    {
        #region Fields
        private readonly byte _mask;
        #endregion

        #region Properties
        public byte Mask { get { return _mask; } }
        #endregion

        #region Constructors
        public CpMaskAttribute(byte mask)
        {
            _mask = mask;
        }
        #endregion
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class CpIgnoreAttribute : Attribute {}
}