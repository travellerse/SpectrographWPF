using SpectrographWPF.FrameData;
using System.Windows;

namespace SpectrographWPF
{
    public partial class MainWindow : Window
    {
        public bool IsDebug = false;
        public void DebugMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            IsDebug = !IsDebug;
            if (IsDebug)
            {
                debugMenuItem.Header = "关闭调试";
                startWorkButton.IsEnabled = true;
                sendDataButton.IsEnabled = true;
                FrameDataServer.Producer.IsDebug = true;
            }
            else
            {
                debugMenuItem.Header = "打开调试";
                if (!serialPortManager.IsOpen())
                {
                    startWorkButton.IsEnabled = false;
                    sendDataButton.IsEnabled = false;
                }
                FrameDataServer.Producer.IsDebug = false;
            }
        }
    }
}
