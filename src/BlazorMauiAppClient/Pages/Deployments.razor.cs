using AppCore.Services.K8s;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.IdentityModel.Tokens;

namespace BlazorMauiAppClient.Pages;
public partial class Deployments
{
    [Inject]
    public CurrentK8SContext CurrentK8SContextClient { get; set; }

    IQueryable<V1Deployment>? items = Enumerable.Empty<V1Deployment>().AsQueryable();
    PaginationState pagination = new PaginationState { ItemsPerPage = 20 };

    IQueryable<V1Deployment>? FilteredItems => items?.Where(x => x.Metadata.Namespace().Contains(CurrentK8SContextClient.NamespaceFilter, StringComparison.CurrentCultureIgnoreCase));

    protected override async Task OnInitializedAsync()
    {
        //var podlist = await CurrentK8SContextClient.Client.Client.ListDeploymentForAllNamespacesAsync();
        //items = podlist.Items.AsQueryable();

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
}

public record DeploymentsPageVm(string Name, string Namespace, int Containers, int Restarts,
    string ControlledBy, string Node, string QoS, string Age, string Status);