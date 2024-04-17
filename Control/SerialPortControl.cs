using SpectrographWPF.Utils;
using System.Diagnostics;
using System.Windows;
using Timer = System.Timers.Timer;

namespace SpectrographWPF
{
    public partial class MainWindow : Window
    {
        public void FindPortButton_Click(object sender, RoutedEventArgs e)
        {
            var portList = serialPortManager.FindPort();
            if (portList != null)
            {
                portsComboBox.ItemsSource = portList;
                portsComboBox.SelectedIndex = 0;
                portsComboBox.IsEnabled = true;
                findPortButton.IsEnabled = true;
                Information($"查找到可以使用的端口{portList.Length}个。");
            }
            else
            {
                Alert("Oops，没有查找到可用端口；您可以点击“查找”按钮手动查找。");
                findPortButton.IsEnabled = false;
            }
        }

        public void OpenClosePortButton_Click(object sender, RoutedEventArgs e)
        {
            if (!serialPortManager.IsOpen())
            {
                try
                {
                    serialPortManager.OpenPort(portsComboBox.Text, int.Parse(baudRateComboBox.Text),
                        int.Parse(dataBitsComboBox.Text), int.Parse(stopBitsComboBox.Text));
                    openClosePortButton.Content = "关闭端口";
                    sendDataButton.IsEnabled = true;
                    startWorkButton.IsEnabled = true;
                    Information("端口已打开。");
                }
                catch
                {
                    Alert("Oops，打开端口失败；请检查端口是否被占用。");
                    return;
                }
            }
            else
            {
                try
                {
                    serialPortManager.ClosePort();
                    openClosePortButton.Content = "打开";
                    sendDataButton.IsEnabled = false;
                    startWorkButton.IsEnabled = false;
                    Information("端口已关闭。");
                }
                catch
                {
                    Alert("Oops，关闭端口失败；请检查端口是否被占用。");
                    return;
                }
            }

        }

        public int LoseFrameCount = 0;

        public void DataUpdate()
        {
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();
            var isVirtualSerial = virtualSerialComboBox.Text == "True";
            byte[] rawData;
            try
            {
                rawData = serialPortManager.Update(isVirtualSerial, sendDataTextBox.Text);
            }
            catch (Exception e)
            {
                LoseFrameCount++;
                Alert(e.ToString());
                return;
            }
            var data = Conversion.ToSpecifiedText(rawData, Conversion.ConversionType.Hex, System.Text.Encoding.UTF8);

            PlotUpdate(data, isVirtualSerial);

            sw.Stop();
            var time = Math.Round(sw.ElapsedTicks / (decimal)Stopwatch.Frequency, 6);
            string maxData = isVirtualSerial ? "63785" : "63425";
            Information($"数据长度:{data.Length}/{maxData}   用时:{time}   Max FPS:{Math.Round(1 / time, 2)}   丢帧:{LoseFrameCount}");
        }

        public void PlotUpdate(string data, bool isVirtualSerial)
        {
            FrameData.FrameData frameData = new(data, isVirtualSerial);
            int n = frameData.Amplitude.Length;
            plot.Plot.Clear();
            plot.Plot.Add.Signal(frameData.Amplitude);
            plot.Plot.Axes.SetLimits(1, n, frameData.Amplitude.Min() - 5, frameData.Amplitude.Max() + 5);
            plot.Refresh();
        }

        public void SendDataButton_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(DataUpdate));
        }
        Timer timer = new(1000);

        public void StartWorkButton_Click(object sender, RoutedEventArgs e)
        {
            if (serialPortManager.IsOpen())
            {
                if (!timer.Enabled)
                {
                    startWorkButton.Content = "停止";
                    FpsComboBox.IsEnabled = false;
                    timer.Interval = 1000.0 / int.Parse(FpsComboBox.Text);
                    LoseFrameCount = 0;
                    timer.Start();
                }
                else
                {
                    startWorkButton.Content = "开始";
                    FpsComboBox.IsEnabled = true;
                    timer.Stop();
                }
            }
            else
            {
                Alert("Oops，端口未打开；请打开端口后再试。");
            }
        }
    }
}
