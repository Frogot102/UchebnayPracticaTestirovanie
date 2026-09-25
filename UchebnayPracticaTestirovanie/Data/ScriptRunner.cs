using Npgsql;

namespace UchebnayPracticaTestirovanie.Data;

public static class ScriptRunner
{
    public static void Run()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "bd.txt");
        var text = File.ReadAllText(path);
        var parts = text.Split(';');

        var postgres = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=frogot1;";
        var infotech = "Host=localhost;Port=5432;Database=Infotech;Username=postgres;Password=frogot1;";

        foreach (var raw in parts)
        {
            var cmd = raw.Trim();
            if (cmd.Length == 0)
                continue;
            if (!cmd.StartsWith("CREATE DATABASE", StringComparison.OrdinalIgnoreCase))
                continue;

            try
            {
                using var conn = new NpgsqlConnection(postgres);
                conn.Open();
                using var q = new NpgsqlCommand(cmd, conn);
                q.ExecuteNonQuery();
            }
            catch
            {
            }
        }

        using (var check = new NpgsqlConnection(infotech))
        {
            check.Open();
            using var q = new NpgsqlCommand("SELECT to_regclass('public.users')::text", check);
            var exists = q.ExecuteScalar() as string;
            if (!string.IsNullOrEmpty(exists))
                return;
        }

        using var db = new NpgsqlConnection(infotech);
        db.Open();
        foreach (var raw in parts)
        {
            var cmd = raw.Trim();
            if (cmd.Length == 0)
                continue;
            if (cmd.StartsWith("CREATE DATABASE", StringComparison.OrdinalIgnoreCase))
                continue;

            using var q = new NpgsqlCommand(cmd, db);
            q.ExecuteScalar();
        }
    }
}
