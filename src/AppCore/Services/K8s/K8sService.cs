namespace AppCore.Services.K8s;
public class K8sService
{
	private readonly ILogger<K8sService> _logger;
	private readonly K8SConfiguration _kubeConfig;
	private Dictionary<string, K8SContextClient> _k8SContextClientList = new Dictionary<string, K8SContextClient>();
    
    public event EventHandler ActiveK8sContextChanged;

    public K8sService(ILogger<K8sService> logger)
	{
		_logger = logger;
        _kubeConfig = KubernetesClientConfiguration.LoadKubeConfig();
        if (_kubeConfig is null)
        {
            _logger.LogError("No Kubernetes configuration found in this machine");
        }

        foreach (var context in _kubeConfig.Contexts)
        {
            if (_k8SContextClientList.ContainsKey(context.Name) is false)
            {
                _k8SContextClientList.Add(context.Name, new K8SContextClient(context.Name));
            }
        }
    }

    public Dictionary<string, K8SContextClient> GetAllContexts()
    {
        return _k8SContextClientList;
    }

    public K8SContextClient? GetK8sContext(string contextName)
    {
        if (_k8SContextClientList.ContainsKey(contextName) is false)
        {
            _k8SContextClientList.Add(contextName, new K8SContextClient(contextName));
        }

        if (_k8SContextClientList[contextName].Client is null)
        {
            var config = KubernetesClientConfiguration.BuildConfigFromConfigObject(_kubeConfig, contextName);
            var client = new Kubernetes(config);
            _k8SContextClientList[contextName].Client = client;
            _k8SContextClientList[contextName].Status = K8SContextStatus.CONNECTED;
            
        }

        ActiveK8sContextChanged?.Invoke(this, EventArgs.Empty);

        return _k8SContextClientList[contextName];
    }
}
