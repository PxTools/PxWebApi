﻿namespace Px.Abstractions
{
    public class Codelist
    {
        public string Id { get; set; }
        public string Label { get; set; }

        public CodelistTypeEnum CodelistType { get; set; }

        public enum CodelistTypeEnum
        {
            ValueSet,
            Aggregation
        }

        public List<CodelistValue> Values { get; set; }

        public Codelist(string id, string label)
        {
            Values = new List<CodelistValue>();
            Id = id;
            Label = label;
        }

    }

    public class CodelistValue
    {
        public string Code { get; set; }
        public string Label { get; set; }
        public List<string> ValueMap { get; set; }

        public CodelistValue(string code, string label)
        {
            Code = code;
            Label = label;
            ValueMap = new List<string>();
        }
    }

}
