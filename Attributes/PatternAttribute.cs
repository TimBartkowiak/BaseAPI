namespace BaseAPI
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class PatternAttribute : System.Attribute
    {
        public string Pattern { get; }
        
        public PatternAttribute(string pattern)
        {
            this.Pattern = pattern;
        }
    }
}