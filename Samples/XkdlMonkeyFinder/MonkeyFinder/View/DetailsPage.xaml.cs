namespace MonkeyFinder.View;

public partial class DetailsPage
{
    public DetailsPage(MonkeyDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}