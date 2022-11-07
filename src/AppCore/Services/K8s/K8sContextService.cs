namespace AppCore.Services.K8s;

public class K8sContextService
{
    private readonly ILogger<K8sService> _logger;
    private readonly CurrentK8SContext _currentK8SContext;

    public K8sContextService(ILogger<K8sService> logger, CurrentK8SContext currentK8SContext)
    {
        _logger = logger;
        _currentK8SContext = currentK8SContext;
    }
    public async Task<V1NamespaceList> GetAllNamespace()
    {
        var list = await _currentK8SContext.Client.Client.CoreV1.ListNamespaceAsync();
        return list;
    }
}
