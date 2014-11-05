namespace WPFValidationPack
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
using System.Linq.Expressions;

    public abstract class ValidationBase : ICanValidate, INotifyPropertyChanged
    {
        public abstract IEnumerable<IValidator> Validators { get;  }

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

        public void RaiseErrorsChangedEvent(DataErrorsChangedEventArgs e)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, e);
        }


        public void RaisePropertyChangedEvent(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        private string GetPropertyName(LambdaExpression lambda)
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
    }
}
