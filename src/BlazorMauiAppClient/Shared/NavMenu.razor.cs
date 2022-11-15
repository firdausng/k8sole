using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using AppCore.Services.K8s;
using AppCore.Services.K8s.Models;

namespace BlazorMauiAppClient.Shared
{
    public partial class NavMenu
    {
        private bool collapseNavMenu = true;
        private Dictionary<string, K8SContextClient> _k8sContextList;
        private string _title = "Namespace";

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;
        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        [Inject]
        public CurrentK8SContext CurrentK8SContextClient { get; set; }
        
        [Inject]
        public K8sContextService K8SContextService { get; set; }

        [Inject]
        public K8sService K8sService { get; set; }
        private readonly string _allNamespaceTitle = "All";

        protected override void OnInitialized()
        {
            _k8sContextList = K8sService.GetAllContexts();
        }

        private void SetCurrentContext(KeyValuePair<string, K8SContextClient> k8sContextClient)
        {
            CurrentK8SContextClient.Client = K8sService.GetK8sContext(k8sContextClient.Key);
            _title = k8sContextClient.Key;
        }
    }
}