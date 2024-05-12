using System.IO;
using Microsoft.Win32;
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

        private void DataExport_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "SpectrographData";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";
            if (dlg.ShowDialog() == true)
            {
                File.WriteAllText(dlg.FileName, LastLightFrameData.ToString());
            }
        }
    }
}
