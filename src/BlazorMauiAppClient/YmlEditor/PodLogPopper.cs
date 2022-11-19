using AppCore.Services.K8s.Interfaces;
using BlazorMauiAppClient.ViewModels;
using Microsoft.JSInterop;

namespace BlazorMauiAppClient.YmlEditor;

public class PodLogPopper
{
    private readonly string _kindName;
    private readonly CurrentK8SContext _currentK8SContextClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly IPodLogRetriever _podLogRetriever;

    public PodLogPopper(CurrentK8SContext currentK8SContextClient, IJSRuntime jsRuntime, IPodLogRetriever podLogRetriever)
    {
        _kindName = "Pod";
        _currentK8SContextClient = currentK8SContextClient;
        _jsRuntime = jsRuntime;
        _podLogRetriever = podLogRetriever;
    }
    public async Task PopPodLogViewer(V1Pod pod, int containerNo = 0)
    {
        StreamReader logReader = await _podLogRetriever.GetPodLogStreamAsync(pod, containerNo);
        string id = GetId(pod);
        string podLogViewerId = id + Constants.PodLogViewerExtension;
        _currentK8SContextClient.AddTabComponents(id, new ComponentMetadata()
        {
            Type = typeof(Shared.PodLogViewer),
            Name = GetName(pod),
            Parameters = new Dictionary<string, object>()
            {
                { "Id", id }
            }
        });


        //StringBuilder initialLogs=new StringBuilder();

        //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        //cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(5000));

        //while (!cancellationTokenSource.Token.IsCancellationRequested)
        //{
        //    string line = await logReader.ReadLineAsync();
        //    initialLogs.AppendLine(line);
        //}
        string line = await logReader.ReadLineAsync();
        await _jsRuntime.InvokeVoidAsync("setPodLogViewer", podLogViewerId, line+"\n");

        _ = Task.Run(async () =>
        {
            while (true)
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(5000));

                StringBuilder sb = new StringBuilder();

                int lc = 0;

                while (lc<25 && !cancellationTokenSource.Token.IsCancellationRequested && await logReader.ReadLineAsync() is { } line)
                {
                    sb.AppendLine(line);
                    lc++;
                }
                await _jsRuntime.InvokeVoidAsync("AppendPodLog", podLogViewerId,sb.ToString());
            }
        }).ConfigureAwait(false);
    }

    public string GetId(V1Pod pod)
    {
        return pod.Namespace() + "_" + _kindName.ToLower() + "_" + pod.Name();
    }
    public string GetName(V1Pod pod)
    {
        return _kindName + ": " + pod.Name();
    }
}