namespace BaseAPI
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class MaxLengthAttribute : System.Attribute
    {
        public int Maxlength { get; }

        public MaxLengthAttribute(int maxLength)
        {
            this.Maxlength = maxLength;
        }
    }
}