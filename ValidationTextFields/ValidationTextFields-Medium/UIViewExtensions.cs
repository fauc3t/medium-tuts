using CoreAnimation;
using CoreGraphics;
using System;
using System.Linq;
using UIKit;

namespace ValidationTextFields_Test
{
    /// <summary>
    /// UIView Extension methods
    /// </summary>
    public static class UIViewExtensions
    {
        /// <summary>
        /// Border naming prefix
        /// </summary>
        private static readonly string COLOR_BORDERS_EXT = "color-borders-ext";

        /// <summary>
        /// All border directions excluding .All
        /// </summary>
        private static readonly BorderDirection[] ALL_BORDERS = new BorderDirection[] { BorderDirection.Bottom, BorderDirection.Left, BorderDirection.Right, BorderDirection.Top };

        /// <summary>
        /// Extension method for coloring a UIView's borders
        /// </summary>
        /// <param name="view">The UIView being operated on</param>
        /// <param name="color">Border color</param>
        /// <param name="width">Border width</param>
        /// <param name="direction">Border directions to color</param>
        public static void ColorBorders(this UIView view, CGColor color, nfloat width, BorderDirection direction)
        {
            // Loop through the four possible borders
            foreach (var border in ALL_BORDERS)
            {
                // Checks that the current border should be colored
                if ((border & direction) == border)
                {
                    // Sublayer name
                    var name = $"{COLOR_BORDERS_EXT}.{border.ToString()}";
                    // Attempt to find sublayer by name
                    var sublayer = view.Layer.Sublayers?
                       .FirstOrDefault(layer => layer.Name != null &&
                    layer.Name.Equals(name));
                    // If a sublayer does not already exist, create and add it
                    if (sublayer == null)
                    {
                        sublayer = new CALayer();
                        view.Layer.AddSublayer(sublayer);
                    }

                    sublayer.Frame = GetBorderFrame(border); // Border Frame
                    sublayer.Name = name; // Name the sublayer
                    sublayer.BorderWidth = width; // Set the border width
                    sublayer.BorderColor = color; // Set the border color
                }
            }

            // Gets the border frame based on the direction provided
            CGRect GetBorderFrame(BorderDirection border)
            {
                switch (border)
                {
                    case BorderDirection.Top:
                        return new CGRect(0, 0, view.Frame.Width, width);
                    case BorderDirection.Bottom:
                        return new CGRect(0, view.Frame.Size.Height - width, view.Frame.Width, width);
                    case BorderDirection.Left:
                        return new CGRect(0, 0, width, view.Frame.Height);
                    case BorderDirection.Right:
                        return new CGRect(view.Frame.Width - width, 0, width, view.Frame.Height);
                    default:
                        throw new NotSupportedException("That border direction is not supported");
                }
            }
        }

        /// <summary>
        /// Extension method for resignigning focus of the first responder when the view is tapped
        /// </summary>
        /// <param name="view">The view being operated on</param>
        public static void ResignFirstResponderOnTap(this UIView view)
        {
            var gesture = new UITapGestureRecognizer(() => {
                view.EndEditing(true);
            });
            gesture.CancelsTouchesInView = false; //for iOS5
            view.AddGestureRecognizer(gesture);
        }
    }

    /// <summary>
    /// Border Directions
    /// </summary>
    public enum BorderDirection
    {
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8,
        All = 15
    }
}