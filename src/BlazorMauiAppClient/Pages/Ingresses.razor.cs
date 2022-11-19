using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;

namespace BlazorMauiAppClient.Pages;
public partial class Ingresses
{
    List<KeyValuePair<string, string>> DetailsData = new();

    [Inject]
    public CurrentK8SContext CurrentK8SContextClient { get; set; }

    [Inject]
    public SharedState SharedState { get; set; }

    [Inject]
    public K8sService K8sService { get; set; }

    IQueryable<V1Ingress>? items = Enumerable.Empty<V1Ingress>().AsQueryable();
    PaginationState pagination = new PaginationState { ItemsPerPage = 10 };
    private StreamReader _logReader;

    IQueryable<V1Ingress>? FilteredItems 
        => items?.Where(x => x.Metadata.Namespace().Contains(CurrentK8SContextClient.NamespaceFilter, StringComparison.CurrentCultureIgnoreCase));

    private V1Ingress _selectedPod;

    protected override async Task OnInitializedAsync()
    {
        await Setup();

        CurrentK8SContextClient.ActiveNamespaceChanged += async (s, e) =>
        {
            await Setup();
            _ = InvokeAsync(async () =>
            {

                StateHasChanged();
            });
        };

        K8sService.ActiveK8sContextChanged += async (s, e) =>
        {

            _ = InvokeAsync(async () =>
            {
                await Setup();
                StateHasChanged();
            });
        };
    }

    public async Task Setup()
    {
        SharedState.CurrentPage = "Ingresses";
        if (CurrentK8SContextClient.ActiveNamespaceList.IsNullOrEmpty())
        {
            var podlist = await CurrentK8SContextClient.Client.Client.NetworkingV1.ListNamespacedIngressAsync("");
            items = podlist.Items.AsQueryable();
        }
        else
        {
            var list = new List<V1Ingress>();
            foreach (var k8sNamespace in CurrentK8SContextClient.ActiveNamespaceList)
            {
                var podlist = await CurrentK8SContextClient.Client.Client.NetworkingV1.ListNamespacedIngressAsync(k8sNamespace.Name());
                list.AddRange(podlist.Items);
            }
            items = list.AsQueryable();
        }
    }

    public void ShowDetail(V1Ingress ingress)
    {
        DetailsData.Add(new KeyValuePair<string, string>("Name", ingress.Name()));
        DetailsData.Add(new KeyValuePair<string, string>("Namespace", ingress.Namespace()));
        if(ingress.GetController() is not null)
        {
            DetailsData.Add(new KeyValuePair<string, string>("Controller Name", ingress.GetController().Name));
            DetailsData.Add(new KeyValuePair<string, string>("Controller ApiVersion", ingress.GetController().ApiVersion));
            DetailsData.Add(new KeyValuePair<string, string>("Controller Kind", ingress.GetController().Kind));
        }
        
        DetailsData.Add(new KeyValuePair<string, string>("Kind", ingress.Kind));
        DetailsData.Add(new KeyValuePair<string, string>("ApiGroup", ingress.ApiGroup()));
        DetailsData.Add(new KeyValuePair<string, string>("ApiVersion", ingress.ApiVersion));
        DetailsData.Add(new KeyValuePair<string, string>("CreationTimestamp", ingress.CreationTimestamp().ToString()));
        DetailsData.Add(new KeyValuePair<string, string>("DeletionTimestamp", ingress.DeletionTimestamp().ToString()));
    }
    
    public async Task DeleteIngressAsync(V1Ingress ingress)
    {
        var da = await CurrentK8SContextClient.Client.Client.NetworkingV1.DeleteNamespacedIngressAsync(ingress.Name(), ingress.Namespace());
        _ = InvokeAsync(async () =>
        {
            await Setup();
            // await GetPodStatus(pod.Name(), pod.Namespace());
            StateHasChanged();
        });
    }

    public async Task GetPodStatus(string podName, string podNamespace)
    {
        var da = await CurrentK8SContextClient.Client.Client.NetworkingV1.ReadNamespacedIngressAsync(podName, podNamespace);
    }
}