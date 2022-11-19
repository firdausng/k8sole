namespace AppCore.Services.K8s.Interfaces;

public interface IYmlCrud
{
    public Task<string> GetYmlAsync(string name, string nameSpace);
    public Task<string> UpdateYmlAsync(string name, string nameSpace, string updatedYml);
}