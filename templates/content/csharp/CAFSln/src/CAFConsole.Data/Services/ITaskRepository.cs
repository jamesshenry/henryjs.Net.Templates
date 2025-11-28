using CAFConsole.Shared;

namespace CAFConsole.Data;

public interface ITaskRepository
{
    void Add(string title);
    List<TodoItem> GetAll();
    TodoItem GetOne(int id);
    void Complete(int id);
}
