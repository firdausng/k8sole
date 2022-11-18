using AppCore.Services.K8s;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace BlazorMauiAppClient.Pages;
public partial class PodDetails
{
    private ElementReference podSecretData;

    [Parameter]
    public string DetailsTitle { get; set; } = "Detail";

    [Parameter]
    public V1Pod Pod { get; set; }

    [Inject]
    public CurrentK8SContext CurrentK8SContextClient { get; set; }


    protected override async Task OnInitializedAsync()
    {
        
    }

    protected override async Task OnParametersSetAsync()
    {
    }

    
}