using CoreGraphics;
using System;
using UIKit;

namespace ValidationTextFields_Test
{
    /// <summary>
    /// A text field with colored borders
    /// </summary>
    public class BorderedTextField
    {
        /// <summary>
        /// The UITextField
        /// </summary>
        public UITextField TextField { get; set; }

        /// <summary>
        /// Border color
        /// </summary>
        public CGColor BorderColor { get; set; }

        /// <summary>
        /// Border directions to color
        /// </summary>
        public BorderDirection BorderDirection { get; set; }

        /// <summary>
        /// Border width
        /// </summary>
        public nfloat BorderWidth { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="textField">A UITextField</param>
        /// <param name="color">The desired border color</param>
        /// <param name="direction">The border directions to color</param>
        /// <param name="borderWidth">The desired border width</param>
        public BorderedTextField(UITextField textField, CGColor color, BorderDirection direction, nfloat borderWidth)
        {
            TextField = textField;
            BorderColor = color;
            BorderDirection = direction;
            BorderWidth = borderWidth;
            Color();
        }

        /// <summary>
        /// Draws/Redraws borders based on the current properties
        /// </summary>
        public void Color()
        {
            TextField.ColorBorders(BorderColor, BorderWidth, BorderDirection);
        }
    }
}