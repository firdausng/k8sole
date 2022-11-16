using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using AppCore.Services.K8s;
using k8s.Models;
using k8s;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Microsoft.Maui.Controls.Shapes;

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

    IQueryable<V1Pod>? items = Enumerable.Empty<V1Pod>().AsQueryable();
    PaginationState pagination = new PaginationState { ItemsPerPage = 20 };
    private StreamReader _logReader;

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

    public void ShowDetail(V1Pod pod)
    {
        DetailsData.Add(new KeyValuePair<string, string>("Name", pod.Name()));
        DetailsData.Add(new KeyValuePair<string, string>("Namespace", pod.Namespace()));
        DetailsData.Add(new KeyValuePair<string, string>("Kind", pod.Kind));
        DetailsData.Add(new KeyValuePair<string, string>("ApiGroup", pod.ApiGroup()));
        DetailsData.Add(new KeyValuePair<string, string>("ApiVersion", pod.ApiVersion));
        DetailsData.Add(new KeyValuePair<string, string>("CreationTimestamp", pod.CreationTimestamp().ToString()));
        DetailsData.Add(new KeyValuePair<string, string>("DeletionTimestamp", pod.DeletionTimestamp().ToString()));
    }

    public async Task ShowPodLogsAsync(V1Pod pod)
    {
        var response = await CurrentK8SContextClient.Client.Client.CoreV1.ReadNamespacedPodLogWithHttpMessagesAsync(
                pod.Metadata.Name,
                pod.Metadata.NamespaceProperty, container: pod.Spec.Containers[0].Name, follow: false, sinceSeconds: 100_000).ConfigureAwait(false);
        var stream = response.Body;
        //_podLogsStream = stream;
        // stream.CopyTo(Console.OpenStandardOutput());

        //using var streamRef = new DotNetStreamReference(stream: stream);

        //await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        _logReader = new StreamReader(stream);
        //string text = reader.ReadLine();
        //_podLogs = text;


        //while ((_podLogs = reader.ReadLine()) != null)
        //{
        //}
    }
}