namespace WPFValidationPack
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public static class ValidationService
    {
        public static bool HasErrors(ICanValidate source)
        {
            return source.Validators.Any(v => v.LastStatus);
        }

        public static IEnumerable GetErrors(ICanValidate source, string propertyName)
        {
            return source.Validators.Where(v => v.PropertyName == propertyName && v.LastStatus).Select(v => v.ErrorMessage);
        }

        public static void Validate(ICanValidate source)
        {
            DoValidation(source, source.Validators);
        }

        public static void Validate(ICanValidate source, string propertyName)
        {
            var validators = source.Validators.Where(v => v.PropertyName == propertyName);
            DoValidation(source, validators);
        }

        private static void DoValidation(ICanValidate source, IEnumerable<IValidator> validators)
        {
            foreach (var validator in validators)
            {
                bool hasError = validator.Check();

                if (hasError != validator.LastStatus)
                {
                    validator.LastStatus = hasError;

                    source.RaiseErrorsChangedEvent(new DataErrorsChangedEventArgs(validator.PropertyName));
                }
            }
        }
    }
}
