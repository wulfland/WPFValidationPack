namespace WPFValidationPack
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    public class DataAnnotationValidatorCollection<T> : IEnumerable<IValidator>
    {
        IList<IValidator> validators = new List<IValidator>();

        public DataAnnotationValidatorCollection(T source)
        {
            var sourceType = source.GetType();

            foreach (var property in sourceType.GetProperties())
            {
                var validationAttributes = GetValidationAttributes(property);
                if (validationAttributes.Count() > 0)
                {
                    foreach (var validationAttribute in validationAttributes)
                    {
                        validators.Add(new CustomValidator
                    {
                        PropertyName = property.Name, 
                        ErrorMessage = validationAttribute.FormatErrorMessage(property.Name), 
                        Check = () => !validationAttribute.IsValid(property.GetValue(source))
                    });
                    }
                }
            }
        }

        private static IEnumerable<ValidationAttribute> GetValidationAttributes(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributes<ValidationAttribute>(true);
        }

        public IEnumerator<IValidator> GetEnumerator()
        {
            return validators.GetEnumerator();
        }

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)validators).GetEnumerator();
        }
    }
}
