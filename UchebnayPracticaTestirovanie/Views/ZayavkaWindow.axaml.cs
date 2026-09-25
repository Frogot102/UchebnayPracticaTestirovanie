using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using UchebnayPracticaTestirovanie.Data;
using UchebnayPracticaTestirovanie.Models;

namespace UchebnayPracticaTestirovanie.Views;

public partial class ZayavkaWindow : Window
{
    public ZayavkaWindow()
    {
        InitializeComponent();

        TypeBox.Items.Add("Компьютер");
        TypeBox.Items.Add("Ноутбук");
        TypeBox.Items.Add("Мышка");
        TypeBox.Items.Add("Клавиатура");
        StatusBox.Items.Add("Новая заявка");
        StatusBox.Items.Add("В процессе ремонта");
        StatusBox.Items.Add("Готова к выдаче");

        using var db = new AppDbContext();
        var clients = db.Users.Include(x => x.Role).Where(x => x.Role.Name == "Заказчик").ToList();
        var masters = db.Users.Include(x => x.Role).Where(x => x.Role.Name == "Техник").ToList();
        ClientBox.ItemsSource = clients;
        MasterBox.ItemsSource = masters;
        ClientBox.DisplayMemberBinding = new Avalonia.Data.Binding("FullName");
        MasterBox.DisplayMemberBinding = new Avalonia.Data.Binding("FullName");

        if (TempVarible.selectedRequest != null)
        {
            var r = db.Requests.First(x => x.Id == TempVarible.selectedRequest.Id);
            ModelBox.Text = r.TechModel;
            ProblemBox.Text = r.Problem;
            StartBox.Text = r.StartDate.ToString("dd.MM.yyyy");
            EndBox.Text = r.CompletionDate == null ? "" : r.CompletionDate.Value.ToString("dd.MM.yyyy");
            PartsBox.Text = r.RepairParts;
            TypeBox.SelectedItem = r.TechType;
            StatusBox.SelectedItem = r.Status;
            ClientBox.SelectedItem = clients.FirstOrDefault(x => x.Id == r.ClientId);
            MasterBox.SelectedItem = masters.FirstOrDefault(x => x.Id == r.MasterId);
        }
        else
        {
            TypeBox.SelectedIndex = 0;
            StatusBox.SelectedIndex = 0;
            StartBox.Text = DateTime.Today.ToString("dd.MM.yyyy");
        }
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        if (ClientBox.SelectedItem is not User client)
        {
            Err.Text = "Выберите клиента";
            return;
        }
        if (string.IsNullOrWhiteSpace(ModelBox.Text) || string.IsNullOrWhiteSpace(ProblemBox.Text))
        {
            Err.Text = "Заполните модель и проблему";
            return;
        }
        if (!DateTime.TryParseExact(StartBox.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var start))
        {
            Err.Text = "Дата начала в формате дд.мм.гггг";
            return;
        }

        DateOnly? end = null;
        if (!string.IsNullOrWhiteSpace(EndBox.Text))
        {
            if (!DateTime.TryParseExact(EndBox.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endDt))
            {
                Err.Text = "Дата окончания в формате дд.мм.гггг";
                return;
            }
            end = DateOnly.FromDateTime(endDt);
        }

        var status = StatusBox.SelectedItem?.ToString() ?? "Новая заявка";
        if (status == "Готова к выдаче" && end == null)
        {
            Err.Text = "Для готовой заявки нужна дата окончания";
            return;
        }

        using var db = new AppDbContext();
        Request req;
        if (TempVarible.selectedRequest == null)
            req = new Request();
        else
            req = db.Requests.First(x => x.Id == TempVarible.selectedRequest.Id);

        req.ClientId = client.Id;
        req.TechType = TypeBox.SelectedItem?.ToString() ?? "";
        req.TechModel = ModelBox.Text.Trim();
        req.Problem = ProblemBox.Text.Trim();
        req.StartDate = DateOnly.FromDateTime(start);
        req.Status = status;
        req.CompletionDate = end;
        req.RepairParts = string.IsNullOrWhiteSpace(PartsBox.Text) ? null : PartsBox.Text.Trim();
        if (MasterBox.SelectedItem is User master)
            req.MasterId = master.Id;
        else
            req.MasterId = null;

        if (TempVarible.selectedRequest == null)
            db.Requests.Add(req);

        db.SaveChanges();
        Close();
    }
}
