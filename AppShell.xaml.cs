namespace RadioControlApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Rejestracja tras dla nawigacji
        Routing.RegisterRoute("DashboardPage", typeof(Views.DashboardPage));
        Routing.RegisterRoute("IRConfigurationPage", typeof(Views.IRConfigurationPage));
        Routing.RegisterRoute("FMConfigurationPage", typeof(Views.FMConfigurationPage));
    }
}