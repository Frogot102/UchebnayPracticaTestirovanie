using Avalonia.Controls;
using Avalonia.Interactivity;
using UchebnayPracticaTestirovanie.Data;
using UchebnayPracticaTestirovanie.Models;

namespace UchebnayPracticaTestirovanie.Views;

public partial class UserWindow : Window
{
    public UserWindow()
    {
        InitializeComponent();
        using var db = new AppDbContext();
        var roles = db.Roles.ToList();
        RoleBox.ItemsSource = roles;
        RoleBox.DisplayMemberBinding = new Avalonia.Data.Binding("Name");

        if (TempVarible.selectedUser != null)
        {
            var u = db.Users.First(x => x.Id == TempVarible.selectedUser.Id);
            NameBox.Text = u.FullName;
            PhoneBox.Text = u.Phone;
            LoginBox.Text = u.Login;
            PassBox.Text = u.Password;
            RoleBox.SelectedItem = roles.FirstOrDefault(x => x.Id == u.RoleId);
        }
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameBox.Text) || string.IsNullOrWhiteSpace(LoginBox.Text) || RoleBox.SelectedItem is not Role role)
        {
            Err.Text = "Заполните ФИО, логин и тип";
            return;
        }
        if (TempVarible.selectedUser == null && string.IsNullOrWhiteSpace(PassBox.Text))
        {
            Err.Text = "Введите пароль";
            return;
        }

        try
        {
            using var db = new AppDbContext();
            User user;
            if (TempVarible.selectedUser == null)
                user = new User();
            else
                user = db.Users.First(x => x.Id == TempVarible.selectedUser.Id);

            user.FullName = NameBox.Text.Trim();
            user.Phone = PhoneBox.Text?.Trim() ?? "";
            user.Login = LoginBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(PassBox.Text))
                user.Password = PassBox.Text;
            user.RoleId = role.Id;

            if (TempVarible.selectedUser == null)
                db.Users.Add(user);

            db.SaveChanges();
            Close();
        }
        catch
        {
            Err.Text = "Такой логин уже есть";
        }
    }
}
