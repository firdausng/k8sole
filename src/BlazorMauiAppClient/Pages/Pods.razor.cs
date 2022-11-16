using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using AppCore.Services.K8s;
using k8s.Models;
using k8s;
using Microsoft.IdentityModel.Tokens;

namespace BlazorMauiAppClient.Pages;
public partial class Pods
{
    [Inject]
    public CurrentK8SContext CurrentK8SContextClient { get; set; }

    IQueryable<V1Pod>? items = Enumerable.Empty<V1Pod>().AsQueryable();
    PaginationState pagination = new PaginationState { ItemsPerPage = 20 };

    IQueryable<V1Pod>? FilteredItems => items?.Where(x => x.Metadata.Namespace().Contains(CurrentK8SContextClient.NamespaceFilter, StringComparison.CurrentCultureIgnoreCase));

    protected override async Task OnInitializedAsync()
    {
        await Setup();

        CurrentK8SContextClient.ActiveNamespaceChanged += async (s, e) =>
        {
            await Setup();
            _ = InvokeAsync(() => StateHasChanged());
        };

    }

    public async Task Setup()
    {
        if (CurrentK8SContextClient.ActiveNamespaceList.IsNullOrEmpty())
        {
            var podlist = await CurrentK8SContextClient.Client.Client.CoreV1.ListNamespacedPodAsync("");
            items = podlist.Items.AsQueryable();
        }
        else
        {
            var list = new List<V1Pod>();
            foreach (var k8sNamespace in CurrentK8SContextClient.ActiveNamespaceList)
            {
                var podlist = await CurrentK8SContextClient.Client.Client.CoreV1.ListNamespacedPodAsync(k8sNamespace.Name());
                list.AddRange(podlist.Items);
            }
            items = list.AsQueryable();
        }
    }
}