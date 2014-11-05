namespace WPFValidationPack.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WPFValidationPack.Tests.Properties;

    [TestClass]
    public class DataAnnotationValidatorCollectionTest : ICanValidate
    {
        IList<IValidator> validators;

        [TestInitialize]
        public void SetUp()
        {
            validators = new DataAnnotationValidatorCollection<DataAnnotationValidatorCollectionTest>(this).ToList();
        }

        [TestMethod]
        public void DataAnnotationValidatorCollection_Test()
        {
            validators.Count().Should().Be(2, "2 attributes are set");

            Validate();
            HasErrors.Should().BeTrue("required field is missing");

            var errors = GetErrors("RequiredString").Cast<string>();
            errors.Count().Should().Be(1);
            errors.First().Should().Be(string.Format(Resources.RequiredMessage, "RequiredString"));

            RequiredString = "A text";
            Validate();
            HasErrors.Should().BeFalse("required field was provided");

            MailAddress = "invalid mail address";
            Validate();
            HasErrors.Should().BeTrue("the provided mail address is invalid");

            errors = GetErrors("MailAddress").Cast<string>();
            errors.Count().Should().Be(1);
            errors.First().Should().Be("Custom Error Message");

            MailAddress = "a@b.de";
            Validate();
            HasErrors.Should().BeFalse("mail address is valid");
        }

        [TestMethod]
        public void DataAnnotationValidatorCollection_Implements_IEnumeable()
        {
            IEnumerable enumerable = this.validators as IEnumerable;
            foreach (object obj in enumerable)
            {
                // do nothing
            }
            validators.Count().Should().Be(2, "2 attributes are set");
        }

        [Required]
        public string RequiredString { get; set; }

        [EmailAddress(ErrorMessage = "Custom Error Message")]
        public string MailAddress { get; set; }

        #region ICanValidate implementation

        public IEnumerable<IValidator> Validators
        {
            get
            {
                return validators;
            }
        }

        public void RaiseErrorsChangedEvent(DataErrorsChangedEventArgs e)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, e);
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

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

        public void Validate()
        {
            ValidationService.Validate(this);
        }
        #endregion

    }
}
