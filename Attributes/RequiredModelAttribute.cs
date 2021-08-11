namespace BaseAPI
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class RequiredModelAttribute : System.Attribute
    {
        
        public enum RequiredActionEnum {CREATE, UPDATE, BOTH}

        private RequiredActionEnum requiredActionEnum;

        public RequiredModelAttribute()
        {
            this.requiredActionEnum = RequiredActionEnum.BOTH;
        }

        public virtual RequiredActionEnum Type
        {
            get => requiredActionEnum;
            set => requiredActionEnum = value;
        }

    }
}