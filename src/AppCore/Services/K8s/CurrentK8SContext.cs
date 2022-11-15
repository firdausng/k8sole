using AppCore.Services.K8s.Models;

namespace AppCore.Services.K8s;

public class CurrentK8SContext
{
    public K8SContextClient Client { get; set; }

    public V1Namespace ActiveNamespace { get; set; }
}