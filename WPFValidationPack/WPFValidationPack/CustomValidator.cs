namespace WPFValidationPack
{
    using System;

    public class CustomValidator : IValidator
    {
        public string PropertyName { get; set; }

        public string ErrorMessage { get; set; }

        public bool LastStatus { get; set; }

        public Func<bool> Check { get; set; }
    }
}
