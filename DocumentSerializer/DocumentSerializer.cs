using System;
using System.Collections;
using System.Linq;
using Amazon.DynamoDBv2.DocumentModel;

namespace cpGames.Serialization
{
    public class DocumentSerializer
    {
        #region Methods
        public static Document Serialize(object item, byte mask = 0)
        {
            var doc = new Document { { Common.TYPE_KEY, item.GetType().AssemblyQualifiedName } };
            var fields = Common.GetFields(item.GetType()).ToArray();

            for (byte iField = 0; iField < fields.Length; iField++)
            {
                var field = fields.ElementAt(iField);

                if (field.IsStatic)
                {
                    continue;
                }

                if (field.IsInitOnly)
                {
                    continue;
                }

                var maskAtt = field.GetCpAttribute<CpMaskAttribute>();

                if (maskAtt != null && (maskAtt.Mask & mask) != mask)
                {
                    continue;
                }

                var value = field.GetValue(item);

                if (value != null)
                {
                    doc.Add(field.Name, SerializeField(value, mask));
                }
            }

            return doc;
        }

        private static DynamoDBEntry SerializeField(object value, byte mask = 0)
        {
            if (value == null)
            {
                return null;
            }

            var type = value.GetType();

            if (IsPrimitive(type))
            {
                return ValueToPrimitive(value);
            }

            if (type.GetInterfaces().Contains(typeof (IList)))
            {
                var list = (IList)value;
                var listOfEntries = new DynamoDBList();
                foreach (var item in list)
                {
                    listOfEntries.Add(SerializeField(item, mask));
                }
                return listOfEntries;
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

        private static bool IsPrimitive(Type type)
        {
            var converter = typeof (Primitive).GetMethod("op_Implicit", new[] { type });
            return converter != null;
        }

        private static Primitive ValueToPrimitive(object val)
        {
            var converter = typeof (Primitive).GetMethod("op_Implicit", new[] { val.GetType() });
            return (Primitive)converter.Invoke(null, new[] { val });
        }

        public static T Deserialize<T>(object data)
        {
            var type = typeof (T);

            var primitive = data as Primitive;
            if (primitive != null)
            {
                return (T)PrimitiveToValue(type, primitive);
            }

            if (type.GetInterfaces().Contains(typeof (IList)))
            {
                return (T)Common.InvokeGeneric<DocumentSerializer>("DeserializeList", type, data);
            }

            if (type.IsClass || type.IsInterface || type.IsValueType)
            {
                return (T)Common.InvokeGeneric<DocumentSerializer>("DeserializeDocument", type, data);
            }

            throw new Exception(string.Format("Unsupported type {0}", type.Name));
        }

        public static object PrimitiveToValue(Type type, Primitive primitive)
        {
            if (type == typeof (float))
            {
                return primitive.AsSingle();
            }
            if (type == typeof (int))
            {
                return primitive.AsInt();
            }
            if (type == typeof (string))
            {
                return primitive.AsString();
            }
            if (type == typeof (long))
            {
                return primitive.AsLong();
            }
            if (type == typeof (byte[]))
            {
                return primitive.AsByteArray();
            }
            if (type.IsEnum)
            {
                return Enum.Parse(type, primitive);
            }
            if (type == typeof (bool))
            {
                return primitive.AsBoolean();
            }
            if (type == typeof (Guid))
            {
                return primitive.AsGuid();
            }
            throw new Exception(string.Format("Unsupported type {0}", type.Name));
        }

        private static T DeserializeDocument<T>(Document doc)
        {
            var typeName = doc[Common.TYPE_KEY];
            var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Unsupported type {0}", typeName));
            }
            var serializable = Activator.CreateInstance(type);
            var fields = Common.GetFields(serializable.GetType());

            foreach (var field in fields)
            {
                if (field.IsStatic)
                {
                    continue;
                }

                if (field.IsInitOnly)
                {
                    continue;
                }

                DynamoDBEntry entry;
                if (doc.TryGetValue(field.Name, out entry))
                {
                    field.SetValue(serializable, Common.InvokeGeneric<DocumentSerializer>("Deserialize", field.FieldType, entry));
                }
            }
            return (T)serializable;
        }

        private static T DeserializeList<T>(DynamoDBList dbList) where T : IList
        {
            var type = typeof (T);
            var listCtor = type.GetConstructor(new[] { typeof (int) });
            var list = (IList)listCtor.Invoke(new object[] { dbList.Entries.Count });
            var elementType = Common.GetElementType(type);
            if (list.IsFixedSize)
            {
                for (var iEntry = 0; iEntry < dbList.Entries.Count; iEntry++)
                {
                    list[iEntry] = Common.InvokeGeneric<DocumentSerializer>("Deserialize", elementType, dbList.Entries[iEntry]);
                }
            }
            else
            {
                foreach (var entry in dbList.Entries)
                {
                    list.Add(Common.InvokeGeneric<DocumentSerializer>("Deserialize", elementType, entry));
                }
            }
            return (T)list;
        }
        #endregion
    }
}