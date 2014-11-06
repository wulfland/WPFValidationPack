namespace WPFValidationPack.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ValidationServiceTest : ICanValidate
    {
        IList<IValidator> validators;

        [TestInitialize]
        public void SetUp()
        {
            validators = new List<IValidator>();
            validators.Add(new CustomValidator
            {
                PropertyName = "PhoneNumberProperty",
                ErrorMessage = "Only Numbers are allowed",
                Check = () => !IsValidPhoneNumber(PhoneNumberProperty)
            });
            validators.Add(new CustomValidator
            {
                PropertyName = "RequiredTextProperty",
                ErrorMessage = "'RequiredTextProperty' is required.",
                Check = () => string.IsNullOrWhiteSpace(RequiredTextProperty)
            });
        }

        [TestMethod]
        public void ValidationService_HasErrors_Test()
        {
            Validate();
            HasErrors.Should().BeTrue("required field is missing");

            RequiredTextProperty = "a text";
            Validate();
            HasErrors.Should().BeFalse("required field is provided");

            PhoneNumberProperty = "invalid phone number";
            Validate();
            HasErrors.Should().BeTrue("phone number is invalid");

            PhoneNumberProperty = "12345";
            Validate();
            HasErrors.Should().BeFalse("all fields are valid");
        }

        [TestMethod]
        public void ValidationService_GetErrors_Test()
        {
            Validate();
            var errors = GetErrors("RequiredTextProperty").Cast<string>();
            errors.Count().Should().Be(1);
            errors.First().Should().Be("'RequiredTextProperty' is required.");

            RequiredTextProperty = "a text";
            Validate();
            errors = GetErrors("RequiredTextProperty").Cast<string>();
            errors.Count().Should().Be(0);
        }

        [TestMethod]
        public void ValidationService_Can_Validate_Single_Property()
        {
            ValidationService.Validate(this, "PhoneNumberProperty");
            
            HasErrors.Should().BeFalse();

            ValidationService.Validate(this, "RequiredTextProperty");

            HasErrors.Should().BeTrue();
        }

        [TestMethod]
        public void ValidationService_ErrorsChangedEvent_Test()
        {
            int eventCalled = 0;
            this.ErrorsChanged += (sender, e) => eventCalled++;

            Validate();
            // required field missing
            eventCalled.Should().Be(1, "required field was not provided");

            Validate();
            eventCalled.Should().Be(1, "nothing has changed");

            RequiredTextProperty = "a text";
            Validate();
            eventCalled.Should().Be(2, "error was removed");
        }

        #region TestProperties

        public string PhoneNumberProperty { get; set; }

        public string RequiredTextProperty { get; set; }

        public bool IsValidPhoneNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true;

            int output;
            return int.TryParse(input, out output);
        }

        #endregion

        #region ICanValidate implementation

        public IEnumerable<IValidator> Validators
        {
            get
            {
                return validators;
            }
        }

        public void RaiseErrorsChangedEvent(string p)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(p));
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
