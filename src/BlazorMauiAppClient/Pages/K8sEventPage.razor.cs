using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using k8s;

namespace BlazorMauiAppClient.Pages;
public partial class K8sEventPage
{
    List<KeyValuePair<string, string>> DetailsData = new();

    [Inject]
    public CurrentK8SContext CurrentK8SContextClient { get; set; }
    [Inject]
    public K8sService K8sService { get; set; }

    [Inject]
    public SharedState SharedState { get; set; }

    IQueryable<Corev1Event>? items = Enumerable.Empty<Corev1Event>().AsQueryable();
    PaginationState pagination = new PaginationState { ItemsPerPage = 20 };

    IQueryable<Corev1Event>? FilteredItems => items?.Where(x => x.Metadata.Namespace().Contains(CurrentK8SContextClient.NamespaceFilter, StringComparison.CurrentCultureIgnoreCase));

    private int? _currentDeploymentReplicaCount;
    private Corev1Event? _currentDeployment;

    protected override async Task OnInitializedAsync()
    {
        await Setup();

        CurrentK8SContextClient.ActiveNamespaceChanged += async (s, e) =>
        {
            _ = InvokeAsync(async () =>
            {
                await Setup();
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
        SharedState.CurrentPage = "Events";
        if (CurrentK8SContextClient.ActiveNamespaceList.IsNullOrEmpty())
        {
            var eventList = await CurrentK8SContextClient.Client.Client.CoreV1.ListEventForAllNamespacesAsync();
            items = eventList.Items.AsQueryable();
        }
        else
        {
            var list = new List<Corev1Event>();
            foreach (var k8sNamespace in CurrentK8SContextClient.ActiveNamespaceList)
            {
                var eventlist = await CurrentK8SContextClient.Client.Client.CoreV1.ListNamespacedEventAsync(k8sNamespace.Name());
                list.AddRange(eventlist.Items);
            }
            items = list.AsQueryable();
        }
    }

    public void ShowDetail(Corev1Event k8sEvent)
    {
        DetailsData.Add(new KeyValuePair<string, string>("Name", k8sEvent.Name()));
        DetailsData.Add(new KeyValuePair<string, string>("Namespace", k8sEvent.Namespace()));
        DetailsData.Add(new KeyValuePair<string, string>("Kind", k8sEvent.Kind));
        DetailsData.Add(new KeyValuePair<string, string>("ApiGroup", k8sEvent.ApiGroup()));
        DetailsData.Add(new KeyValuePair<string, string>("ApiVersion", k8sEvent.ApiVersion));
        DetailsData.Add(new KeyValuePair<string, string>("CreationTimestamp", k8sEvent.CreationTimestamp().ToString()));
        DetailsData.Add(new KeyValuePair<string, string>("DeletionTimestamp", k8sEvent.DeletionTimestamp().ToString()));

        DetailsData.Add(new KeyValuePair<string, string>("InvolvedObject ResourceVersion", k8sEvent.InvolvedObject.ResourceVersion));
        DetailsData.Add(new KeyValuePair<string, string>("InvolvedObject FieldPath", k8sEvent.InvolvedObject.FieldPath));
    }
}