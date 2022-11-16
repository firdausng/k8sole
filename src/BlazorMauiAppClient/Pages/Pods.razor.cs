using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using AppCore.Services.K8s;
using k8s.Models;
using k8s;

namespace BlazorMauiAppClient.Pages;
public partial class Pods
{
    [Inject]
    public CurrentK8SContext CurrentK8SContextClient { get; set; }

    IQueryable<V1Pod>? items = Enumerable.Empty<V1Pod>().AsQueryable();
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

    IQueryable<V1Pod>? FilteredItems => items?.Where(x => x.Metadata.Namespace().Contains(namespaceFilter, StringComparison.CurrentCultureIgnoreCase));

    protected override async Task OnInitializedAsync()
    {
        var podlist = await CurrentK8SContextClient.Client.Client.CoreV1.ListNamespacedPodAsync("");
        items = podlist.Items.AsQueryable();
    }
}