using k8s.Models;

namespace BlazorMauiAppClient.ViewModels;

public abstract class BaseViewModel
{
    public abstract string Name { get; set; }
    public abstract string Namespace { get; set; }
    public abstract string Age { get; set; }

    public string GetAgeString(DateTime creationDateTime)
    {
        var diff = DateTime.Now - creationDateTime;
        string ageStr = string.Empty;
        if (diff.Days > 0)
        {
            ageStr += diff.Days + "d ";
        }

        if (diff.Hours > 0)
        {
            ageStr += diff.Hours + "h ";
        }
        if (diff.Minutes > 0)
        {
            ageStr += diff.Minutes + "m ";
        }
        return ageStr;
    }
}