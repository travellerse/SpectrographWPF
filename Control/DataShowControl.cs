using System.Windows;

namespace SpectrographWPF
{
    public partial class MainWindow : Window
    {
        private void RecvDataMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (recvDataGroupBox.Visibility == Visibility.Visible)
            {
                recvDataMenuItem.IsChecked = false;
                recvDataGroupBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                recvDataMenuItem.IsChecked = true;
                recvDataGroupBox.Visibility = Visibility.Visible;
            }
        }
    }
}
