using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using UchebnayPracticaTestirovanie.Controls;
using UchebnayPracticaTestirovanie.Models;

namespace UchebnayPracticaTestirovanie.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        UserText.Text = Session.FullName + " (" + Session.Role + ")";
        if (Session.Role != "Менеджер" && Session.Role != "Оператор")
            UsersBtn.IsVisible = false;
        MainControl.Content = new ZayavkiControl();
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        MainControl.Content = new ZayavkiControl();
    }

    private void Button_Click_1(object? sender, RoutedEventArgs e)
    {
        MainControl.Content = new ComentControl();
    }

    private void Button_Click_2(object? sender, RoutedEventArgs e)
    {
        MainControl.Content = new UsersControl();
    }

    private void Logout_Click(object? sender, RoutedEventArgs e)
    {
        Session.Clear();
        var login = new LoginWindow();
        login.Show();
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = login;
        Close();
    }
}
