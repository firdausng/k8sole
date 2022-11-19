namespace AppCore.Services.K8s.Interfaces;

public interface IPodLogRetriever
{
    public Task<StreamReader> GetPodLogStreamAsync(V1Pod pod,int containerNo=0);
}