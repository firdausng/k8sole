using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using AppCore.Services.K8s;
using AppCore.Services.K8s.Models;
using k8s.Models;

namespace BlazorMauiAppClient.Shared
{
    public partial class NavMenu
    {
        private bool collapseNavMenu = true;
        private Dictionary<string, K8SContextClient> _k8sContextList;
        private IList<V1Namespace> _contextNamespaces = new List<V1Namespace>();
        private string _title = "No Cluster Configured";

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

        [Inject]
        private NamespaceService _namespaceService { get; set; }

        private readonly string _allNamespaceTitle = "All";

        private string _currentNamespaceList = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            _k8sContextList = K8sService.GetAllContexts();
            if (_k8sContextList != null)
            {
                await SetCurrentContext(_k8sContextList.First());
            }
            var list = await _namespaceService.GetCurrentNamespaceListAsync();
            _currentNamespaceList = string.Join(", ", list.Select(n => n.Metadata.Name));
            
        }

        private async Task SetCurrentContext(KeyValuePair<string, K8SContextClient> k8sContextClient)
        {
            CurrentK8SContextClient.Client = K8sService.GetK8sContext(k8sContextClient.Key);
            _title = k8sContextClient.Key;
            _contextNamespaces = await _namespaceService.GetAllAsync();
        }

        private async Task UpdateCurrentNamespace(string name)
        {
            await _namespaceService.AddCurrentNamespaceAsync(name);
            var list = await _namespaceService.GetCurrentNamespaceListAsync();
            _currentNamespaceList = string.Join(", ", list.Select(n => n.Metadata.Name));
            StateHasChanged();
        }

        private async Task ClearCurrentNamespace()
        {
            _namespaceService.ClearCurrentNamespaces();
            var list = await _namespaceService.GetCurrentNamespaceListAsync();
            _currentNamespaceList = string.Join(", ", list.Select(n => n.Metadata.Name));
            StateHasChanged();
        }
    }
}