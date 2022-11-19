using AppCore.Services.K8s.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace AppCore.Services.K8s;

public class ServicesService : IYmlCrud
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

    public async Task<string> GetYmlAsync(string name, string nameSpace)
    {
        var s = await _context.Client.Client!.CoreV1.ReadNamespacedServiceAsync(name, nameSpace, true);
        return KubernetesYaml.Serialize(s);
    }

    public Task<string> UpdateYmlAsync(string name, string nameSpace, string updatedYml)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetYmlAsync(V1Service service)
    {
        return Task.FromResult(KubernetesYaml.Serialize(service));
    }


}