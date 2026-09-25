namespace UchebnayPracticaTestirovanie.Models;

public static class Session
{
    public static int UserId { get; set; }
    public static string FullName { get; set; } = "";
    public static string Role { get; set; } = "";

    public static bool IsManager => Role == "Менеджер";
    public static bool IsTechnician => Role == "Техник";
    public static bool IsOperator => Role == "Оператор";
    public static bool IsClient => Role == "Заказчик";

    public static void Clear()
    {
        UserId = 0;
        FullName = "";
        Role = "";
    }
}
