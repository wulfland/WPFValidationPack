namespace WPFValidationPack.SampleApp.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using WPFValidationPack.SampleApp.Model;


    public class MainViewModel : ValidationBase
    {
        IList<IValidator> validators = new List<IValidator>();

        public MainViewModel()
        {
            SaveCommand = new RelayCommand(Save, CanSave);

            // Initialize Models that handle there own validation
            ModelWithBase = new MyModelWithBase();
            ModelWithoutBase = new MyModelWithoutBase();

            // Add data annotation validation
            foreach (var validator in new DataAnnotationValidatorCollection<MainViewModel>(this))
            {
                validators.Add(validator);
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

        public MyModelWithBase ModelWithBase
        {
            get { return GetValue(() => ModelWithBase); }
            set { SetValue(() => ModelWithBase, value); }
        }

        public MyModelWithoutBase ModelWithoutBase
        {
            get { return GetValue(() => ModelWithoutBase); }
            set { SetValue(() => ModelWithoutBase, value); }
        }

        [Required]
        [StringLength(20)]
        public string Name
        {
            get { return GetValue(() => Name); }
            set { SetValue(() => Name, value); }
        }

        [Required(ErrorMessage = "We need a valid email to be able to send you a confirmation code.")]
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

        public ICommand SaveCommand { get; set; }

        public bool CanSave(object para)
        {
            return !HasErrors;
        }

        public void Save(object para)
        {
            ValidateAll();

            if (!HasErrors)
            {
                // save data
            }
        }

        public override IEnumerable<IValidator> Validators
        {
            get { return validators; }
        }
    }
}
