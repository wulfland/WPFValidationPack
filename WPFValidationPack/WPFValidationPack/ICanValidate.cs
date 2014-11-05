namespace WPFValidationPack
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public interface ICanValidate : INotifyDataErrorInfo
    {
        IEnumerable<IValidator> Validators { get; }

        void Validate();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        void RaiseErrorsChangedEvent(DataErrorsChangedEventArgs e);
    }
}
