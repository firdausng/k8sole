using AppCore.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using AppCore.Services.K8s;
using BlazorMauiAppClient.ViewModels;
using k8s.Models;
using k8s;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using Microsoft.Maui.Controls.Shapes;

namespace BlazorMauiAppClient.Pages;
public partial class Services
{
    List<KeyValuePair<string, string>> DetailsData = new();

    [Inject]
    public CurrentK8SContext CurrentK8SContextClient { get; set; }

    [Inject]
    private ServicesService ServicesService { get; set; }

    [Inject]
    public IJSRuntime JS { get; set; }

    IQueryable<V1ServiceVm>? items = Enumerable.Empty<V1ServiceVm>().AsQueryable();
    PaginationState pagination = new PaginationState { ItemsPerPage = 20 };

    IQueryable<V1ServiceVm>? FilteredItems => items?.Where(x => x.Namespace.Contains(CurrentK8SContextClient.NamespaceFilter, StringComparison.CurrentCultureIgnoreCase));

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
    }

    public async Task Setup()
    {
        var services = await ServicesService.GetNamespaced();
        items = services.Select(x => new V1ServiceVm(x)).AsQueryable();

    }

    public void ShowDetail(V1ServiceVm service)
    {
        DetailsData.Add(new KeyValuePair<string, string>("Name", service.Name));
        DetailsData.Add(new KeyValuePair<string, string>("Namespace", service.Namespace));
        //DetailsData.Add(new KeyValuePair<string, string>("Kind", service.Kind));
        //DetailsData.Add(new KeyValuePair<string, string>("ApiGroup", service.ApiGroup()));
        //DetailsData.Add(new KeyValuePair<string, string>("ApiVersion", service.ApiVersion));
        //DetailsData.Add(new KeyValuePair<string, string>("CreationTimestamp", service.CreationTimestamp().ToString()));
        //DetailsData.Add(new KeyValuePair<string, string>("DeletionTimestamp", service.DeletionTimestamp().ToString()));
    }
}

public class V1ServiceSummaryVm : BaseViewModel
{
    public string Type { get; set; }
    public string ClusterIP { get; set; }
    public List<ServicePortVm> PortVms { get; set; }

    public string PortSummary
    {
        get
        {
            if (PortVms.IsNullOrEmpty()) return "None";
            string summary = string.Empty;
            for (int i = 0; i < Math.Min(2, PortVms.Count); i++)
            {
                summary += " " + PortVms[i].ToSummary();
            }

            return summary.Truncate(30);

        }
    }

    public IList<string> ExternalIPs { get; set; }

    public string ExternalIPsSummary
    {
        get
        {
            if (ExternalIPs.IsNullOrEmpty()) return "None";
            string summary = string.Empty;
            for (int i = 0; i < Math.Min(2, ExternalIPs.Count); i++)
            {
                summary += ExternalIPs[i];
            }

            return summary.Truncate(30);

        }
    }

    public IDictionary<string, string> Selector { get; set; }

    public ServiceStatus Status => ServiceStatus.Active;


    public string SelectorSummary
    {
        get
        {
            if (Selector.IsNullOrEmpty()) return "None";
            string summary = string.Empty;

            foreach (var selector in Selector.Take(2))
            {
                summary += selector.Key + "=" + selector.Value;
            }
            return summary.Truncate(30);

        }
    }

    public V1ServiceSummaryVm(V1Service service)
    {
        Name = service.Name();
        Namespace = service.Namespace();
        Age = GetAgeString(service.Metadata.CreationTimestamp!.Value);
        Type = service.Spec.Type;
        ClusterIP = service.Spec.ClusterIP;
        PortVms = service.Spec.Ports.Select(p => new ServicePortVm(p)).ToList();
        ExternalIPs = service.Spec.ExternalIPs;
        Selector = service.Spec.Selector;
    }

    public sealed override string Name { get; set; }
    public sealed override string Namespace { get; set; }
    public sealed override string Age { get; set; }
}
public class V1ServiceVm : V1ServiceSummaryVm
{
    public V1ServiceVm(V1Service service) : base(service)
    {
    }
}

public class ServicePortVm
{
    public ServicePortVm(V1ServicePort servicePort)
    {
        Port = servicePort.Port.ToString();
        TargetPort = servicePort.TargetPort.ToString();
        NodePort = servicePort.NodePort.ToString();
        Protocol = servicePort.Protocol;
        Name = servicePort.Name;
    }
    public string Port { get; set; }
    public string TargetPort { get; set; }
    public string NodePort { get; set; }
    public string Protocol { get; set; }
    public string Name { get; set; }

    public string ToSummary()
    {
        return Port + ":" + Name + "/" + Protocol;
    }
    public override string ToString()
    {
        return Port + ":" + Name + "/" + Protocol + "->" + TargetPort;
    }
}

public enum ServiceStatus
{
    Unknown,
    Active,
    Inactive,
}