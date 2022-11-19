using BlazorMauiAppClient.YmlEditor;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using k8s;

namespace BlazorMauiAppClient.Pages;
public partial class SecretsPage
{
    List<KeyValuePair<string, string>> DetailsData = new();

    [Inject]
    public CurrentK8SContext CurrentK8SContextClient { get; set; }
    [Inject]
    public K8sService K8sService { get; set; }

    [Inject]
    public SharedState SharedState { get; set; }

    [Inject] private DeploymentService _deploymentService { get; set; }
    [Inject] private IJSRuntime _jsRuntime { get; set; }

    IQueryable<V1Secret>? items = Enumerable.Empty<V1Secret>().AsQueryable();
    PaginationState pagination = new PaginationState { ItemsPerPage = 15 };

    IQueryable<V1Secret>? FilteredItems => items?.Where(x => x.Metadata.Namespace().Contains(CurrentK8SContextClient.NamespaceFilter, StringComparison.CurrentCultureIgnoreCase));

    private int? _currentDeploymentReplicaCount;
    private V1Secret? _currentDeployment;

    private YmlEditorPopper _ymlEditorPopper;
    private V1Secret _selectedSecret;

    protected override async Task OnInitializedAsync()
    {
        _ymlEditorPopper = new YmlEditorPopper("Deployment", CurrentK8SContextClient, _jsRuntime, _deploymentService);
        _ = InvokeAsync(async () =>
        {
            await Setup();
            StateHasChanged();
        });

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
        SharedState.CurrentPage = "Secrets";
        if (CurrentK8SContextClient.ActiveNamespaceList.IsNullOrEmpty())
        {
            var secretlist = await CurrentK8SContextClient.Client.Client.ListSecretForAllNamespacesAsync();
            items = secretlist.Items.AsQueryable();
        }
        else
        {
            var list = new List<V1Secret>();
            foreach (var k8sNamespace in CurrentK8SContextClient.ActiveNamespaceList)
            {
                var secretlist = await CurrentK8SContextClient.Client.Client.ListNamespacedSecretAsync(k8sNamespace.Name());
                list.AddRange(secretlist.Items);
            }
            items = list.AsQueryable();
        }
    }
}