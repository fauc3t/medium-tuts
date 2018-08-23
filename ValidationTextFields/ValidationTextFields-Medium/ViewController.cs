using HealthKit;
using System;
using System.Threading.Tasks;
using UIKit;

namespace ValidationTextFields_Test
{
    public partial class ViewController : UIViewController
    {
        /// <summary>
        /// The validation field
        /// </summary>
        private ValidationTextField _validationField;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        /// <summary>
        /// Override of ViewDidLoad
        /// </summary>
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            // Instantiate the bordered and validation text fields, set some properties and add triggers
            var borderedField = new BorderedTextField(NameTextField, NameTextField.TextColor.CGColor, BorderDirection.Bottom, 4);
            _validationField = new ValidationTextField(borderedField)
            {
                EditingColor = UIColor.FromRGB(149, 165, 166).CGColor,
                ErrorStateColor = UIColor.FromRGB(210, 77, 87).CGColor,
                ValidStateColor = UIColor.FromRGB(101, 198, 187).CGColor,
                ErrorFont = UIFont.SystemFontOfSize(10)
            };
            _validationField.AddNeutralTrigger(Empty);
            _validationField.AddErrorTrigger(IsLannister, "No Lannisters allowed!");

            // UIView extension to resign focus when the view controller is tapped
            View.ResignFirstResponderOnTap();
        }

        /// <summary>
        /// Override of ViewDidLayoutSubviews
        /// </summary>
        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            // Call Reframe() to set the error label size 
            // AFTER subviews have resized based on constraints
            _validationField.Reframe(); 
        }

        /// <summary>
        /// The text field is empty
        /// </summary>
        /// <param name="text">The validation text field's text</param>
        /// <returns>True if the field is null or empty, otherwise false.</returns>
        private bool Empty(string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        /// <summary>
        /// No Lannister's allowed. Except Tyrion.
        /// </summary>
        /// <param name="text">The validation text field's text</param>
        /// <returns>True if the text contains 'Lannister' without 'Tyrion', otherwise false.</returns>
        private bool IsLannister(string text)
        {
            return text.ToLower().Contains("lannister") && !text.ToLower().Contains("tyrion");
        }
    }
}