using AppCore.Services.K8s.Models;
using BlazorMauiAppClient.ViewModels;

namespace AppCore.Services.K8s;

public class CurrentK8SContext
{
    public K8SContextClient Client { get; set; }

    private List<V1Namespace> _activeNamespaceList = new List<V1Namespace>();
    public IReadOnlyList<V1Namespace> ActiveNamespaceList => _activeNamespaceList;

    public event EventHandler ActiveNamespaceChanged;

    public string NamespaceFilter { get; set; } = string.Empty;

    private Dictionary<string, ComponentMetadata> _tabComponents = new();
    public event EventHandler TabComponentsChanged;

    public void ResetTabComponentActiveness()
    {
        foreach (var componentMetadata in _tabComponents)
        {
            componentMetadata.Value.Active = false;
        }
    }
    public void AddTabComponents(string key, ComponentMetadata value)
    {
        ResetTabComponentActiveness();
        var exist = _tabComponents.TryGetValue(key, out var currentComponentMetadata);
        if (!exist)
        {
            value.Active = true;
            _tabComponents.Add(key, value);
        }
        else
        {
            currentComponentMetadata!.Active = true;
        }
        TabComponentsChanged?.Invoke(this, EventArgs.Empty);
    }
    public void RemoveTabComponents(string key)
    {
        var reIndex = false;
        if (_tabComponents[key].Active)
        {
            if (_tabComponents.Count > 1)
                reIndex = true;
        }
        _tabComponents.Remove(key);
        if (reIndex)
        {
            _tabComponents.Last().Value.Active = true;

        }
        TabComponentsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ActiveTabComponent(string key)
    {
        if (_tabComponents.ContainsKey(key))
        {
            ResetTabComponentActiveness();
            _tabComponents[key].Active = true;
        }
    }

    public IReadOnlyDictionary<string, ComponentMetadata> GetTabComponentMetadatas()
    {
        return _tabComponents;
    }


    public void AddActiveNamespace(V1Namespace @namespace)
    {
        if (!_activeNamespaceList.Any(n => n.Name().Equals(@namespace.Name())))
        {
            _activeNamespaceList.Add(@namespace);
            ActiveNamespaceChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            RemoveActiveNamespace(@namespace);
        }
    }

    public void RemoveActiveNamespace(V1Namespace @namespace)
    {
        //_activeNamespaceList.Remove(@namespace);
        //ActiveNamespaceChanged?.Invoke(this, EventArgs.Empty);
        var namespaceToBeRemoved = ActiveNamespaceList.FirstOrDefault(n => n.Name().Equals(@namespace.Name()));
        if (namespaceToBeRemoved is not null)
        {
            _activeNamespaceList.Remove(namespaceToBeRemoved);
            ActiveNamespaceChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void ClearActiveNamespace()
    {
        _activeNamespaceList.Clear();
        ActiveNamespaceChanged?.Invoke(this, EventArgs.Empty);
    }
}