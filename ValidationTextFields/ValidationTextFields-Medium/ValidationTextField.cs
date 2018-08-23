using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;

namespace ValidationTextFields_Test
{
    /// <summary>
    /// Validation Text Field
    /// </summary>
    public class ValidationTextField
    {
        private BorderedTextField _textField;
        private UILabel _errorLabel;
        private ValidationState _state = ValidationState.Neutral;
        private List<Trigger> _neutralTriggers = new List<Trigger>();
        private List<Trigger> _errorTriggers = new List<Trigger>();

        /// <summary>
        /// Neutral state border color
        /// </summary>
        public CGColor NeutralStateColor { get; set; }

        /// <summary>
        /// Valid state border color
        /// </summary>
        public CGColor ValidStateColor { get; set; }

        /// <summary>
        /// Error state border color
        /// </summary>
        public CGColor ErrorStateColor { get; set; }

        /// <summary>
        /// Editing border color
        /// </summary>
        public CGColor EditingColor { get; set; }

        /// <summary>
        /// Sets the error label's font and resizes the frame
        /// </summary>
        public UIFont ErrorFont
        {
            get
            {
                return _errorLabel.Font;
            }
            set
            {
                _errorLabel.Font = value;
                Reframe();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="textField">A bordered text field</param>
        public ValidationTextField(BorderedTextField textField)
        {
            _textField = textField;
            NeutralStateColor = ValidStateColor = ErrorStateColor = _textField.BorderColor;

            EditingColor = _textField.BorderColor;
            _textField.TextField.EditingDidBegin += EditingBegan;
            _textField.TextField.EditingDidEnd += EditingEnded;

            var superview = _textField.TextField.Superview;
            _errorLabel = new UILabel();
            _errorLabel.TextAlignment = UITextAlignment.Center;
            ErrorFont = _textField.TextField.Font;
            superview.AddSubviews(_errorLabel);
        }

        /// <summary>
        /// Validates the text field text
        /// </summary>
        /// <returns>The update state of the text field</returns>
        public async Task<ValidationState> Validate()
        {
            var trigger = await RunTriggers();
            Update(trigger.State, trigger.Message);
            return trigger.State;
        }

        /// <summary>
        /// Adds a neutral state validation trigger
        /// </summary>
        /// <param name="function">The trigger function</param>
        public void AddNeutralTrigger(Func<string, bool> function)
        {
            _neutralTriggers.Add(new Trigger(ValidationState.Neutral, function));
        }

        /// <summary>
        /// Adds an error state validation trigger
        /// </summary>
        /// <param name="function">The trigger function</param>
        /// <param name="message">A validation message</param>
        public void AddErrorTrigger(Func<string, bool> function, string message)
        {
            _errorTriggers.Add(new Trigger(ValidationState.Error, function, message));
        }

        /// <summary>
        /// Resizes the error label
        /// </summary>
        public void Reframe()
        {
            _errorLabel.Frame = new CGRect(_textField.TextField.Frame.X, _textField.TextField.Frame.Bottom + 5, _textField.TextField.Frame.Width, _errorLabel.Font.LineHeight);
        }

        /// <summary>
        /// Text field editing began event handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event arguments</param>
        private void EditingBegan(object sender, EventArgs args)
        {
            _textField.BorderColor = EditingColor;
            _textField.Color();
        }

        /// <summary>
        /// Text field editing ended event handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Event arguments</param>
        private async void EditingEnded(object sender, EventArgs args)
        {
            await Validate();
        }

        /// <summary>
        /// Updates the current state and recolors.
        /// </summary>
        /// <param name="state">The new validation state</param>
        /// <param name="message">The validation message (can be null)</param>
        private void Update(ValidationState state, string message)
        {
            _state = state;
            _errorLabel.Text = message;
            _errorLabel.Hidden = string.IsNullOrWhiteSpace(message);
            switch (state)
            {
                case ValidationState.Valid:
                    _textField.BorderColor = ValidStateColor;
                    break;
                case ValidationState.Error:
                    _textField.BorderColor = ErrorStateColor;
                    break;
                default:
                case ValidationState.Neutral:
                    _textField.BorderColor = NeutralStateColor;
                    break;
            }

            _errorLabel.TextColor = UIColor.FromCGColor(_textField.BorderColor);
            _textField.TextField.Superview.InvokeOnMainThread(() => _textField.Color());
        }

        /// <summary>
        /// Runs all triggers starting with neutral, then error. Everything else is considered valid.
        /// </summary>
        /// <returns>A matched or valid state trigger.</returns>
        private async Task<Trigger> RunTriggers()
        {
            var text = _textField.TextField.Text;
            foreach (var trigger in _neutralTriggers)
            {
                if (await Task.Run(() => trigger.TriggerFunction(text)))
                {
                    return trigger;
                }
            }
            foreach (var trigger in _errorTriggers)
            {
                if (await Task.Run(() => trigger.TriggerFunction(text)))
                {
                    return trigger;
                }
            }

            return Trigger.Valid;
        }
    }

    /// <summary>
    /// Validation State
    /// </summary>
    public enum ValidationState
    {
        Valid,
        Error,
        Neutral
    }

    /// <summary>
    /// A function that causes a specified validation state and validation message
    /// </summary>
    internal class Trigger
    {
        /// <summary>
        /// A valid state trigger
        /// </summary>
        public static Trigger Valid { get => new Trigger(ValidationState.Valid); }

        /// <summary>
        /// The validation state this trigger causes
        /// </summary>
        public ValidationState State;

        /// <summary>
        /// The trigger function
        /// </summary>
        public Func<string, bool> TriggerFunction;

        /// <summary>
        /// The validation message
        /// </summary>
        public string Message;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="state">The validation state</param>
        /// <param name="triggerFunction">The function</param>
        /// <param name="message">A n optional validation message</param>
        public Trigger(ValidationState state, Func<string, bool> triggerFunction, string message = null)
        {
            State = state;
            TriggerFunction = triggerFunction;
            Message = message;
        }

        /// <summary>
        /// Internal Constructor for valid state triggers
        /// </summary>
        /// <param name="state">The validation state</param>
        private Trigger(ValidationState state)
        {
            State = state;
        }
    }
}
