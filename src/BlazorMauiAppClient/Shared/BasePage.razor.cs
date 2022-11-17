using Microsoft.AspNetCore.Components;

namespace BlazorMauiAppClient.Shared;
public partial class BasePage
{
    [Parameter]
    public string PageTitle { get; set; } = string.Empty;
}