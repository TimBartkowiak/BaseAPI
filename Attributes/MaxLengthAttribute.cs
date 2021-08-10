namespace BaseAPI
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class MaxLengthAttribute : System.Attribute
    {
        public int Maxlength { get; }

        public MaxLengthAttribute(int maxLength)
        {
            this.Maxlength = maxLength;
        }
    }
}