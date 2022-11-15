using System.Net;

namespace AppCore.Services.K8s;

public class NamespaceService
{
    private readonly CurrentK8SContext _currentK8SContext;

    public NamespaceService(CurrentK8SContext currentK8SContext)
    {
        _currentK8SContext = currentK8SContext;
    }
    public async Task<IList<V1Namespace>> GetAllAsync()
    {
        var res = await _currentK8SContext.Client!.Client!.CoreV1.ListNamespaceAsync();
        return res.Items;
    }
    public void DeleteAsync(string namespaceName, V1DeleteOptions? deleteOption = null)
    {
        _currentK8SContext.Client!.Client!.CoreV1.DeleteNamespaceAsync(namespaceName, deleteOption ?? new V1DeleteOptions()).ConfigureAwait(false);
    }

    public async Task SetCurrentNamespaceAsync(string name)
    {
        _currentK8SContext.ActiveNamespace = await _currentK8SContext.Client!.Client!.CoreV1.ReadNamespaceAsync(name).ConfigureAwait(false);
    }
    public Task<V1Namespace> GetCurrentNamespaceAsync()
    {
        return Task.FromResult(_currentK8SContext.ActiveNamespace);
    }
}