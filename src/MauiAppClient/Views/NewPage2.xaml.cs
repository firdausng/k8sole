namespace MauiAppClient.Views;

public partial class NewPage2 : ContentPage
{
    int count = 0;
    private Window _windows;

    public NewPage2()
	{
		InitializeComponent();
    }

    void OnOpenWindowClicked(object sender, EventArgs e)
    {
        _windows = new Window(new NewPage1());
        Application.Current.OpenWindow(_windows);
    }

    void OnCloseWindowClicked(object sender, EventArgs e)
    {
        var window = GetParentWindow();

        if (window is not null)
            Application.Current.CloseWindow(_windows);
    }
}

