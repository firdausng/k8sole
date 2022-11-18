using AppCore.Services.K8s;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace BlazorMauiAppClient.Shared;
public partial class SecretData
{
    [Inject]
    public CurrentK8SContext CurrentK8SContextClient { get; set; }

    public string SecretValue { get; set; } = "secret";

    [Parameter]
    public string SecretName { get; set; }

    [Parameter]
    public string Namespace { get; set; }

    [Parameter]
    public string SecretKey { get; set; }


    public async Task GetSecretAsync()
    {
        //cosmosdb
        var ss = await CurrentK8SContextClient.Client.Client.ReadNamespacedSecretAsync(SecretName, Namespace);
        var decodedString = string.Empty;
        foreach (var item in ss.Data)
        {
            if (item.Key.Equals(SecretKey, StringComparison.OrdinalIgnoreCase))
            {
                SecretValue = Encoding.UTF8.GetString(item.Value);
            }

        }
    }
}