using ScottPlot;
using SpectrographWPF.FrameData;
using SpectrographWPF.Manager;
using SpectrographWPF.Utils;
using SpectrographWPF.Utils.Algorithm;
using System.Diagnostics;
using System.Windows;

namespace SpectrographWPF
{
    public partial class MainWindow : Window
    {
        public void FindPortButton_Click(object sender, RoutedEventArgs e)
        {
            var portList = SerialPortManager.FindPort();
            if (portList.Length > 0)
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
                        int.Parse(dataBitsComboBox.Text), int.Parse(stopBitsComboBox.Text),
                        virtualSerialComboBox.Text == "True");
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
            var sw = Stopwatch.StartNew();
            sw.Start();
            var isVirtualSerial = virtualSerialComboBox.Text == "True";
            var rawData = serialPortManager.Update(sendDataTextBox.Text);
            try
            {
                //rawData = serialPortManager.Update(isVirtualSerial, sendDataTextBox.Text);
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
            var maxData = isVirtualSerial ? "63785" : "63425";
            Information(
                $"数据长度:{data.Length}/{maxData}   用时:{time}   Max FPS:{Math.Round(1 / time, 2)}   丢帧:{LoseFrameCount}");
        }

        public void PlotUpdate(string data, bool isVirtualSerial)
        {
            FrameData.FrameData frameData = new(data, isVirtualSerial);
            LightFrameData lightFrameData = new(frameData);


            plot.Plot.Clear();


            var barsPlot = plot.Plot.Add.Bars(lightFrameData.WaveLength, lightFrameData.Value);
            foreach (var bar in barsPlot.Bars)
            {
                bar.BorderLineWidth = (float)((lightFrameData.WaveLength.Max() - lightFrameData.WaveLength.Min()) /
                                              lightFrameData.WaveLength.Length);
                bar.FillColor = Conversion.RgbCalculator.Calc(bar.Position);
                bar.BorderColor = Conversion.RgbCalculator.Calc(bar.Position);
            }
            //plot.Plot.Add.SignalXY(lightFrameData.WaveLength, Ss);

            if ((bool)autoPeakingCheckBox.IsChecked)
            {
                var peaks = new SymmetricZeroAreaPeaking(300, 100, 200).Apply(lightFrameData);
                for (int i = 0; i < peaks.Length; i++)
                {
                    var line = plot.Plot.Add.VerticalLine(peaks[i].Index);
                    line.LinePattern = LinePattern.Dashed;
                }
            }

            plot.Plot.Axes.SetLimits(lightFrameData.WaveLength.Min(), lightFrameData.WaveLength.Max(), 400,
                lightFrameData.Value.Max() + 1);
            plot.Refresh();
        }


        public void PlotUpdate(LightFrameData lightFrameData)
        {
            plot.Plot.Clear();

            var barsPlot = plot.Plot.Add.Bars(lightFrameData.WaveLength, lightFrameData.Value);
            int index = 0;
            foreach (var bar in barsPlot.Bars)
            {
                bar.BorderLineWidth = (float)((lightFrameData.WaveLength.Max() - lightFrameData.WaveLength.Min()) / lightFrameData.WaveLength.Length);
                bar.FillColor = lightFrameData.Color[index];
                bar.BorderColor = lightFrameData.Color[index++];
            }

            if ((bool)autoPeakingCheckBox.IsChecked)
            {
                var peaks = new SymmetricZeroAreaPeaking(300, 100, 200).Apply(lightFrameData);
                foreach (var peak in peaks)
                {
                    var line = plot.Plot.Add.VerticalLine(peak.Index);
                    line.LinePattern = LinePattern.Dashed;
                }
            }

            plot.Plot.Axes.SetLimits(lightFrameData.WaveLength.Min(), lightFrameData.WaveLength.Max(), 400, lightFrameData.Value.Max() + 1);
            plot.Refresh();
            Information($"延迟:{DateTimeOffset.Now.ToUnixTimeMilliseconds() - lightFrameData.Timestamp}ms");
        }

        public void SendDataButton_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(DataUpdate));
        }

        public void StartWorkButton_Click(object sender, RoutedEventArgs e)
        {
            if (serialPortManager.IsOpen())
            {
                if (!FrameDataServer.IsRunning)
                {
                    startWorkButton.Content = "停止";
                    FpsComboBox.IsEnabled = false;
                    sendDataButton.IsEnabled = false;
                    LoseFrameCount = 0;
                    FrameDataServer.Start();

                }
                else
                {
                    startWorkButton.Content = "开始";
                    FpsComboBox.IsEnabled = true;
                    sendDataButton.IsEnabled = true;
                    FrameDataServer.Stop();

                }
            }
            else
            {
                Alert("Oops，端口未打开；请打开端口后再试。");
            }
        }
    }
}
