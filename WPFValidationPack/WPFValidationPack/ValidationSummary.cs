namespace WPFValidationPack
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    [TemplatePart(Name = "ValidationErrorList", Type = typeof(ItemsControl))]
    public class ValidationSummary : Control
    {
        ItemsControl errorList;

        static ValidationSummary()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ValidationSummary), new FrameworkPropertyMetadata(typeof(ValidationSummary)));
            WidthProperty.OverrideMetadata(typeof(ValidationSummary), new FrameworkPropertyMetadata(300.0));
        }

        public static readonly DependencyProperty ValidationScopeProperty =
            DependencyProperty.Register("ValidationScope", typeof(FrameworkElement), typeof(ValidationSummary), new FrameworkPropertyMetadata(OnValidationScopeChanged));

        public ObservableCollection<ValueCommand<ValidationError>> ActiveErrors { get; private set; }

        public override void OnApplyTemplate()
        {
            errorList = GetTemplateChild<ItemsControl>("ValidationErrorList");

            ActiveErrors = new ObservableCollection<ValueCommand<ValidationError>>();
            var binding = new Binding("ActiveErrors") { Source = this, NotifyOnValidationError = true };
            errorList.SetBinding(ItemsControl.ItemsSourceProperty, binding);

        }

        public FrameworkElement ValidationScope
        {
            get { return (FrameworkElement)GetValue(ValidationScopeProperty); }
            set { SetValue(ValidationScopeProperty, value); }
        }


        static void OnValidationScopeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var validationSummary = d as ValidationSummary;

            if (e.OldValue != null)
                Validation.RemoveErrorHandler((DependencyObject)e.OldValue, validationSummary.OnValidationError);

            if (e.NewValue != null)
            {
                validationSummary.Visibility = Visibility.Collapsed;

                Validation.AddErrorHandler((DependencyObject)e.NewValue, validationSummary.OnValidationError);
            }
        }

        void OnValidationError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                ActiveErrors.Add(new ValueCommand<ValidationError>(MoveFocus, e.Error));
            }
            else
            {
                ActiveErrors.Remove(ActiveErrors.First(x => x.Value == e.Error));
            }

            if (ActiveErrors.Count > 0)
            {
                this.Visibility = Visibility.Visible;
            }
            else
            {
                this.Visibility = Visibility.Collapsed;
            }
        }

        void MoveFocus(object error)
        {
            var validationError = error as ValidationError;

            var targetControl = ((BindingExpression)validationError.BindingInError).Target as IInputElement;
            if (targetControl != null)
            {
                targetControl.Focus();
            }
        }

        T GetTemplateChild<T>(string name) where T : class
        {
            object obj = GetTemplateChild(name);

            if (obj == null)
                return null;

            if (obj is T)
                return (T)obj;

            throw new InvalidOperationException();
        }
    }
}
