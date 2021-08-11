using System;

namespace BaseAPI
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class AcceptedValuesAttribute : System.Attribute
    {
        public string[] Values { get; set; }
        public Type EnumType { get; set; }
    }
}