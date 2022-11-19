using Microsoft.AspNetCore.Components;

namespace BlazorMauiAppClient.Pages;

public partial class SecretDetail
{
    [Parameter] 
    public string DetailsTitle { get; set; } = "Detail";

    [Parameter] 
    public V1Secret Secret { get; set; }

    [Inject] public CurrentK8SContext CurrentK8SContextClient { get; set; }

    public List<SecretDetailVm> SecretDetailVmList { get; set; }


    protected override void OnInitialized()
    {
        if (Secret != null)
        {
            SecretDetailVmList = Secret.Data.Select(d => new SecretDetailVm
            {
                Key = d.Key,
                EncodedValue = Convert.ToBase64String(d.Value),
                DecodedValue = Encoding.UTF8.GetString(d.Value)
            })
                    .ToList();
            StateHasChanged();
        }
    }
}

public class SecretDetailVm
{
    public string Key { get; set; }
    public string DecodedValue { get; set; }
    public string EncodedValue { get; set; }

    public bool IsDecode { get; set; } = false;
}