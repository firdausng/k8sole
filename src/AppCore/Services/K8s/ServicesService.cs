using Microsoft.IdentityModel.Tokens;

namespace AppCore.Services.K8s;

public class ServicesService
{
    private readonly CurrentK8SContext _context;

    public ServicesService(CurrentK8SContext context)
    {
        _context = context;
    }

    public async Task<IList<V1Service>> GetAllAsync()
    {
        return (await _context.Client.Client!.CoreV1.ListNamespacedServiceAsync("")).Items;
    }
    public async Task<IList<V1Service>> GetNamespaced()
    {
        if (!_context.ActiveNamespaceList.IsNullOrEmpty())
        {
            var services = new List<V1Service>();
            foreach (var v1Namespace in _context.ActiveNamespaceList)
            {
                var nServices = (await _context.Client.Client!.CoreV1.ListNamespacedServiceAsync(v1Namespace.Name())).Items;
                services.AddRange(nServices);
            }
            return services;

        }
        else
        {
            return await GetAllAsync();
        }

    }

}