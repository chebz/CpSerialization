using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace cpGames.Serialization
{
    public class DictionarySerializer
    {
        #region Methods
        public static bool TryClone<T>(T source, out T output, out string errorMessage)
            where T : class
        {
            if (!TrySerialize(source, out var data, out errorMessage) ||
                !TryDeserialize(data, out output, out errorMessage))
            {
                output = null;
                return false;
            }
            return true;
        }

        public static bool TrySerialize(object item,
            SerializationMaskType mask,
            out Dictionary<string, object> data,
            out string errorMessage)
        {
            try
            {
                data = Serialize(item, mask);
                errorMessage = string.Empty;
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                data = null;
                return false;
            }
        }

        public static bool TrySerialize(object item,
            out Dictionary<string, object> data,
            out string errorMessage)
        {
            return TrySerialize(item, SerializationMaskType.Everything, out data, out errorMessage);
        }

        public static Dictionary<string, object> Serialize(object item,
            SerializationMaskType mask = SerializationMaskType.Everything)
        {
            var data = new Dictionary<string, object>
            {
                { Common.TYPE_KEY, item.GetType().AssemblyQualifiedName }
            };
            var fields = Common.GetFields(item.GetType()).ToArray();

            for (byte iField = 0; iField < fields.Length; iField++)
            {
                var field = fields.ElementAt(iField);

                if (field.IsStatic)
                {
                    continue;
                }

                if (field.IsLiteral)
                {
                    continue;
                }

                var maskAtt = field.GetAttribute<SerializationMaskAttribute>();

                if (maskAtt != null && maskAtt.IsMaskValid(mask))
                {
                    continue;
                }

                var value = field.GetValue(item);

                if (value != null)
                {
                    data.Add(field.Name, SerializeField(value, mask));
                }
            }

            return data;
        }

        private static object SerializeField(object value, SerializationMaskType mask)
        {
            if (value == null)
            {
                return null;
            }

            var type = value.GetType();

            if (type.IsPrimitive || type == typeof(string))
            {
                return value;
            }

            if (type.GetInterfaces().Contains(typeof(IList)))
            {
                var list = (IList)value;
                var listData = new object[list.Count];

                var iItem = 0;
                foreach (var item in list)
                {
                    listData[iItem++] = SerializeField(item, mask);
                }
                return listData;
            }

            if (type.GetInterfaces().Contains(typeof(IDictionary)))
            {
                var dict = (IDictionary)value;
                var dictData = new Dictionary<object, object>();
                foreach (var key in dict.Keys)
                {
                    dictData[SerializeField(key, mask)] = SerializeField(dict[key], mask);
                }
                return dictData;
            }

            if (type.IsEnum)
            {
                return value.ToString();
            }

            if (type.IsClass || type.IsValueType)
            {
                return Serialize(value, mask);
            }

            throw new Exception(string.Format("Unsupported type {0}", type.Name));
        }

        public static bool TryDeserialize<T>(object data, out T item, out string errorMessage)
        {
            try
            {
                item = Deserialize<T>(data);
                errorMessage = string.Empty;
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                item = default(T);
                return false;
            }
        }

        public static T Deserialize<T>(object data)
        {
            var type = typeof(T);
            var dataType = data.GetType();

            if (type.IsEnum)
            {
                return (T)Enum.Parse(type, (string)data);
            }

            if (type.IsPrimitive || type == typeof(string) ||
                dataType.IsPrimitive || dataType == typeof(string))
            {
                return (T)data;
            }

            if (type.GetInterfaces().Contains(typeof(IList)))
            {
                if (data is Dictionary<string, object> dataDictionary)
                {
                    data = dataDictionary["_items"];
                }
                return (T)Common.InvokeGeneric<DictionarySerializer>("DeserializeList", type, data);
            }

            if (type.GetInterfaces().Contains(typeof(IDictionary)))
            {
                return (T)Common.InvokeGeneric<DictionarySerializer>("DeserializeDictionary", type,
                    data);
            }

            if (type.IsClass || type.IsInterface || type.IsValueType)
            {
                return (T)Common.InvokeGeneric<DictionarySerializer>("DeserializeObject", type,
                    data);
            }

            throw new Exception(string.Format("Unsupported type {0}", type.Name));
        }

        private static T DeserializeObject<T>(IDictionary<string, object> data)
        {
            var type = Type.GetType((string)data[Common.TYPE_KEY]);
            var serializable =
                Activator.CreateInstance(type ?? throw new InvalidOperationException());
            var fields = Common.GetFields(type).ToArray();

            for (byte iField = 0; iField < fields.Length; iField++)
            {
                var field = fields.ElementAt(iField);

                if (field.IsStatic)
                {
                    continue;
                }

                if (data.TryGetValue(field.Name, out var value))
                {
                    field.SetValue(serializable,
                        Common.InvokeGeneric<DictionarySerializer>("Deserialize", field.FieldType,
                            value));
                }
            }
            return (T)serializable;
        }

        public static T DeserializeList<T>(IList<object> data) where T : IList
        {
            var type = typeof(T);
            var listCtor = type.GetConstructor(new[] { typeof(int) });
            var list = (IList)listCtor.Invoke(new object[] { data.Count });
            var elementType = Common.GetElementType(type);
            if (list.IsFixedSize)
            {
                for (var iEntry = 0; iEntry < data.Count; iEntry++)
                {
                    list[iEntry] = Common.InvokeGeneric<DictionarySerializer>("Deserialize",
                        elementType, data[iEntry]);
                }
            }
            else
            {
                foreach (var item in data)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    list.Add(Common.InvokeGeneric<DictionarySerializer>("Deserialize", elementType,
                        item));
                }
            }
            return (T)list;
        }

        public static T DeserializeDictionary<T>(IDictionary<object, object> data)
            where T : IDictionary
        {
            var type = typeof(T);
            var arguments = type.GetGenericArguments();
            var keyType = arguments[0];
            var valueType = arguments[1];
            var dict = (IDictionary)Activator.CreateInstance(type);

            foreach (var kvp in data)
            {
                var key = Common.InvokeGeneric<DictionarySerializer>("Deserialize",
                    keyType, kvp.Key);
                var value = Common.InvokeGeneric<DictionarySerializer>("Deserialize",
                    valueType, kvp.Value);
                dict[key] = value;
            }
            return (T)dict;
        }
        #endregion
    }
}