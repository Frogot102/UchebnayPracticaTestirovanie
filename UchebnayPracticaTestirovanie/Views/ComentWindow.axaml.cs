using Avalonia.Controls;
using Avalonia.Interactivity;
using UchebnayPracticaTestirovanie.Data;
using UchebnayPracticaTestirovanie.Models;

namespace UchebnayPracticaTestirovanie.Views;

public partial class ComentWindow : Window
{
    public ComentWindow()
    {
        InitializeComponent();
        using var db = new AppDbContext();
        var list = db.Requests.ToList();
        if (Session.Role == "Техник")
            list = list.Where(x => x.MasterId == Session.UserId || x.MasterId == null).ToList();

        ReqBox.ItemsSource = list.Select(x => new ReqItem { Id = x.Id, Title = "№" + x.Id + " " + x.TechModel }).ToList();
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        if (ReqBox.SelectedItem is not ReqItem req)
        {
            Err.Text = "Выберите заявку";
            return;
        }
        if (string.IsNullOrWhiteSpace(MsgBox.Text))
        {
            Err.Text = "Введите сообщение";
            return;
        }

        using var db = new AppDbContext();
        db.Comments.Add(new Comment
        {
            RequestId = req.Id,
            MasterId = Session.UserId,
            Message = MsgBox.Text.Trim()
        });
        db.SaveChanges();
        Close();
    }

    class ReqItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public override string ToString() => Title;
    }
}
