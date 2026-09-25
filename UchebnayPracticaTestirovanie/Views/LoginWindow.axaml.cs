using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using UchebnayPracticaTestirovanie.Data;
using UchebnayPracticaTestirovanie.Models;

namespace UchebnayPracticaTestirovanie.Views;

public partial class LoginWindow : Window
{
    private bool _busy;

    public LoginWindow()
    {
        InitializeComponent();
    }

    private void Input_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            Login_Click(sender, e);
    }

    private void Login_Click(object? sender, RoutedEventArgs e)
    {
        if (_busy)
            return;

        var login = LoginBox.Text?.Trim() ?? "";
        var password = PasswordBox.Text ?? "";
        if (login.Length == 0 || password.Length == 0)
        {
            ErrorText.Text = "Введите логин и пароль.";
            return;
        }

        _busy = true;
        try
        {
            ScriptRunner.Run();
            using var db = new AppDbContext();
            var user = db.Users.Include(x => x.Role).FirstOrDefault(x => x.Login == login && x.Password == password);
            if (user is null)
            {
                ErrorText.Text = "Неверный логин или пароль.";
                return;
            }

            Session.UserId = user.Id;
            Session.FullName = user.FullName;
            Session.Role = user.Role.Name;

            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
                return;

            var main = new MainWindow();
            main.Show();
            Close();
            desktop.MainWindow = main;
        }
        catch (Exception ex)
        {
            ErrorText.Text = ex.Message;
        }
        finally
        {
            _busy = false;
        }
    }
}
