using AppCore.Services.K8s;
using MauiAppClient.ViewModels;

namespace MauiAppClient.Views;

public partial class MainPage2 : FlyoutPage
{
    private K8sService _k8sService;
    private readonly K8sPage _k8SPage;
    private CurrentK8SContext _currentk8sContextClient;

    public MainPage2(MainViewModel mainViewModel, K8sService k8SService, K8sPage k8SPage, CurrentK8SContext k8SContextClient)
	{
		InitializeComponent();

        _k8sService = k8SService;
        _k8SPage = k8SPage;
        _currentk8sContextClient = k8SContextClient;
        BindingContext = mainViewModel;
        FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
        flyoutPage.collectionView.SelectionChanged += OnSelectionChanged;
    }

    void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not FlyoutPageItem item) return;
        _currentk8sContextClient.Client = _k8sService.GetK8sContext(item.Title);
        var k8sContextService = this.Handler.MauiContext.Services.GetService<K8sContextService>();
        Detail = new NavigationPage(new K8sPage(_currentk8sContextClient, k8sContextService));
        if(FlyoutLayoutBehavior == FlyoutLayoutBehavior.Popover)
        {
            IsPresented = false;
        } 
    }

}