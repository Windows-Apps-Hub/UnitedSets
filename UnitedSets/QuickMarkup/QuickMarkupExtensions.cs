using System.Xml.Linq;
using Get.Data.Bindings;
using Get.UI.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Core;

namespace UnitedSets.QuickMarkup;

static class QuickMarkupExtensions
{
    extension<T>(IReadOnlyProperty<T> item)
    {
        public void ApplyAndRegisterForNewValue(Action<T> action)
        {
            item.ApplyAndRegisterForNewValue((x, _) => action(x));
        }
    }

    public static Reference<T> CreateReadOnlyRefrence<T>(this DependencyObject element, DependencyProperty property)
    {
        var r = Ref((T)element.GetValue(property));
        void SetValue()
        {
            r.Value = (T)element.GetValue(property);
        }
        element.RegisterPropertyChangedCallback(property, (_, _) => SetValue());
        return r;
    }
    extension<T>(T element) where T : DependencyObject
    {
        public void BindReferenceOneWayToSource<T2>(DependencyProperty property, Reference<T2> reference)
        {
            void SetValue()
            {
                reference.Value = (T2)element.GetValue(property);
            }
            SetValue();
            element.RegisterPropertyChangedCallback(property, (_, _) => SetValue());
        }
        public void BindReferenceOneWayToSource<T2, T3>(DependencyProperty property, Reference<T2> reference, Func<T3, T2> converter)
        {
            void SetValue()
            {
                reference.Value = converter((T3)element.GetValue(property));
            }
            SetValue();
            element.RegisterPropertyChangedCallback(property, (_, _) => SetValue());
        }
    }

    extension<T>(T element) where T : UIElement
    {
        public bool IsVisible
        {
            get => element.Visibility is Visibility.Visible;
            set => element.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    extension<T>(T element) where T : FrameworkElement
    {
        public double Canvas_Left
        {
            get => Canvas.GetLeft(element);
            set => Canvas.SetLeft(element, value);
        }
        public double Canvas_Top
        {
            get => Canvas.GetTop(element);
            set => Canvas.SetTop(element, value);
        }
        public int Grid_Row
        {
            get => Grid.GetRow(element);
            set => Grid.SetRow(element, value);
        }
        public int Grid_Column
        {
            get => Grid.GetColumn(element);
            set => Grid.SetColumn(element, value);
        }
        public int Grid_RowSpan
        {
            get => Grid.GetRowSpan(element);
            set => Grid.SetRowSpan(element, value);
        }
        public int Grid_ColumnSpan
        {
            get => Grid.GetColumnSpan(element);
            set => Grid.SetColumnSpan(element, value);
        }
        public int Canvas_ZIndex
        {
            get => Canvas.GetZIndex(element);
            set => Canvas.SetZIndex(element, value);
        }
        public bool DragRegion_Clickable
        {
            get => DragRegion.GetClickable(element);
            set => DragRegion.SetClickable(element, value);
        }
        //public CoreCursorType Cursor
        //{
        //    get => FrameworkElementExtensions.GetCursor(element);
        //    set => FrameworkElementExtensions.SetCursor(element, value);
        //}
        public object Tooltip
        {
            get => ToolTipService.GetToolTip(element);
            set => ToolTipService.SetToolTip(element, value);
        }
        public T Center()
        {
            element.HorizontalAlignment = HorizontalAlignment.Center;
            element.VerticalAlignment = VerticalAlignment.Center;
            return element;
        }
        public T CenterH()
        {
            element.HorizontalAlignment = HorizontalAlignment.Center;
            return element;
        }
        public T CenterV()
        {
            element.VerticalAlignment = VerticalAlignment.Center;
            return element;
        }
        public T StretchH()
        {
            element.HorizontalAlignment = HorizontalAlignment.Stretch;
            return element;
        }
        public T StretchV()
        {
            element.VerticalAlignment = VerticalAlignment.Stretch;
            return element;
        }
        public T Left()
        {
            element.HorizontalAlignment = HorizontalAlignment.Left;
            return element;
        }
        public T Top()
        {
            element.VerticalAlignment = VerticalAlignment.Top;
            return element;
        }
        public T Bottom()
        {
            element.VerticalAlignment = VerticalAlignment.Bottom;
            return element;
        }
        public T Right()
        {
            element.HorizontalAlignment = HorizontalAlignment.Right;
            return element;
        }
    }
    extension(Border element)
    {
        public Border FullRounded()
        {
            element.SizeChanged += FullRoundedSizeChangedHandler;
            FullRoundedSizeChangedHandler(element, null);
            return element;
        }
    }
    extension(RowDefinition rd)
    {
        public void Auto()
        {
            rd.Height = GridLength.Auto;
        }
    }
    extension(ColumnDefinition rd)
    {
        public void Auto()
        {
            rd.Width = GridLength.Auto;
        }
    }
    extension<T>(T element) where T : Control
    {
        public T FullRounded()
        {
            element.SizeChanged += FullRoundedSizeChangedHandler;
            FullRoundedSizeChangedHandler(element, null);
            return element;
        }
    }
    private static void FullRoundedSizeChangedHandler(object sender, SizeChangedEventArgs? e)
    {
        if (sender is FrameworkElement ele)
        {
            double radius;
            if (e is not null)
                radius = Math.Min(e.NewSize.Width, e.NewSize.Height) / 2;
            else
                radius = Math.Min(ele.ActualWidth, ele.ActualHeight) / 2;
            if (ele is Control control)
            {
                control.CornerRadius = new(radius);
            }
            else if (ele is Border border)
            {
                border.CornerRadius = new(radius);
            }
        }
    }
    extension<T>(IReadOnlyBinding<T> prop)
    {
        public Reference<T> CreateReadOnlyReference()
        {
            var r = new Reference<T>(prop.CurrentValue);
            prop.ValueChanged += (_, val) => r.Value = val;
            return r;
        }
    }
    public static void FirstLoadedEv(this FrameworkElement element, Action ev)
    {
        element.Loaded += Element_Loaded;
        void Element_Loaded(object sender, RoutedEventArgs e)
        {
            element.Loaded -= Element_Loaded;
            ev();
        }
    }
}
