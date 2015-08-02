using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sakuya_Aki
{
    public class MyBehavior : Behavior<UIElement>
    {
        private Canvas canvas;

        private bool isDragging = false;

        private Point mouseOffset;
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseLeftButtonDown +=

                new System.Windows.Input.MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown);

            AssociatedObject.MouseMove +=

                new System.Windows.Input.MouseEventHandler(AssociatedObject_MouseMove);

            AssociatedObject.MouseLeftButtonUp +=

                new System.Windows.Input.MouseButtonEventHandler(AssociatedObject_MouseRightButtonUp);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseLeftButtonDown -=

                new System.Windows.Input.MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown);

            AssociatedObject.MouseMove -=

                new System.Windows.Input.MouseEventHandler(AssociatedObject_MouseMove);

            AssociatedObject.MouseLeftButtonUp -=

                new System.Windows.Input.MouseButtonEventHandler(AssociatedObject_MouseRightButtonUp);
        }

        void AssociatedObject_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                AssociatedObject.ReleaseMouseCapture();

                isDragging = false;
            }
        }

        void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isDragging)
            {
                Point point = e.GetPosition(canvas);
                AssociatedObject.SetValue(Canvas.TopProperty, point.Y - mouseOffset.Y);
                AssociatedObject.SetValue(Canvas.LeftProperty, point.X - mouseOffset.X);
            }
        }

        void AssociatedObject_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (canvas == null)
            {
                canvas = VisualTreeHelper.GetParent(AssociatedObject) as Canvas;
            }

            isDragging = true;

            mouseOffset = e.GetPosition(AssociatedObject);

            AssociatedObject.CaptureMouse();
        }
    }
}
