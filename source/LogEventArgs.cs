using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MicrosoftOnline.Ads.LogAssistant
{
    [System.Serializable]
    public class LogEventArgs : EventArgs
    {
        public LogEventArgs(DateTime datetime, LogLevel level,
            LogCategoryType categoryType,
            string wayPoint,
            string message = null,
            object parameters = null,
            Exception exception = null,
            string trackingId = null,
            string[] sensitiveProperties = null)
        {
            this.LogDateTime = datetime;
            this.CategoryType = categoryType;
            this.WayPoint = wayPoint;
            this.Message = message;
            this.Parameters = parameters;
            this.Exception = exception;
            this.TrackingId = trackingId ?? Guid.NewGuid().ToString();
            this.Level = level;
            this.SensitiveProperties = sensitiveProperties;
        }

        private string _sensitiveString = "******";

        [JsonProperty("sensitiveString", Order = 9)]
        public string SensitiveString
        {
            get { return _sensitiveString; }
            set
            {
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
                    _sensitiveString = value;
            }
        }

        [JsonProperty("logDateTime", Order = 1)]
        public DateTime LogDateTime { get; private set; }

        [JsonProperty("level", Order = 2)]
        public LogLevel Level { get; private set; }

        [JsonProperty("category", Order = 3)]
        public LogCategoryType CategoryType { get; private set; }

        [JsonProperty("entry", Order = 4)]
        public string WayPoint { get; private set; }

        [JsonProperty("message", Order = 5)]
        public string Message { get; private set; }

        [JsonProperty("parameters", Order = 6)]
        public object Parameters { get; private set; }

        [JsonProperty("exception", Order = 7)]
        public Exception Exception { get; private set; }

        [JsonProperty("trackingId", Order = 0)]
        public string TrackingId { get; private set; }

        [JsonProperty("sensitiveProperties", Order = 8)]
        public string[] SensitiveProperties { get; private set; }

        public override string ToString()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(
                string.Format("\t{0:yyyy-MM-dd HH:mm:ss.fff}\t{1}\t[{2} - {3}]\tMessage = {4}",
                    this.LogDateTime,
                    this.CategoryType,
                    this.Level,
                    this.WayPoint,
                    this.Message ?? string.Empty));

            strBuilder.Append(string.Format("\r\n\tTrackingId = {0}", this.TrackingId));

            if (this.Parameters != null)
            {
                cache.Clear();
                _startTime = DateTime.UtcNow;

                strBuilder.Append(string.Format("\r\n\tParameters[{1}] = {0}", ParseClass(this.Parameters), this.Parameters.GetType()));
            }

            if (this.Exception != null)
            {
                cache.Clear();
                _startTime = DateTime.UtcNow;

                strBuilder.Append(string.Format("\r\n\tException[{1}] = {0}",
                    ParseClass(this.Exception), this.Exception.GetType()));
            }

            return strBuilder.ToString();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        private Dictionary<string, int> cache = new Dictionary<string, int>();
        private DateTime _startTime = DateTime.UtcNow;

        public string ParseClass(object o, int tabs = 0, object parent = null)
        {
            if (o == null)
                return "<NULL>";

            var _type = o.GetType();
            if (IsTypeIgnore(_type))
                return "<IngoredType>";

            if (!_type.IsClass)
                return string.Format("{0}", o);

            if (parent != null && parent.GetType() == _type)
            {
                var n = _type.ToString();
                if (cache.Keys.Contains(n))
                    cache[n]++;
                else
                    cache.Add(n, 1);
                if (cache[n] > 2)
                    return "<RecursiveType>";
            }

            if (DateTime.UtcNow.Subtract(_startTime).TotalSeconds >= 2)
                return "<ParseTimeOut>";

            var sb = new StringBuilder();
            sb.Append(string.Format("{0}{{ ", GetTabs(tabs)));

            tabs++;

            List<object> os = new List<object>();
            os.AddRange(_type.GetFields().Where(x => x.IsPublic));
            os.AddRange(_type.GetProperties().Where(x => x.CanRead));

            for (int i = 0; i < os.Count; i++)
            {
                var pf = os[i];
                Type pType = null;
                object _o = null;
                string _name = "";

                if (pf.GetType() == typeof(System.Reflection.PropertyInfo) ||
                    pf.GetType().ToString() == "System.Reflection.RuntimePropertyInfo")
                {
                    var p = (System.Reflection.PropertyInfo)pf;
                    pType = p.PropertyType;
                    _o = GetPropertyValue(o, p);
                    _name = p.Name;
                }
                else if (
                    pf.GetType() == typeof(System.Reflection.FieldInfo) ||
                    pf.GetType().ToString() == "System.Reflection.RuntimeFieldInfo" ||
                    pf.GetType().ToString() == "System.Reflection.RtFieldInfo")
                {
                    var f = (System.Reflection.FieldInfo)pf;
                    pType = f.FieldType;
                    _o = GetFieldValue(o, f);
                    _name = f.Name;
                }
                else
                {
                    continue;
                }

                if (_o == null || IsTypeIgnore(pType))
                    continue;

                //Console.WriteLine("Name:    {0} Type:{1}", _name, pType);

                if (pType.IsArray || pType.IsGenericType)
                {
                    //as will never throw an exception but return null if error occured
                    //Array list = _o as Array;
                    System.Collections.IList list = _o as System.Collections.IList;
                    if (list == null)//System.Nullable<T> : IsGenericType = true
                    {
                        sb.Append(string.Format("{4}{0}[{1}] = {2}{3} ",
                        _name ?? "<NULL>",
                        pType,
                        ParseClass(_o, tabs, o),
                        i == os.Count - 1 ? "" : ",",
                        GetTabs(tabs)));

                        continue;
                    }

                    var str = string.Format("{0}[", GetTabs(tabs));
                    for (int c = 0; c < list.Count; c++)
                    {
                        var a = list[c];
                        str += string.Format("{1}Array.[{0}] = ", c, GetTabs(tabs + 1));
                        if (a.GetType().IsClass && a.GetType() != typeof(System.String))
                        {
                            str += ParseClass(a, tabs + 1, o);
                        }
                        else
                        {
                            str += string.Format("{0}", a);
                        }
                    }
                    str += string.Format("{0}]", GetTabs(tabs));

                    sb.Append(string.Format("{4}{0}[{1}] = {2}{3} ",
                        _name ?? "<NULL>",
                        pType,
                        str,
                        i == os.Count - 1 ? "" : ",",
                        GetTabs(tabs)));
                }
                else if (pType.IsClass && pType != typeof(System.String))
                {
                    sb.Append(string.Format("{4}{0}[{1}] = {2}{3} ",
                        _name ?? "<NULL>",
                        pType,
                        ParseClass(_o, tabs, o),
                        i == os.Count - 1 ? "" : ",",
                        GetTabs(tabs)));
                }
                else
                {
                    sb.Append(string.Format("{4}{0}[{1}] = {2}{3} ",
                            _name ?? "<NULL>",
                            pType,
                            IsSensitiveProperty(_name) ? this.SensitiveString : _o,
                            i == os.Count - 1 ? "" : ",",
                            GetTabs(tabs)));
                }
            }

            tabs--;
            sb.Append(string.Format("{0}}}", GetTabs(tabs)));

            return sb.ToString();
        }

        private object GetPropertyValue(object source, System.Reflection.PropertyInfo p)
        {
            try
            {
                return p.GetValue(source);
            }
            catch
            {
                return null;
            }
        }

        private object GetFieldValue(object source, System.Reflection.FieldInfo f)
        {
            try
            {
                return f.GetValue(source);
            }
            catch
            {
                return null;
            }
        }

        private bool IsTypeIgnore(Type _type)
        {
            if (_type == null)
                return true;

            var _types = new string[]
                {
                    "System.Type",
                    "System.Reflection.Module",
                    "System.Reflection.MemberInfo",
                    "System.Reflection.ParameterInfo",
                    "System.Object",
                    "System.Reflection.RuntimeMethodInfo",
                    "System.Runtime.Serialization.ExtensionDataObject",
                    //"System.IO.DirectoryInfo",
                };
            foreach (var _t in _types)
            {
                if (_type.ToString() == _t)
                    return true;
            }
            return false;
        }

        private string GetTabs(int tabs)
        {
            if (tabs <= 0)
                return "\r\n\t";
            var str = "";
            for (int i = 0; i < tabs; i++)
                str += "\t";
            return "\r\n\t" + str;
        }

        private bool IsSensitiveProperty(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName) || string.IsNullOrEmpty(propertyName))
                return false;

            if (this.SensitiveProperties == null || this.SensitiveProperties.Length == 0)
                return false;

            return this.SensitiveProperties.Count(p => p.Equals(propertyName)) > 0;
        }
    }
}