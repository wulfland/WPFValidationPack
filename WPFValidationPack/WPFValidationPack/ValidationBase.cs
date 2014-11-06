namespace WPFValidationPack
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;

    public abstract class ValidationBase : ICanValidate, INotifyPropertyChanged
    {
        Dictionary<string, object> propertyValueStorage = new Dictionary<string, object>();

        public abstract IEnumerable<IValidator> Validators { get; }

        public void Validate()
        {
            ValidationService.Validate(this);
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return ValidationService.GetErrors(this, propertyName);
        }

        public bool HasErrors
        {
            get
            {
                return ValidationService.HasErrors(this);
            }
        }

        public void Validate(string propertyName)
        {
            ValidationService.Validate(this, propertyName);
        }

        public void Validate<T>(Expression<Func<T>> property)
        {
            var lambda = property as LambdaExpression;

            if (lambda == null)
                throw new ArgumentException("Invalid lambda expression.");

            var propertyName = GetPropertyName(lambda);

            Validate(propertyName);
        }

        public void RaiseErrorsChangedEvent(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }


        public void RaisePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected T GetValue<T>(Expression<Func<T>> property)
        {
            var lambda = property as LambdaExpression;

            if (lambda == null)
                throw new ArgumentException("Invalid lambda expression.");

            var propertyName = GetPropertyName(lambda);

            return GetValueInternal<T>(propertyName);
        }

        protected void SetValue<T>(Expression<Func<T>> property, T value)
        {
            var lambdaExpression = property as LambdaExpression;

            if (lambdaExpression == null)
                throw new ArgumentException("Invalid lambda expression.");

            var propertyName = GetPropertyName(lambdaExpression);

            T storedValue = GetValueInternal<T>(propertyName);

            if (!value.Equals(storedValue))
            {
                this.propertyValueStorage[propertyName] = value;

                RaisePropertyChangedEvent(propertyName);
                Validate(propertyName);
            }

        }

        private static string GetPropertyName(LambdaExpression lambda)
        {
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                var memberExpression = unaryExpression.Operand as MemberExpression;

                return memberExpression.Member.Name;
            }
            else
            {
                var memberExpression = lambda.Body as MemberExpression;
                return memberExpression.Member.Name;
            }
        }

        private T GetValueInternal<T>(string propertyName)
        {
            object value;
            return propertyValueStorage.TryGetValue(propertyName, out value) ? (T)value : default(T);
        }
    }
}
