using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using BaseAPI.Exceptions;
using Microsoft.VisualBasic;

namespace BaseAPI.Models
{
    public abstract class AbstractModel
    {
        protected string Id { get; set; }

        public string getId()
        {
            return Id;
        }

        protected void additionalValidation()
        {
            //override for additional validation
        }

        public void scrubAndValidate(RequiredModelAttribute.RequiredActionEnum requiredActionEnum)
        {
            PropertyInfo[] allProperties = this.GetType().GetProperties();
            
            scrub(allProperties);
            validate(allProperties, requiredActionEnum);
        }

        /**
     * Trims all of the white space from the front and back of a string input.  We can also add other scrubbing here if needs be.
     * @param allFields - The fields to loop through and scrub
     */
        private void scrub(PropertyInfo[] allProperties)
        {
            foreach (PropertyInfo property in allProperties)
            {
                try
                {
                    // See if we have a getter and try to call it, if either of these fail it's ok to move onto the next since we can't get the value
                    if (property.PropertyType == typeof(string))
                    {
                        string propertyValue = (string)property.GetValue(this);
                        if (propertyValue != null)
                        {
                            propertyValue = propertyValue.Trim();

                            property.SetValue(this, propertyValue);
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
        private void validate(PropertyInfo[] allProperties, RequiredModelAttribute.RequiredActionEnum requiredActionEnum)
        {
            foreach (PropertyInfo property in allProperties)
            {
                try
                {
                    object propertyValue = property.GetValue(this);
                    
                    RequiredModelAttribute requiredAttribute = property.GetCustomAttribute<RequiredModelAttribute>(true);
                    if (requiredAttribute != null)
                    {
                        RequiredModelAttribute.RequiredActionEnum fieldEnum = requiredAttribute.Type;

                        if (fieldEnum == RequiredModelAttribute.RequiredActionEnum.BOTH || requiredActionEnum == fieldEnum)
                        {
                            if (propertyValue == null)
                            {
                                throw new InvalidModelException("The field " + property.Name + " is required and a value has not been provided.");
                            } 
                            else if (property.PropertyType == typeof(string) && string.IsNullOrEmpty((string) propertyValue))
                            {
                                throw new InvalidModelException("The field " + property.Name + " is required and a value has not been provided");
                            }
                        }
                    }

                    if (propertyValue != null)
                    {
                        PatternAttribute patternAttribute = property.GetCustomAttribute<PatternAttribute>(true);
                        if (patternAttribute != null)
                        {
                            if (property.PropertyType == typeof(string))
                            {
                                string value = (string)propertyValue;

                                if (!string.IsNullOrEmpty(value))
                                {
                                    Regex regex = new Regex(patternAttribute.Pattern);

                                    if (!regex.IsMatch(value))
                                    {
                                        throw new InvalidModelException("The field " + property.Name + " doesn't match the pattern " + regex);
                                    }
                                }
                            }
                        }

                        // Check that all of the length fields are greater than or equal to their lengths
                        LengthAttribute lengthAttribute = property.GetCustomAttribute<LengthAttribute>(true);
                        if (lengthAttribute != null)
                        {
                            if (property.PropertyType == typeof(string))
                            {
                                int length = lengthAttribute.Length;
                                string value = (string)propertyValue;

                                if (value.Length < length)
                                {
                                    throw new InvalidModelException("The field " + property.Name + " isn't long enough.  It must have " + length + " characters.  It has: " + value.Length + " characters");
                                }
                            }
                        }

                        // Verify the max length fields
                        MaxLengthAttribute maxLengthAttribute = property.GetCustomAttribute<MaxLengthAttribute>(true);
                        if (maxLengthAttribute != null)
                        {
                            if (property.PropertyType == typeof(string))
                            {
                                int maxLength = maxLengthAttribute.Maxlength;
                                string value = (string)propertyValue;

                                if (value.Length > maxLength)
                                {
                                    throw new InvalidModelException("The field " + property.Name + " is too long.  It can only be " + maxLength + " characters long.  It is: " + value.Length);
                                }
                            }
                        }

                        // Check to see if the accepted values fields have values that are expected
                        AcceptedValuesAttribute acceptedValuesAttribute =
                            property.GetCustomAttribute<AcceptedValuesAttribute>(true);
                        if (acceptedValuesAttribute != null)
                        {
                            if (property.PropertyType == typeof(string))
                            {
                                Type enumType = acceptedValuesAttribute.EnumType;
                                string[] acceptedValues = acceptedValuesAttribute.Values;

                                string value = (string)propertyValue;

                                if (!string.IsNullOrEmpty(value))
                                {
                                    if (acceptedValues.Length > 0)
                                    {
                                        List<string> acceptedValuesList = acceptedValues.ToList();

                                        if (!acceptedValuesList.Contains(value))
                                        {
                                            throw new InvalidModelException("The value provided for field " + property.Name + " '" + value + "' isn't an accepted value for that field.  Accepted values are: " + string.Join(", ", acceptedValuesList));
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
                                                throw new InvalidModelException("The value provided for field " + property.Name + " '" + value + "' isn't an accepted value for that field.  Accepted values are: " + string.Join(", ", enumConstants));
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
                    if (e.GetType() == typeof(InvalidModelException))
                    {
                        throw;
                    }
                }
            }
        }
    }
}