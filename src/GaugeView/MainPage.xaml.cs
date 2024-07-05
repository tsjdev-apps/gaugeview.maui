namespace GaugeView;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnIncrementClicked(
        object sender, EventArgs e)
    {
        GaugeView.Value += 10;
    }

    private void OnDecrementClicked(
        object sender, EventArgs e)
    {
        GaugeView.Value -= 10;
    }
}
