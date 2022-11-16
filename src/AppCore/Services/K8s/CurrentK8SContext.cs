using AppCore.Services.K8s.Models;

namespace AppCore.Services.K8s;

public class CurrentK8SContext
{
    public K8SContextClient Client { get; set; }

    public List<V1Namespace> ActiveNamespaceList { get; set; } = new List<V1Namespace>();
    public string NamespaceFilter { get; set; } = string.Empty;
}