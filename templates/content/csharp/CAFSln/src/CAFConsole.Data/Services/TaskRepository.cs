using CAFConsole.Shared;
using Dapper;
using Microsoft.Data.Sqlite;

[module: DapperAot]

namespace CAFConsole.Data;

public class TaskRepository(string connectionString) : ITaskRepository
{
    private SqliteConnection GetConnection() => new(connectionString);

    public void Add(string title)
    {
        using var conn = GetConnection();
        conn.Execute("INSERT INTO Tasks (Title) VALUES (@Title)", new { Title = title });
    }

    public List<TodoItem> GetAll()
    {
        using var conn = GetConnection();
        return conn.Query<TodoItem>("SELECT * FROM Tasks ORDER BY CreatedAt DESC").AsList();
    }

    public TodoItem GetOne(int id)
    {
        using var conn = GetConnection();
        return conn.QueryFirst<TodoItem>("SELECT * FROM Tasks WHERE Id=@id", new { id });
    }

    public void Complete(int id)
    {
        using var conn = GetConnection();
        conn.Execute("UPDATE Tasks SET IsComplete = 1 WHERE Id = @id", new { id });
    }
}
