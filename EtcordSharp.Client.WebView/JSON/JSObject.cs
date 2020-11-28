using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EtcordSharp.Client.WebView.JSON
{
    public abstract class JSObject
    {
        public abstract string name { get; }

        public virtual string ToJSON()
        {
            string json = "{";

            PropertyInfo[] properties = GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                json += properties[i].Name + ":";

                if (properties[i].PropertyType == typeof(string))
                    json += "'" + properties[i].GetValue(this) + "'";
                else if (properties[i].PropertyType == typeof(int) || properties[i].PropertyType == typeof(float) || properties[i].PropertyType == typeof(bool))
                    json += properties[i].GetValue(this);
                else if (properties[i].PropertyType == typeof(Enum))
                    json += properties[i].GetValue(this);

                if (i < properties.Length - 1)
                    json += ",";
            }

            json += "}";
            return json;
        }
    }
}
