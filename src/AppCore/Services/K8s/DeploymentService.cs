using AppCore.Services.K8s.Interfaces;
using k8s.KubeConfigModels;

namespace AppCore.Services.K8s;

public class DeploymentService : IYmlCrud
{
    private readonly CurrentK8SContext _context;

    public DeploymentService(CurrentK8SContext context)
    {
        _context = context;
    }
    public async Task<string> GetYmlAsync(string name, string nameSpace)
    {
        var s = await _context.Client.Client!.ReadNamespacedDeploymentAsync(name, nameSpace, true);
        return KubernetesYaml.Serialize(s);
    }

    public Task<string> UpdateYmlAsync(string name, string nameSpace, string updatedYml)
    {
        throw new NotImplementedException();
    }
}