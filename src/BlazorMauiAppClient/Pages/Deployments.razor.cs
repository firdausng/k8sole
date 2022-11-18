using k8s;
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
    PaginationState pagination = new PaginationState { ItemsPerPage = 15 };

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
        if(deployment.GetController() is not null)
        {
            DetailsData.Add(new KeyValuePair<string, string>("Controller Name", deployment.GetController().Name));
            DetailsData.Add(new KeyValuePair<string, string>("Controller ApiVersion", deployment.GetController().ApiVersion));
            DetailsData.Add(new KeyValuePair<string, string>("Controller Kind", deployment.GetController().Kind));
        }
        
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

    private async Task ScaleDeployment(V1Deployment deployment)
    {
        var currentDeploymentScale = await CurrentK8SContextClient.Client.Client.ReadNamespacedDeploymentScaleAsync(deployment.Name(), deployment.Namespace());
        currentDeploymentScale.Spec.Replicas = _currentDeploymentReplicaCount;
        var newScale = await CurrentK8SContextClient.Client.Client.ReplaceNamespacedDeploymentScaleAsync(currentDeploymentScale, deployment.Name(), deployment.Namespace());
    }
}

public record DeploymentsPageVm(string Name, string Namespace, int Containers, int Restarts,
    string ControlledBy, string Node, string QoS, string Age, string Status);