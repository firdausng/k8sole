using AppCore.Services.K8s.Interfaces;
using BlazorMauiAppClient.ViewModels;
using Microsoft.JSInterop;

namespace BlazorMauiAppClient.YmlEditor;

public class YmlEditorPopper
{
    private readonly string _kindName;
    private readonly CurrentK8SContext _currentK8SContextClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly IYmlCrud _ymlCrud;
    private readonly bool _readOnlyMode;

    public YmlEditorPopper(string kindName, CurrentK8SContext currentK8SContextClient, IJSRuntime jsRuntime, IYmlCrud ymlCrud, bool readOnlyMode = false)
    {
        _kindName = kindName;
        _currentK8SContextClient = currentK8SContextClient;
        _jsRuntime = jsRuntime;
        _ymlCrud = ymlCrud;
        _readOnlyMode = readOnlyMode;
    }
    public async Task PopEditYmlAsync(YmlQuery ymlQuery)
    {
        string yml = await _ymlCrud.GetYmlAsync(ymlQuery.Name, ymlQuery.Namespace);
        string id = GetId(ymlQuery);
        string editorId = id + Constants.YmlEditorExtension;
        _currentK8SContextClient.AddTabComponents(id, new ComponentMetadata()
        {
            Type = typeof(Shared.YmlEditor),
            Name = GetName(ymlQuery),
            Parameters = new Dictionary<string, object>()
            {
                { "Id", id },
                {"ReadOnlyMode", _readOnlyMode}
            }
        });
        await _jsRuntime.InvokeVoidAsync("setYmlEditor", editorId, yml, _readOnlyMode);
    }

    public string GetId(YmlQuery ymlQuery)
    {
        return ymlQuery.Namespace + "_" + _kindName.ToLower() + "_" + ymlQuery.Name;
    }
    public string GetName(YmlQuery ymlQuery)
    {
        return _kindName + ": " + ymlQuery.Name;
    }
}