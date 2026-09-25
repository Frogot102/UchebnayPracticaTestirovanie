using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using UchebnayPracticaTestirovanie.Data;
using UchebnayPracticaTestirovanie.Models;
using UchebnayPracticaTestirovanie.Views;

namespace UchebnayPracticaTestirovanie.Controls;

public partial class ZayavkiControl : UserControl
{
    public ZayavkiControl()
    {
        InitializeComponent();
        StatusBox.Items.Add("Все");
        StatusBox.Items.Add("Новая заявка");
        StatusBox.Items.Add("В процессе ремонта");
        StatusBox.Items.Add("Готова к выдаче");
        StatusBox.SelectedIndex = 0;

        if (Session.Role == "Заказчик" || Session.Role == "Техник")
            AddBtn.IsVisible = false;
        if (Session.Role == "Заказчик")
            EditBtn.IsVisible = false;
        if (Session.Role != "Менеджер")
            DelBtn.IsVisible = false;

        LoadData();
    }

    private void LoadData()
    {
        using var db = new AppDbContext();
        var list = db.Requests.Include(x => x.Client).Include(x => x.Master).ToList();

        if (Session.Role == "Заказчик")
            list = list.Where(x => x.ClientId == Session.UserId).ToList();
        else if (Session.Role == "Техник")
            list = list.Where(x => x.MasterId == Session.UserId || x.MasterId == null).ToList();

        if (StatusBox.SelectedIndex > 0)
        {
            var st = StatusBox.SelectedItem.ToString();
            list = list.Where(x => x.Status == st).ToList();
        }

        if (!string.IsNullOrWhiteSpace(SearchBox.Text))
        {
            var s = SearchBox.Text.ToLower();
            list = list.Where(x =>
                x.TechModel.ToLower().Contains(s) ||
                x.Problem.ToLower().Contains(s) ||
                x.Client.FullName.ToLower().Contains(s)).ToList();
        }

        Grid.ItemsSource = list.Select(x => new RequestView
        {
            Id = x.Id,
            Start = x.StartDate.ToString("dd.MM.yyyy"),
            TechType = x.TechType,
            Model = x.TechModel,
            Problem = x.Problem,
            Status = x.Status,
            Master = x.Master == null ? "" : x.Master.FullName,
            Client = x.Client.FullName
        }).ToList();
    }

    private void Button_Click_Find(object? sender, RoutedEventArgs e)
    {
        LoadData();
    }

    private async void Button_Click_Add(object? sender, RoutedEventArgs e)
    {
        TempVarible.selectedRequest = null;
        var w = new ZayavkaWindow();
        await w.ShowDialog((Window)this.VisualRoot!);
        LoadData();
    }

    private async void Button_Click_Edit(object? sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not RequestView row)
            return;

        using var db = new AppDbContext();
        TempVarible.selectedRequest = db.Requests.First(x => x.Id == row.Id);
        var w = new ZayavkaWindow();
        await w.ShowDialog((Window)this.VisualRoot!);
        LoadData();
    }

    private async void Grid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (EditBtn.IsVisible)
            Button_Click_Edit(sender, e);
    }

    private async void Button_Click_Del(object? sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not RequestView row)
            return;

        using var db = new AppDbContext();
        var comments = db.Comments.Where(x => x.RequestId == row.Id);
        db.Comments.RemoveRange(comments);
        db.Requests.Remove(db.Requests.First(x => x.Id == row.Id));
        db.SaveChanges();
        LoadData();
    }

    class RequestView
    {
        public int Id { get; set; }
        public string Start { get; set; } = "";
        public string TechType { get; set; } = "";
        public string Model { get; set; } = "";
        public string Problem { get; set; } = "";
        public string Status { get; set; } = "";
        public string Master { get; set; } = "";
        public string Client { get; set; } = "";
    }
}
