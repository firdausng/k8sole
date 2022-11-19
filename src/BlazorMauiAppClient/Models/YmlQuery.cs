namespace BlazorMauiAppClient.Models;

public class YmlQuery
{
    public YmlQuery(string name,string nameSpace)
    {
        this.Name = name;
        this.Namespace=nameSpace;
    }

    public string Name { get; set; } 
    public string Namespace { get; set; }
}