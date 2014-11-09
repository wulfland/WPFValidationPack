namespace WPFValidationPack.SampleApp.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;

    public class ReusableValidator : IValidator
    {
        ICollection<string> blacklistedWords = new Collection<string>() { "fuck", "shit", "ass" };

        public ReusableValidator(Expression<Func<string>> property)
        {
            var lambda = property as LambdaExpression;
            var member = lambda.Body as MemberExpression;

            PropertyName = member.Member.Name;
            Check = () => 
            {
                var value = property.Compile().Invoke();
                return blacklistedWords.Any(x => (value ?? string.Empty).ToUpperInvariant().Contains(x.ToUpperInvariant()));
            };
        }

        public Func<bool> Check { get; set; }

        public string ErrorMessage
        {
            get { return "The property '" + PropertyName + "' does contain illegal words."; }
        }

        public bool LastStatus { get; set; }

        public string PropertyName { get; private set; }
    }
}
