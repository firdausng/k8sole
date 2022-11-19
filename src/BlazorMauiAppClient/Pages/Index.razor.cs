using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;

namespace BlazorMauiAppClient.Pages;
public partial class Index
{
    private NodeMetricsList _nodeMetric;
    private PodMetricsList _podeMetric;

    [Inject]
    public CurrentK8SContext CurrentK8SContextClient { get; set; }
    [Inject]
    public K8sService K8sService { get; set; }

    protected override async Task OnInitializedAsync()
    {
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
        try
        {
            //_nodeMetric = await CurrentK8SContextClient.Client.Client.GetKubernetesNodesMetricsAsync();
            _podeMetric = await CurrentK8SContextClient.Client.Client.GetKubernetesPodsMetricsAsync();
        }
        catch (Exception e)
        {

        }
    }
}
