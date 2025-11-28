using Microsoft.Extensions.DependencyInjection;

namespace CAFConsole.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTodoData(
        this IServiceCollection services,
        string connectionString
    )
    {
        services.AddSingleton<ITaskRepository>(_ => new TaskRepository(connectionString));
        return services;
    }
}
