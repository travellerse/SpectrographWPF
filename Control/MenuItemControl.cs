using Microsoft.Win32;
using SpectrographWPF.FrameData;
using System.IO;
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
            SaveFileDialog dlg = new()
            {
                FileName = "SpectrographData",
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };
            if (dlg.ShowDialog() == true)
            {
                File.WriteAllText(dlg.FileName, LastLightFrameData.ToString());
            }
        }

        private void DarkFieldDataImport_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new()
            {
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var reader = new StreamReader(dlg.FileName);
                    string line = reader.ReadLine();
                    var waveLength = new double[10550];
                    var value = new double[10550];
                    for (int i = 0; i < 10550; i++)
                    {
                        line = reader.ReadLine();
                        waveLength[i] = double.Parse(line.Split(',')[0]);
                        value[i] = double.Parse(line.Split(',')[1]);
                    }
                    DarkFieldData = new LightFrameData(waveLength, value, 1);
                    reader.Close();
                    PlotUpdate(DarkFieldData);
                }
                catch (Exception)
                {
                    Alert("导入失败，请检查文件格式。");
                    return;
                }
            }
        }
    }
}
