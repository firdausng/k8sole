using AppCore.Services.K8s;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.IdentityModel.Tokens;

namespace BlazorMauiAppClient.Pages;
public partial class Deployments
{
    List<KeyValuePair<string, string>> DetailsData = new();

    [Inject]
    public CurrentK8SContext CurrentK8SContextClient { get; set; }

    IQueryable<V1Deployment>? items = Enumerable.Empty<V1Deployment>().AsQueryable();
    PaginationState pagination = new PaginationState { ItemsPerPage = 20 };

    IQueryable<V1Deployment>? FilteredItems => items?.Where(x => x.Metadata.Namespace().Contains(CurrentK8SContextClient.NamespaceFilter, StringComparison.CurrentCultureIgnoreCase));

    protected override async Task OnInitializedAsync()
    {
        await Setup();

        CurrentK8SContextClient.ActiveNamespaceChanged += async (s, e) =>
        {
            await Setup();
            _ = InvokeAsync(()=> StateHasChanged());
        };
    }

    public async Task Setup()
    {
        if (CurrentK8SContextClient.ActiveNamespaceList.IsNullOrEmpty())
        {
            var podlist = await CurrentK8SContextClient.Client.Client.ListDeploymentForAllNamespacesAsync();
            items = podlist.Items.AsQueryable();
        }
        else
        {
            var list = new List<V1Deployment>();
            foreach (var k8sNamespace in CurrentK8SContextClient.ActiveNamespaceList)
            {
                var podlist = await CurrentK8SContextClient.Client.Client.ListNamespacedDeploymentAsync(k8sNamespace.Name());
                list.AddRange(podlist.Items);
            }
            items = list.AsQueryable();
        }
    }

    public void ShowDetail(V1Deployment deployment)
    {
        DetailsData.Add(new KeyValuePair<string, string>("Name", deployment.Name()));
        DetailsData.Add(new KeyValuePair<string, string>("Namespace", deployment.Namespace()));
        DetailsData.Add(new KeyValuePair<string, string>("Kind", deployment.Kind));
        DetailsData.Add(new KeyValuePair<string, string>("ApiGroup", deployment.ApiGroup()));
        DetailsData.Add(new KeyValuePair<string, string>("ApiVersion", deployment.ApiVersion));
        DetailsData.Add(new KeyValuePair<string, string>("CreationTimestamp", deployment.CreationTimestamp().ToString()));
        DetailsData.Add(new KeyValuePair<string, string>("DeletionTimestamp", deployment.DeletionTimestamp().ToString()));
    }
}

public record DeploymentsPageVm(string Name, string Namespace, int Containers, int Restarts,
    string ControlledBy, string Node, string QoS, string Age, string Status);