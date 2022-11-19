using AppCore.Services.K8s.Models;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace BlazorMauiAppClient.Shared
{
    public partial class TopNavBar
    {
        private Dictionary<string, K8SContextClient> _k8sContextList;
        private string _title = "No K8s Context";
        private IList<TopNavBarNamespace> _contextNamespaces = new List<TopNavBarNamespace>();
        private IList<TopNavBarNamespace> _filterContextNamespaces = new List<TopNavBarNamespace>();
        private IList<TopNavBarNamespace> _enabledContextNamespaces = new List<TopNavBarNamespace>();

        [Inject]
        public CurrentK8SContext CurrentK8SContextClient { get; set; }

        [Inject]
        public K8sContextService K8SContextService { get; set; }

        [Inject]
        public K8sService K8sService { get; set; }

        [Inject]
        private NamespaceService _namespaceService { get; set; }

        [Inject]
        public SharedState SharedState { get; set; }

        private string _currentNamespaceList = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            _k8sContextList = K8sService.GetAllContexts();
            
            if (_k8sContextList != null)
            {
                await SetCurrentContext(_k8sContextList.First(c => c.Value.ContextName.Contains("sci-aksc-weu-02", StringComparison.OrdinalIgnoreCase)));
            }
            var list = await _namespaceService.GetCurrentNamespaceListAsync();
            _currentNamespaceList = string.Join(", ", list.Select(n => n.Metadata.Name));

        }

        private async Task SearchNamespace(object checkedValue)
        {
            if (checkedValue != null)
            {
                var query = checkedValue as string;
                _filterContextNamespaces = _contextNamespaces
                    .Where(c => c.Namespace.Name().Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                StateHasChanged();
            }
        }


        private async Task UpdateCurrentNamespace(string name, object checkedValue)
        {
            var selectedNamespace = _contextNamespaces.FirstOrDefault(n => n.Namespace.Name().Equals(name, StringComparison.OrdinalIgnoreCase));
            if (checkedValue is bool selected)
            {
                await _namespaceService.AddCurrentNamespaceAsync(name);
                if (selectedNamespace is not null)
                {
                    selectedNamespace.Selected = selected;
                    var list = await _namespaceService.GetCurrentNamespaceListAsync();
                    _currentNamespaceList = string.Join(", ", list.Select(n => n.Metadata.Name));
                    _enabledContextNamespaces = list.Select(n => new TopNavBarNamespace(n, selected)).ToList();
                    StateHasChanged();
                }
            }
        }

        private async Task ClearCurrentNamespace()
        {
            _namespaceService.ClearCurrentNamespaces();
            var list = await _namespaceService.GetCurrentNamespaceListAsync();
            _currentNamespaceList = string.Join(", ", list.Select(n => n.Metadata.Name));
            StateHasChanged();
        }

        private async Task SetCurrentContext(KeyValuePair<string, K8SContextClient> k8sContextClient)
        {
            CurrentK8SContextClient.Client = K8sService.GetK8sContext(k8sContextClient.Key);
            _title = k8sContextClient.Key;
            var @namespaceList = await _namespaceService.GetAllAsync();
            _contextNamespaces = @namespaceList.Select(n => new TopNavBarNamespace(n, false)).ToList();
            _filterContextNamespaces = _contextNamespaces;
            _enabledContextNamespaces = _filterContextNamespaces;
        }
    }
}


public class TopNavBarNamespace
{
    public TopNavBarNamespace(V1Namespace ns, bool selected)
    {
        Namespace = ns;
        Selected = selected;
    }

    public bool Selected { get; set; }
    public V1Namespace Namespace { get; set; }
}