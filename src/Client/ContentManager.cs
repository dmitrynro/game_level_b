using System;
using System.Collections.Generic;

namespace Client
{
    public sealed class ContentManager : IDisposable
    {
        private Dictionary<string, object> Content = new Dictionary<string, object>();

        public void Add(string name, object value)
        {
            Content[name] = value;
        }

        public T Get<T>(string name) where T : class
        {
            if (Content.ContainsKey(name))
                return Content[name] as T;
            else
                return default(T);
        }

        public void Dispose()
        {
            foreach (var kv in Content)
                if (kv.Value is IDisposable)
                    (kv.Value as IDisposable).Dispose();

            Content.Clear();
        }
    }
}
