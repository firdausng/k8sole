using AppCore.Services.K8s;
using Microsoft.AspNetCore.Components;

namespace BlazorMauiAppClient.Shared;

public partial class BottomBar
{
    [Inject]
    private CurrentK8SContext CurrentContext { get; set; }

    protected override async Task OnInitializedAsync()
    {

        CurrentContext.TabComponentsChanged += async (s, e) =>
        {
            _ = InvokeAsync(async() => { StateHasChanged(); });
        };
    }
}