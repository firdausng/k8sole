using AppCore.Services.K8s;
using k8s.Models;
using Microsoft.Maui.Controls;

namespace MauiAppClient.Views;

public partial class K8sPage : ContentPage
{
    private readonly CurrentK8SContext _currentK8SContextClient;
    private readonly K8sContextService _k8SContextService;
    private readonly string _allNamespaceTitle = "All";

    public K8sPage(CurrentK8SContext currentK8SContextClient, K8sContextService k8SContextService)
	{
		InitializeComponent();
        _currentK8SContextClient = currentK8SContextClient;
        _k8SContextService = k8SContextService;

        //Title = _allNamespaceTitle;

        //Slider titleView = new Slider { HeightRequest = 44, WidthRequest = 300 };
        var sasa = new HorizontalStackLayout
        {

        };
        sasa.Add(new Frame 
        { 
            Content = new Picker
            {
                ItemsSource = new List<string>
                {
                    "Baboon", "Capuchin Monkey", "Blue Monkey", "Squirrel Monkey"
                },
                
            }
        });
        NavigationPage.SetTitleView(this, sasa);
    }

    protected override async void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        var namespaceList = await _k8SContextService.GetAllNamespace();

        listView.ItemsSource = namespaceList.Items;

        var namespaceMenuBar = new MenuBarItem
        {
            Text = "Namespaces",
        };

        namespaceMenuBar.Add(new MenuFlyoutItem
        {
            Text = _allNamespaceTitle,
            
        });

        

        namespaceList.Items.ToList().ForEach(k8sNamespace =>
        {
            namespaceMenuBar.Add(new MenuFlyoutItem
            {
                Text = k8sNamespace.Metadata.Name,
            });
        });

        


        MenuBarItems.Add(namespaceMenuBar);
    }
}