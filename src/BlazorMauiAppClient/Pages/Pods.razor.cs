using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using AppCore.Services.K8s;
using BlazorMauiAppClient.YmlEditor;
using k8s.Models;
using k8s;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;

namespace BlazorMauiAppClient.Pages;
public partial class Pods
{
    List<KeyValuePair<string, string>> DetailsData = new();

    List<string> _podLogs = new();
    Stream _podLogsStream;

    [Inject]
    public CurrentK8SContext CurrentK8SContextClient { get; set; }

    [Inject]
    public IJSRuntime JS { get; set; }

    [Inject]
    public SharedState SharedState { get; set; }

    [Inject]
    public K8sService K8sService { get; set; }

    IQueryable<V1Pod>? items = Enumerable.Empty<V1Pod>().AsQueryable();
    PaginationState pagination = new PaginationState { ItemsPerPage = 15 };
    private StreamReader _logReader;

    IQueryable<V1Pod>? FilteredItems => items?.Where(x => x.Metadata.Namespace().Contains(CurrentK8SContextClient.NamespaceFilter, StringComparison.CurrentCultureIgnoreCase));

    private V1Pod _selectedPod;

    [Inject]
    internal PodService PodService { get; set; }

    private YmlEditorPopper _ymlEditorPopper;
    private PodLogPopper _podLogPopper;
    
    protected override async Task OnInitializedAsync()
    {
        _ymlEditorPopper = new YmlEditorPopper("Pod", CurrentK8SContextClient, JS, PodService,true);
        _podLogPopper = new PodLogPopper(CurrentK8SContextClient, JS, PodService);
        _ = InvokeAsync(async () =>
        {
            await Setup();
            StateHasChanged();
        });
        //_ = InvokeAsync(async () =>
        //{
        //    var periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        //    while (await periodicTimer.WaitForNextTickAsync())
        //    {
        //        _ = InvokeAsync(async () =>
        //        {
        //            await Setup();
        //            StateHasChanged();
        //        });
        //    }
        //});


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
        SharedState.CurrentPage = "Pods";
        if (CurrentK8SContextClient.ActiveNamespaceList.IsNullOrEmpty())
        {
            var podlist = await Task.Run(async()=>await CurrentK8SContextClient.Client.Client.CoreV1.ListNamespacedPodAsync(""));
            items = podlist.Items.AsQueryable();
        }
        else
        {
            var listTasks = new List<Task<V1PodList>>();
            foreach (var k8sNamespace in CurrentK8SContextClient.ActiveNamespaceList)
            {
                var podlist = CurrentK8SContextClient.Client.Client.CoreV1.ListNamespacedPodAsync(k8sNamespace.Name());
                listTasks.Add(podlist);
            }

            await Task.WhenAll(listTasks);

            List<V1Pod> podList = new List<V1Pod>();
            foreach (var getPodListTask in listTasks)
            {
                var nsPodList = await getPodListTask;
                podList.AddRange(nsPodList.Items);
            }

            items = podList.AsQueryable();
        }
    }

    public void ShowDetail(V1Pod pod)
    {
        _selectedPod = pod;
    }

    //public async Task ShowPodLogsAsync(V1Pod pod)
    //{
    //    _podLogs = new();
    //    var response = await CurrentK8SContextClient.Client.Client.CoreV1.ReadNamespacedPodLogWithHttpMessagesAsync(
    //            pod.Metadata.Name,
    //            pod.Metadata.NamespaceProperty, container: pod.Spec.Containers[0].Name, follow: true, sinceSeconds: 50_000, timestamps: true).ConfigureAwait(false);
    //    _logReader = new StreamReader(response.Body);
    //    var line = string.Empty;
    //    _ = InvokeAsync(async () =>
    //    {
            
    //        while ((line = await _logReader.ReadLineAsync()) != null)
    //        {
    //            _podLogs.Add(line);
    //            // Update the UI
    //            StateHasChanged();
    //        }
    //    });
    //}

    public async Task DeletePodAsync(V1Pod pod)
    {
        var da = await CurrentK8SContextClient.Client.Client.DeleteNamespacedPodAsync(pod.Name(), pod.Namespace());
        _ = InvokeAsync(async () =>
        {
            await Setup();
            // await GetPodStatus(pod.Name(), pod.Namespace());
            StateHasChanged();
        });
    }

    public async Task GetPodStatus(string podName, string podNamespace)
    {
        var da = await CurrentK8SContextClient.Client.Client.ReadNamespacedPodStatusAsync(podName, podNamespace);
    }
}