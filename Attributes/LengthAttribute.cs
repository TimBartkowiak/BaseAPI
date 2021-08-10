namespace BaseAPI
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]

    public class LengthAttribute : System.Attribute
    {
        public int Length { get; }
        
        public LengthAttribute(int minLength)
        {
            this.Length = minLength;
        }
    }
}