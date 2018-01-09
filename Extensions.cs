using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace Mapps.Helpers.Extension
{
    public static class Extensions
    {
        // TODO: Add documentation
        public static List<dynamic> ToDynamic<T>(this List<T> list, IEnumerable<string> propertyList) where T : class
        {
            if (list == null)
                return null;

            if (list.Count == 0)
                return new List<dynamic>();

            var dynamicList = new List<dynamic>();
            foreach (var obj in list)
                dynamicList.Add(obj.ToDynamic(propertyList));

            return dynamicList;
        }

        // TODO: Add documentation
        public static dynamic ToDynamic<T>(this T obj, IEnumerable<string> propertyList) where T : class
        {
            dynamic data = new ExpandoObject();
            var dataDictionary = (IDictionary<string, object>)data;

            foreach (var prop in propertyList)
            {
                if (!prop.Contains("."))
                    dataDictionary.Add(prop, obj.GetPropertyValue(prop));
                else
                {
                    var value = obj.GetPropertyValue(prop);
                    var properties = prop.Split('.');
                    var parent = dataDictionary;
                    for (var i = 0; i < properties.Length; i++)
                    {
                        if (i == properties.Length - 1)
                            parent.Add(properties[i], value);

                        if (i < properties.Length - 1)
                        {
                            if (parent.TryGetValue(properties[i], out dynamic current))
                            {
                                parent = current;
                                continue;
                            }
                            var child = new ExpandoObject();
                            parent.Add(properties[i], child);
                            parent = child;
                        }
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// Get the property value of a object by reflection.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="propName">Property Name. eg.: "MyProperty" or "ObjectProperty.Property".</param>
        /// <returns>Returns the value as a <see cref="object"/></returns>
        public static object GetPropertyValue(this object obj, string propName)
        {
            var properties = propName.Split('.');
            object value = obj;
            foreach (var prop in properties)
            {
                if (value == null)
                    break;
                value = value?.GetType().GetProperty(prop)?.GetValue(value, null);
            }
            return value;
        }
    }
}
