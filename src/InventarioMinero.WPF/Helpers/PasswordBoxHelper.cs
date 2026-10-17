using System.Windows;
using System.Windows.Controls;

namespace InventarioMinero.WPF.Helpers;

public static class PasswordBoxHelper
{
    public static readonly DependencyProperty BoundPasswordProperty =
        DependencyProperty.RegisterAttached(
            "BoundPassword",
            typeof(string),
            typeof(PasswordBoxHelper),
            new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

    public static readonly DependencyProperty BindPasswordProperty =
        DependencyProperty.RegisterAttached(
            "BindPassword",
            typeof(bool),
            typeof(PasswordBoxHelper),
            new PropertyMetadata(false, OnBindPasswordChanged));

    private static readonly DependencyProperty UpdatingPasswordProperty =
        DependencyProperty.RegisterAttached(
            "UpdatingPassword",
            typeof(bool),
            typeof(PasswordBoxHelper),
            new PropertyMetadata(false));

    public static string GetBoundPassword(DependencyObject obj) =>
        (string)obj.GetValue(BoundPasswordProperty);

    public static void SetBoundPassword(DependencyObject obj, string value) =>
        obj.SetValue(BoundPasswordProperty, value);

    public static bool GetBindPassword(DependencyObject obj) =>
        (bool)obj.GetValue(BindPasswordProperty);

    public static void SetBindPassword(DependencyObject obj, bool value) =>
        obj.SetValue(BindPasswordProperty, value);

    private static bool GetUpdatingPassword(DependencyObject obj) =>
        (bool)obj.GetValue(UpdatingPasswordProperty);

    private static void SetUpdatingPassword(DependencyObject obj, bool value) =>
        obj.SetValue(UpdatingPasswordProperty, value);

    private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PasswordBox box)
        {
            if ((bool)box.GetValue(BindPasswordProperty) == false)
                return;

            box.PasswordChanged -= HandlePasswordChanged;

            var newPassword = (string)e.NewValue;
            if (!GetUpdatingPassword(box))
            {
                box.Password = newPassword ?? string.Empty;
            }

            box.PasswordChanged += HandlePasswordChanged;
        }
    }

    private static void OnBindPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PasswordBox box)
        {
            var wasBound = (bool)e.OldValue;
            var needToBind = (bool)e.NewValue;

            if (wasBound)
            {
                box.PasswordChanged -= HandlePasswordChanged;
            }

            if (needToBind)
            {
                box.PasswordChanged += HandlePasswordChanged;
            }
        }
    }

    private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox box)
        {
            SetUpdatingPassword(box, true);
            SetBoundPassword(box, box.Password);
            SetUpdatingPassword(box, false);
        }
    }
}
