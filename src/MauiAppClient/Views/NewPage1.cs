namespace MauiAppClient.Views;

public class NewPage1 : ContentPage
{
    public NewPage1()
    {
        Title = "NewPage1";
        Content = new VerticalStackLayout
        {
            Children =
            {
                new Label
                {
                    HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center,
                    Text = "Welcome to .NET MAUI! NewPage1"
                },
                new HorizontalStackLayout
                {

                }
            }
        };

       
    }
}