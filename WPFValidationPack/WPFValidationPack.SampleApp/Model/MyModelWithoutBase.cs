namespace WPFValidationPack.SampleApp.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// If you cannot use the base class you can easily implement the interface yourself by using the validation service.
    /// </summary>
    public class MyModelWithoutBase : ICanValidate, INotifyPropertyChanged
    {
        IList<IValidator> validators = new List<IValidator>();

        public MyModelWithoutBase()
        {
            // Add a validator for each data annotation
            foreach (var dataAnnotationValidator in new DataAnnotationValidatorCollection<MyModelWithoutBase>(this))
            {
                validators.Add(dataAnnotationValidator);
            }

            // Add custom validators
            validators.Add(new CustomValidator
            {
                PropertyName = "RepeatedEmail",
                ErrorMessage = "The two email addresses do not match.",
                Check = () => Email != RepeatedEmail
            });

            // Add reusable validators
            validators.Add(new ReusableValidator(() => Name));
        }

        [Required]
        [StringLength(20)]
        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    RaisePropertyChanged("Name");
                    ValidationService.Validate(this, "Name");
                }
            }
        }
        string name;

        [Required(ErrorMessage = "We need a valid email to be able to send you a confirmation code.")]
        [EmailAddress]
        public string Email
        {
            get { return email; }
            set
            {
                if (value != email)
                {
                    email = value;
                    RaisePropertyChanged("Email");
                    ValidationService.Validate(this, "Email");
                }
            }
        }
        string email;

        [Required]
        [EmailAddress]
        public string RepeatedEmail
        {
            get { return repeatedEmail; }
            set
            {
                if (value != repeatedEmail)
                {
                    repeatedEmail = value;
                    RaisePropertyChanged("RepeatedEmail");
                    ValidationService.Validate(this, "RepeatedEmail");
                }
            }
        }
        string repeatedEmail;

        [Phone]
        [StringLength(20)]
        public string Phone
        {
            get { return phone; }
            set
            {
                if (value != phone)
                {
                    phone = value;
                    RaisePropertyChanged("Phone");
                    ValidationService.Validate(this, "Phone");
                }
            }
        }
        string phone;

        [Phone]
        [StringLength(20)]
        public string Mobile
        {
            get { return mobile; }
            set
            {
                if (value != mobile)
                {
                    mobile = value;
                    RaisePropertyChanged("Mobile");
                    ValidationService.Validate(this, "Mobile");
                }
            }
        }
        string mobile;

        #region ICanValidate

        public IEnumerable<IValidator> Validators
        {
            get { return validators; }
        }

        public void RaiseErrorsChangedEvent(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public void Validate()
        {
            ValidationService.Validate(this);
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return ValidationService.GetErrors(this, propertyName);
        }

        public bool HasErrors
        {
            get { return ValidationService.HasErrors(this); }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
