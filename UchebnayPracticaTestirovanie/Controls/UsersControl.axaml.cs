using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using UchebnayPracticaTestirovanie.Data;
using UchebnayPracticaTestirovanie.Models;
using UchebnayPracticaTestirovanie.Views;

namespace UchebnayPracticaTestirovanie.Controls;

public partial class UsersControl : UserControl
{
    public UsersControl()
    {
        InitializeComponent();
        if (Session.Role != "Менеджер")
        {
            AddBtn.IsVisible = false;
            EditBtn.IsVisible = false;
            DelBtn.IsVisible = false;
        }
        LoadData();
    }

    private void LoadData()
    {
        using var db = new AppDbContext();
        var list = db.Users.Include(x => x.Role).ToList();
        if (!string.IsNullOrWhiteSpace(SearchBox.Text))
        {
            var s = SearchBox.Text.ToLower();
            list = list.Where(x => x.FullName.ToLower().Contains(s) || x.Login.ToLower().Contains(s) || x.Phone.Contains(s)).ToList();
        }

        Grid.ItemsSource = list.Select(x => new UserView
        {
            Id = x.Id,
            FullName = x.FullName,
            Phone = x.Phone,
            Login = x.Login,
            Role = x.Role.Name
        }).ToList();
    }

    private void Button_Click_Find(object? sender, RoutedEventArgs e) => LoadData();

    private async void Button_Click_Add(object? sender, RoutedEventArgs e)
    {
        TempVarible.selectedUser = null;
        var w = new UserWindow();
        await w.ShowDialog((Window)this.VisualRoot!);
        LoadData();
    }

    private async void Button_Click_Edit(object? sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not UserView row)
            return;
        using var db = new AppDbContext();
        TempVarible.selectedUser = db.Users.First(x => x.Id == row.Id);
        var w = new UserWindow();
        await w.ShowDialog((Window)this.VisualRoot!);
        LoadData();
    }

    private async void Grid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (EditBtn.IsVisible)
            Button_Click_Edit(sender, e);
    }

    private void Button_Click_Del(object? sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not UserView row)
            return;
        if (row.Id == Session.UserId)
            return;

        try
        {
            using var db = new AppDbContext();
            db.Users.Remove(db.Users.First(x => x.Id == row.Id));
            db.SaveChanges();
            LoadData();
        }
        catch
        {
        }
    }

    class UserView
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Login { get; set; } = "";
        public string Role { get; set; } = "";
    }
}
