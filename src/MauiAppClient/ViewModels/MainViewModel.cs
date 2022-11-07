using AppCore.Services.K8s;
using System.Collections.ObjectModel;

namespace MauiAppClient.ViewModels;
public partial class MainViewModel : BaseViewModel
{
    public ObservableCollection<K8SContextClient> K8SContextClientList { get; private set; }
	public Command GetK8sContextCommand { get; }
	public Command TestCommand { get; }
    public K8sService K8SService { get; }
	public FlyoutPageItem FlyoutPageItemList { get; set; }

	public MainViewModel(K8sService k8SService)
	{
		Title = "K8s Console";
		K8SService = k8SService;
		GetK8sContextCommand = new Command(async () => await GetK8sContextListAsync());

        K8SContextClientList = new ObservableCollection<K8SContextClient>();
        var contextList = K8SService.GetAllContexts().Select(c => c.Value).ToList();
        contextList.ForEach(context => K8SContextClientList.Add(context));
		TestCommand = new Command(() =>
		{
			Debug.WriteLine("Test");
		});
    }

	async Task GetK8sContextListAsync()
	{
		if (IsBusy)
		{
			return;
		}

		try
		{
			if (K8SContextClientList.Count != 0)
			{
                K8SContextClientList.Clear();
            }

            var contextList = K8SService.GetAllContexts().Select(c => c.Value).ToList();
			contextList.ForEach(context => K8SContextClientList.Add(context));
        }
		catch (Exception ex)
		{
			Debug.WriteLine("Cannot get k8s context");
			await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
		}
		finally
		{
			IsBusy = false;
		}
	}
}


public class CurrentWindow
{
	public CurrentWindow(CurrentK8SContext currentK8SContext)
	{

	}
}