using MauiAppClient.ViewModels;

namespace MauiAppClient.Views;

public partial class FlyoutMenuPage : ContentPage
{
    public FlyoutMenuPage()
    {
        InitializeComponent();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        var k8sService = Handler.MauiContext.Services.GetService<K8sService>();
        var k8sContextList = k8sService.GetAllContexts();

        var itemsSource = new List<FlyoutPageItem>();
        foreach (var item in k8sContextList)
        {
            itemsSource.Add(new FlyoutPageItem
            {
                IconSource = "contacts.png",
                Title = item.Key,
                TargetType = typeof(K8sPage),
            });
        }
        collectionView.ItemsSource = itemsSource;
    }
}