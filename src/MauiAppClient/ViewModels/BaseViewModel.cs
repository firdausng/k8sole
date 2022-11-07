using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiAppClient.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(isNotBusy))]
    bool isBusy;
    
    bool isNotBusy => !isBusy;

    [ObservableProperty]
    string title;
}