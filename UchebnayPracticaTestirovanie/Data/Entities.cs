namespace UchebnayPracticaTestirovanie.Data;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public ICollection<User> Users { get; set; } = new List<User>();
}

public class User
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
    public Role Role { get; set; } = null!;
}

public class Request
{
    public int Id { get; set; }
    public DateOnly StartDate { get; set; }
    public string TechType { get; set; } = "";
    public string TechModel { get; set; } = "";
    public string Problem { get; set; } = "";
    public string Status { get; set; } = "";
    public DateOnly? CompletionDate { get; set; }
    public string? RepairParts { get; set; }
    public int? MasterId { get; set; }
    public int ClientId { get; set; }
    public User? Master { get; set; }
    public User Client { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

public class Comment
{
    public int Id { get; set; }
    public string Message { get; set; } = "";
    public int MasterId { get; set; }
    public int RequestId { get; set; }
    public User Master { get; set; } = null!;
    public Request Request { get; set; } = null!;
}

public static class Lists
{
    public static readonly string[] TechTypes = ["Компьютер", "Ноутбук", "Мышка", "Клавиатура"];
    public static readonly string[] RequestStatuses = ["Новая заявка", "В процессе ремонта", "Готова к выдаче"];
}
