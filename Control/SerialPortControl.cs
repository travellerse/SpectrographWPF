using System.Diagnostics;
using SpectrographWPF.Utils;
using System.Timers;
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
                Information(string.Format("查找到可以使用的端口{0}个。", portsComboBox.Items.Count.ToString()));
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
                    Information("端口已关闭。");
                }
                catch
                {
                    Alert("Oops，关闭端口失败；请检查端口是否被占用。");
                    return;
                }
            }

        }

        public int loseFrameCount = 0;

        public void DataUpdate()
        {
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();
            var isVirtualSerial = true;
            if (virtualSerialComboBox.Text == "True")
                isVirtualSerial = true;
            else if (virtualSerialComboBox.Text == "False")
                isVirtualSerial = false;
            byte[] rawData;
            try
            {
                rawData = serialPortManager.Update(isVirtualSerial);
            }
            catch (Exception e)
            {
                loseFrameCount++;
                Alert(e.ToString());
                return;
            }
            var data =
                Conversion.ToSpecifiedText(rawData, Conversion.ConversionType.Hex, System.Text.Encoding.UTF8);
            //recvDataRichTextBox.AppendText(data);
            //recvDataRichTextBox.ScrollToEnd();
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                FrameData.FrameData frameData = new(data, isVirtualSerial);
                int n = frameData.Amplitude.Length;
                plot.Plot.Clear();
                plot.Plot.Add.Signal(frameData.Amplitude);
                plot.Plot.Axes.SetLimits(1, n, frameData.Amplitude.Min() - 5, frameData.Amplitude.Max() + 5);
                plot.Refresh();
            }));
            sw.Stop();
            var time = Math.Round(sw.ElapsedTicks / (decimal)Stopwatch.Frequency, 6);
            string maxData = isVirtualSerial ? "63785" : "63425";
            Information("数据长度:" + data.Length.ToString() + "/" + maxData + "   用时:" + time + "   Max FPS:" + Math.Round(1 / time, 2) + "   丢帧:" + loseFrameCount);
        }

        public void SendDataButton_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                DataUpdate();
            }));
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
                    loseFrameCount = 0;
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
