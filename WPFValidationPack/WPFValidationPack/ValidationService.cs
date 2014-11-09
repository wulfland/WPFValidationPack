namespace WPFValidationPack
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class ValidationService
    {
        public static bool HasErrors(ICanValidate source)
        {
            bool hasErrors = false;

            DoForAllNested(source, (v) => hasErrors = hasErrors || v.Validators.Any(x => x.LastStatus));

            return hasErrors;
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

        public static void ValidateAll(ICanValidate parent)
        {
            DoForAllNested(parent, (v) => DoValidation(v, v.Validators));
        }

        private static void DoValidation(ICanValidate source, IEnumerable<IValidator> validators)
        {
            foreach (var validator in validators)
            {
                bool hasError = validator.Check();

                if (hasError != validator.LastStatus)
                {
                    validator.LastStatus = hasError;

                    source.RaiseErrorsChangedEvent(validator.PropertyName);
                }
            }
        }

        private static void DoForAllNested(ICanValidate parent, Action<ICanValidate> action)
        {
            if (parent == null)
                return;

            action(parent);

            var i = typeof(ICanValidate);
            var properties = parent.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (i.IsAssignableFrom(property.PropertyType))
                {
                    var child = property.GetValue(parent) as ICanValidate;
                    DoForAllNested(child, action);
                }
            }
        }
    }
}
