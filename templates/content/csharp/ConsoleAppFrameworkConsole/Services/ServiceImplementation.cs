using Microsoft.Extensions.Logging;

public interface IService
{
    void DoSomething();
}
public class ServiceImplementation(ILogger<ServiceImplementation> logger) : IService
{
    public void DoSomething()
    {
        logger.LogCritical("I am doing something");
    }
}