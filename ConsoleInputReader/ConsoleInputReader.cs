using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleInputReader
{
    public class Reader : IEnumerable<KeyValuePair<string, string>>
    {
        public Dictionary<string, string> Arguments = new Dictionary<string, string>();

        public Reader(string[] args)
        {
            string key = string.Empty;
            string value = string.Empty;
  
            foreach (string item in args)
            {
                if (item.StartsWith("/") || item.StartsWith("-"))
                {
                    // Last item was a flag that has no value
                    if (key != string.Empty && value == string.Empty)
                    {
                        Arguments.Add(key, string.Empty);
                        key = string.Empty;
                        value = string.Empty;
                    }

                    // Insert the old item, then create a new one
                    if (value != string.Empty)
                    {
                        Arguments.Add(key, value);
                        key = item;
                        value = string.Empty;
                    }
                    else
                    {
                        key = item;
                    }
                }
                // It's a value but we have to key that goes with it
                else if (key == string.Empty)
                {
                    throw new Exception(item + " is invalid!");
                }
                else
                {
                    value = item;
                    Arguments.Add(key, value);
                    key = string.Empty;
                    value = string.Empty;
                }
            }

            if (key != string.Empty)
            {
                Arguments.Add(key, value);
            }
        }

        public override string ToString()
        {
            string result = "";

            foreach (var p in Arguments)
            {
                result += p.Key + " " + p.Value + " ";
            }

            return result.Trim();
        }

        public bool ContainsKey(string key)
        {
            foreach (var p in Arguments)
            {
                if (p.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        public string GetKey(string value)
        {
            foreach (var p in Arguments)
            {
                if (p.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                    return p.Key;
            }

            return string.Empty;
        }

        public bool ContainsValue(string value)
        {
            foreach (var p in Arguments)
            {
                if (p.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        public string GetValue(string key)
        {
            foreach (var p in Arguments)
            {
                if (p.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                    return p.Value;
            }

            return string.Empty;
        }

        public bool SetValue(string key, string value)
        {
            foreach (var p in Arguments)
            {
                if (p.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase))
                {
                    Arguments.Remove(p.Key);
                    Arguments.Add(p.Key, p.Value);
                    return true;
                }
            }

            return false;
        }

        #region IEnumerable<KeyValuePair<string,string>> Members

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return Arguments.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Arguments.GetEnumerator();
        }

        #endregion
    }
}
