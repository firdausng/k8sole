using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.IdentityModel.Tokens;

namespace BlazorMauiAppClient.Pages;
public partial class Deployments
{
    List<KeyValuePair<string, string>> DetailsData = new();

    [Inject]
    public CurrentK8SContext CurrentK8SContextClient { get; set; }
    [Inject]
    public K8sService K8sService { get; set; }

    [Inject]
    public SharedState SharedState { get; set; }

    IQueryable<V1Deployment>? items = Enumerable.Empty<V1Deployment>().AsQueryable();
    PaginationState pagination = new PaginationState { ItemsPerPage = 20 };

    IQueryable<V1Deployment>? FilteredItems => items?.Where(x => x.Metadata.Namespace().Contains(CurrentK8SContextClient.NamespaceFilter, StringComparison.CurrentCultureIgnoreCase));

    private int? _currentDeploymentReplicaCount;
    private V1Deployment? _currentDeployment;

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
        SharedState.CurrentPage = "Deployment";
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

    private async Task SetActiveDeployment(V1Deployment deployment)
    {
        _currentDeploymentReplicaCount = deployment?.Spec.Replicas.Value;
        _currentDeployment = deployment;
        _ = InvokeAsync(async () =>
        {
            StateHasChanged();
        });
    }   

    // private Task UpdateActiveDeployment(V1Deployment deployment)
    // {
    //     var jsonPatch = new JsonPatchDocument<V1Scale>();
    //     jsonPatch.ContractResolver = new DefaultContractResolver
    //     {
    //         NamingStrategy = new CamelCaseNamingStrategy()
    //     };
    //     jsonPatch.Replace(e => e.Spec.Replicas, request.Replicas);
    //     var jsonPatchString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonPatch);
    //     var patch = new V1Patch(jsonPatchString, V1Patch.PatchType.JsonPatch);
    //     var podlist = await CurrentK8SContextClient.Client.Client.PatchNamespacedDeploymentScaleAsync();
    // }
}

public record DeploymentsPageVm(string Name, string Namespace, int Containers, int Restarts,
    string ControlledBy, string Node, string QoS, string Age, string Status);