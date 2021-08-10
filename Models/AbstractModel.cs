using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BaseAPI.Exceptions;
using Microsoft.VisualBasic;

namespace BaseAPI.Models
{
    public abstract class AbstractModel
    {
        protected string Id { get; set; }

        protected void additionalValidation()
        {
            //override for additional validation
        }

        public void scrubAndValidate(RequiredModelAttribute.RequiredActionEnum requiredActionEnum)
        {
            FieldInfo[] allFields = this.GetType().GetFields();
            
            scrub(allFields);
            validate(allFields, requiredActionEnum);
        }

        /**
     * Trims all of the white space from the front and back of a string input.  We can also add other scrubbing here if needs be.
     * @param allFields - The fields to loop through and scrub
     */
        private void scrub(FieldInfo[] allFields)
        {
            foreach (FieldInfo field in allFields)
            {
                try
                {
                    // See if we have a getter and try to call it, if either of these fail it's ok to move onto the next since we can't get the value
                    if (field.FieldType == typeof(string))
                    {
                        string fieldValue = (string)field.GetValue(this);
                        if (fieldValue != null)
                        {
                            fieldValue = fieldValue.Trim();

                            field.SetValue(this, fieldValue);
                        }
                    }
                }
                catch (Exception e)
                {
                    //ignore
                }
            }
        }

        /**
     * Loops through the fields sent in and validates each one based on the annotations on the field
     * @param allFields - The fields to loop through and validate
     * @param requiredActionEnum - The action we are performing this validation for
     * @throws InvalidModelException - Thrown if one of the validations isn't met.
     */
        private void validate(FieldInfo[] allFields, RequiredModelAttribute.RequiredActionEnum requiredActionEnum)
        {
            foreach (FieldInfo field in allFields)
            {
                try
                {
                    object fieldValue = field.GetValue(this);
                    
                    RequiredModelAttribute requiredAttribute = field.GetCustomAttribute<RequiredModelAttribute>(true);
                    if (requiredAttribute != null)
                    {
                        RequiredModelAttribute.RequiredActionEnum fieldEnum = requiredAttribute.Type;

                        if (fieldEnum == RequiredModelAttribute.RequiredActionEnum.BOTH || requiredActionEnum == fieldEnum)
                        {
                            if (fieldValue == null)
                            {
                                throw new InvalidModelException("The field " + field.Name + " is required and a value has not been provided.");
                            } 
                            else if (field.FieldType == typeof(string) && string.IsNullOrEmpty((string) fieldValue))
                            {
                                throw new InvalidModelException("The field " + field.Name + " is required and a value has not been provided");
                            }
                        }
                    }

                    if (fieldValue != null)
                    {
                        PatternAttribute patternAttribute = field.GetCustomAttribute<PatternAttribute>(true);
                        if (patternAttribute != null)
                        {
                            if (field.FieldType == typeof(string))
                            {
                                string value = (string)fieldValue;

                                if (!string.IsNullOrEmpty(value))
                                {
                                    Regex regex = new Regex(patternAttribute.Pattern);

                                    if (!regex.IsMatch(value))
                                    {
                                        throw new InvalidModelException("The field " + field.Name + " doesn't match the pattern " + regex);
                                    }
                                }
                            }
                        }

                        // Check that all of the length fields are greater than or equal to their lengths
                        LengthAttribute lengthAttribute = field.GetCustomAttribute<LengthAttribute>(true);
                        if (lengthAttribute != null)
                        {
                            if (field.FieldType == typeof(string))
                            {
                                int length = lengthAttribute.Length;
                                string value = (string)fieldValue;

                                if (value.Length < length)
                                {
                                    throw new InvalidModelException("The field " + field.Name + " isn't long enough.  It must have " + length + " characters.  It has: " + value.Length + " characters");
                                }
                            }
                        }

                        // Verify the max length fields
                        MaxLengthAttribute maxLengthAttribute = field.GetCustomAttribute<MaxLengthAttribute>(true);
                        if (maxLengthAttribute != null)
                        {
                            if (field.FieldType == typeof(string))
                            {
                                int maxLength = maxLengthAttribute.Maxlength;
                                string value = (string)fieldValue;

                                if (value.Length > maxLength)
                                {
                                    throw new InvalidModelException("The field " + field.Name + " is too long.  It can only be " + maxLength + " characters long.  It is: " + value.Length);
                                }
                            }
                        }

                        // Check to see if the accepted values fields have values that are expected
                        AcceptedValuesAttribute acceptedValuesAttribute =
                            field.GetCustomAttribute<AcceptedValuesAttribute>(true);
                        if (acceptedValuesAttribute != null)
                        {
                            if (field.FieldType == typeof(string))
                            {
                                Type enumType = acceptedValuesAttribute.EnumType;
                                string[] acceptedValues = acceptedValuesAttribute.Values;

                                string value = (string)fieldValue;

                                if (!string.IsNullOrEmpty(value))
                                {
                                    if (acceptedValues.Length > 0)
                                    {
                                        List<string> acceptedValuesList = acceptedValues.ToList();

                                        if (!acceptedValuesList.Contains(value))
                                        {
                                            throw new InvalidModelException("The value provided for field " + field.Name + " '" + value + "' isn't an accepted value for that field.  Accepted values are: " + string.Join(", ", acceptedValuesList));
                                        }
                                    }
                                    else
                                    {
                                        // If we designated an enum as the list of expected values verify that enum exists and check our value against the list of constants
                                        if (enumType != null && typeof(Enum).IsAssignableFrom(enumType))
                                        {
                                            List<string> enumConstants = Enum.GetNames(enumType).ToList();

                                            if (!enumConstants.Contains(value))
                                            {
                                                throw new InvalidModelException("The value provided for field " + field.Name + " '" + value + "' isn't an accepted value for that field.  Accepted values are: " + string.Join(", ", enumConstants));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    additionalValidation();
                }
                catch (Exception e)
                {
                    //ignore
                }
            }
        }
    }
}