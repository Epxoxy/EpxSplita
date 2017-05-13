using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Collections.Generic
{
    [Serializable]
    public class OptionalItem<T1, T2>
    {
        public T1 Value { get; set; }
        public T2 Display { get; set; }
        public OptionalItem(T1 value, T2 display)
        {
            Value = value;
            Display = display;
        }

        public override string ToString()
        {
            if (Display is string) return Display.ToString();
            return base.ToString();
        }
    }
}

namespace GuetModel
{
    [Serializable]
    public class RequestBaseModel
    {
        public string Key { get; set; }
        public string Url { get; set; }
        public string ParsingRules { get; set; }
        public string[] DataHeaders { get; set; }

        public RequestBaseModel() { }
        public RequestBaseModel(string url, string parsingRules, string[] dataHeades)
        {
            Url = url;
            ParsingRules = parsingRules;
            DataHeaders = dataHeades;
        }
    }

    [Serializable]
    public class PostModel : RequestBaseModel
    {
        public BindingList<OptionalItem<string, string>[]> Selector { get; set; }
        public string FormatString { get; set; }
        public string NextKey { get; set; }
        public string HandHeader { get; set; }
        public string GetHandRule { get; set; }

        public PostModel() { }
        public PostModel(string url, string parsingRules, string[] dataHeades, BindingList<OptionalItem<string, string>[]> selector, string formatString, string handHeader, string getHandRule)
            : base(url, parsingRules, dataHeades)
        {
            Selector = selector;
            FormatString = formatString;
            HandHeader = handHeader;
            GetHandRule = getHandRule;
        }
    }
}
