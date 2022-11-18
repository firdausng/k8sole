namespace BlazorMauiAppClient.ViewModels;

public class ComponentMetadata
{
    public Type Type { get; set; }
    public string Name { get; set; }
    public bool Active { get; set; }
    public Dictionary<string, object> Parameters { get; set; } =
        new Dictionary<string, object>();
}