using AppCore.Services.K8s;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace BlazorMauiAppClient.Pages;
public partial class Deployments
{
    [Inject]
    public CurrentK8SContext CurrentK8SContextClient { get; set; }

    IQueryable<V1Deployment>? items = Enumerable.Empty<V1Deployment>().AsQueryable();
    PaginationState pagination = new PaginationState { ItemsPerPage = 20 };
    string namespaceFilter 
    {
        get
        {
            return CurrentK8SContextClient.NamespaceFilter;
        }
        set
        {
            CurrentK8SContextClient.NamespaceFilter = value;
        } 
    
    }

    IQueryable<V1Deployment>? FilteredItems => items?.Where(x => x.Metadata.Namespace().Contains(namespaceFilter, StringComparison.CurrentCultureIgnoreCase));

    protected override async Task OnInitializedAsync()
    {
        var podlist = await CurrentK8SContextClient.Client.Client.ListNamespacedDeploymentAsync("");
        items = podlist.Items.AsQueryable();
    }
}

public record DeploymentsPageVm(string Name, string Namespace, int Containers, int Restarts,
    string ControlledBy, string Node, string QoS, string Age, string Status);