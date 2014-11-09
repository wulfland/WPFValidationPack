namespace WPFValidationPack.SampleApp.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// This is the most comfortable way to implement validation using the provided base class.
    /// </summary>
    public class MyModelWithBase : ValidationBase
    {
        IList<IValidator> validators = new List<IValidator>();

        public MyModelWithBase()
        {
            // Add a validator for each data annotation
            foreach (var dataAnnotationValidator in new DataAnnotationValidatorCollection<MyModelWithBase>(this))
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
            get { return GetValue(() => Name); }
            set { SetValue(() => Name, value); }
        }

        [Required(ErrorMessage="We need a valid email to be able to send you a confirmation code.")]
        [EmailAddress]
        public string Email
        {
            get { return GetValue(() => Email); }
            set { SetValue(() => Email, value); }
        }

        [Required]
        [EmailAddress]
        public string RepeatedEmail
        {
            get { return GetValue(() => RepeatedEmail); }
            set { SetValue(() => RepeatedEmail, value); }
        }

        [Phone]
        [StringLength(20)]
        public string Phone
        {
            get { return GetValue(() => Phone); }
            set { SetValue(() => Phone, value); }
        }

        [Phone]
        [StringLength(20)]
        public string Mobile
        {
            get { return GetValue(() => Mobile); }
            set { SetValue(() => Mobile, value); }
        }

        public override IEnumerable<IValidator> Validators
        {
            get { return validators; }
        }
    }
}
