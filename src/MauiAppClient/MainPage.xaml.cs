using AppCore.Services.K8s;
using AppCore.Services.K8s.Models;
using k8s;
using k8s.Models;
using MauiAppClient.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MauiAppClient
{
    public partial class MainPage : ContentPage
    {
        private K8SContextClient _k8sClient;

        public MainPage(MainViewModel mainViewModel)
        {
            InitializeComponent();
            BindingContext = mainViewModel;
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            var k8sService = this.Handler.MauiContext.Services.GetService<K8sService>();
            _k8sClient = k8sService.GetK8sContext("docker-desktop");
            
        }

    }
}