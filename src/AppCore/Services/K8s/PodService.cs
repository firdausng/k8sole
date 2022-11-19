using AppCore.Services.K8s.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace AppCore.Services.K8s;

public class PodService : IYmlCrud, IPodLogRetriever
{
    private readonly CurrentK8SContext _context;

    public PodService(CurrentK8SContext context)
    {
        _context = context;
    }
    public async Task<IList<V1Pod>> GetAllAsync()
    {
        return (await _context.Client.Client!.CoreV1.ListNamespacedPodAsync("")).Items;
    }
    public async Task<IList<V1Pod>> GetNamespaced()
    {
        if (!_context.ActiveNamespaceList.IsNullOrEmpty())
        {
            var services = new List<V1Pod>();
            foreach (var v1Namespace in _context.ActiveNamespaceList)
            {
                var nServices = (await _context.Client.Client!.CoreV1.ListNamespacedPodAsync(v1Namespace.Name())).Items;
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
        var s = await _context.Client.Client!.CoreV1.ReadNamespacedPodAsync(name, nameSpace, true);
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

    public async Task<StreamReader> GetPodLogStreamAsync(V1Pod pod,int containerNo=0)
    {
        var response = await _context.Client.Client.CoreV1.ReadNamespacedPodLogWithHttpMessagesAsync(
            pod.Metadata.Name,
            pod.Metadata.NamespaceProperty, container: pod.Spec.Containers[containerNo].Name, follow: true, timestamps: true,tailLines:500).ConfigureAwait(false);
        return new StreamReader(response.Body);
    }
}