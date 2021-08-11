namespace BaseAPI
{
    [System.AttributeUsage(System.AttributeTargets.Property)]

    public class LengthAttribute : System.Attribute
    {
        public int Length { get; }
        
        public LengthAttribute(int minLength)
        {
            this.Length = minLength;
        }
    }
}