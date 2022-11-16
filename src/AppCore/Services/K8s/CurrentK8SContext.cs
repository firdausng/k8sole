using AppCore.Services.K8s.Models;

namespace AppCore.Services.K8s;

public class CurrentK8SContext
{
    public K8SContextClient Client { get; set; }

    private List<V1Namespace> _activeNamespaceList = new List<V1Namespace>();
    public IReadOnlyList<V1Namespace> ActiveNamespaceList  => _activeNamespaceList;

    public event EventHandler ActiveNamespaceChanged;
    public string NamespaceFilter { get; set; } = string.Empty;

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