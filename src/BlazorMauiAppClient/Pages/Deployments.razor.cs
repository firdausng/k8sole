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

    [Inject]
    public K8sContextService K8SContextService { get; set; }

    [Inject]
    private NamespaceService _namespaceService { get; set; }

    GridSort<DeploymentsPageVm> sortByName = GridSort<DeploymentsPageVm>
        .ByAscending(p => p.Name)
        .ThenAscending(p => p.Namespace);

    IQueryable<DeploymentsPageVm> people = new[]
    {
        new DeploymentsPageVm("Name1", "Namespace1", 1, 2, "ControlledBy", "Node", "QoS", "Age", "Status"),
    }.AsQueryable();
    private List<V1Pod> _podlist = new();

    GridItemsProvider<V1Pod>? _podProvider;

    protected override async Task OnInitializedAsync()
    {
        var sss = await CurrentK8SContextClient.Client.Client.CoreV1.ListNamespacedPodAsync(CurrentK8SContextClient.ActiveNamespace.Name());

        if (sss != null)
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