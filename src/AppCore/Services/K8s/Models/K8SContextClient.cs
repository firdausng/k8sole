namespace AppCore.Services.K8s.Models;

public record K8SContextClient(string ContextName)
{
    public Kubernetes? Client { get; set; }
    public K8SContextStatus? Status { get; set; } = K8SContextStatus.DISCONNECTED;
}

public enum K8SContextStatus
{
    CONNECTED,
    DISCONNECTED
}