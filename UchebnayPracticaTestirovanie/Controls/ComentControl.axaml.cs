using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using UchebnayPracticaTestirovanie.Data;
using UchebnayPracticaTestirovanie.Models;
using UchebnayPracticaTestirovanie.Views;

namespace UchebnayPracticaTestirovanie.Controls;

public partial class ComentControl : UserControl
{
    public ComentControl()
    {
        InitializeComponent();
        if (Session.Role == "Заказчик")
            AddBtn.IsVisible = false;
        if (Session.Role != "Менеджер")
            DelBtn.IsVisible = false;
        LoadData();
    }

    private void LoadData()
    {
        using var db = new AppDbContext();
        var list = db.Comments.Include(x => x.Master).Include(x => x.Request).ToList();

        if (Session.Role == "Заказчик")
            list = list.Where(x => x.Request.ClientId == Session.UserId).ToList();
        else if (Session.Role == "Техник")
            list = list.Where(x => x.Request.MasterId == Session.UserId || x.Request.MasterId == null || x.MasterId == Session.UserId).ToList();

        if (!string.IsNullOrWhiteSpace(SearchBox.Text))
        {
            var s = SearchBox.Text.ToLower();
            list = list.Where(x => x.Message.ToLower().Contains(s) || x.Master.FullName.ToLower().Contains(s)).ToList();
        }

        Grid.ItemsSource = list.Select(x => new ComentView
        {
            Id = x.Id,
            RequestId = x.RequestId,
            Master = x.Master.FullName,
            Message = x.Message
        }).ToList();
    }

    private void Button_Click_Find(object? sender, RoutedEventArgs e)
    {
        LoadData();
    }

    private async void Button_Click_Add(object? sender, RoutedEventArgs e)
    {
        var w = new ComentWindow();
        await w.ShowDialog((Window)this.VisualRoot!);
        LoadData();
    }

    private void Button_Click_Del(object? sender, RoutedEventArgs e)
    {
        if (Grid.SelectedItem is not ComentView row)
            return;

        using var db = new AppDbContext();
        db.Comments.Remove(db.Comments.First(x => x.Id == row.Id));
        db.SaveChanges();
        LoadData();
    }

    class ComentView
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string Master { get; set; } = "";
        public string Message { get; set; } = "";
    }
}
