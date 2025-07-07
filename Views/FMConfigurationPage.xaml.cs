using RadioControlApp.ViewModels;

namespace RadioControlApp.Views;

public partial class FMConfigurationPage : ContentPage
{
    public FMConfigurationPage(FMConfigurationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}