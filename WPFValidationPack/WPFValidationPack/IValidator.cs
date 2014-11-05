namespace WPFValidationPack
{
    using System;

    public interface IValidator
    {
        string PropertyName { get; }

        bool LastStatus { get; set; }

        Func<bool> Check { get; set; }

        string ErrorMessage { get; }
    }
}
