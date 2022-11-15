using AppCore.Services.K8s;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using System.Reflection;

namespace BlazorMauiAppClient.Pages;
public partial class Deployments
{
    [Inject]
    public CurrentK8SContext CurrentK8SContextClient { get; set; }

    [Inject]
    public K8sContextService K8SContextService { get; set; }

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
        var sss = await CurrentK8SContextClient.Client.Client.CoreV1.ListNamespacedPodAsync("sitecore-backbone-agent-configuration-api-dev");
        if (sss != null)
        {
            _podlist.AddRange(sss.Items);
            StateHasChanged();
        }

        //_podProvider = async req =>
        //{
        //    var sss = await CurrentK8SContextClient.Client.Client.CoreV1.ListNamespacedPodAsync("default");
        //    return GridItemsProviderResult.From(
        //        items: sss!.Items,
        //        totalItemCount: sss!.Items.Count);
        //};


    }
}

public record DeploymentsPageVm(string Name, string Namespace, int Containers, int Restarts,
    string ControlledBy, string Node, string QoS, string Age, string Status);