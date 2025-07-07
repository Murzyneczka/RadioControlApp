using RadioControlApp.ViewModels;

namespace RadioControlApp.Views;

public partial class IRConfigurationPage : ContentPage
{
    public IRConfigurationPage(IRConfigurationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}