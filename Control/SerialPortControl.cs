using ScottPlot;
using SpectrographWPF.FrameData;
using SpectrographWPF.Manager;
using SpectrographWPF.Utils;
using SpectrographWPF.Utils.Algorithm;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                openClosePortButton.IsEnabled = true;
                Information($"查找到可以使用的端口{portList.Length}个。");
            }
            else
            {
                Alert("Oops，没有查找到可用端口；您可以点击“查找”按钮手动查找。");
                openClosePortButton.IsEnabled = false;
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

        public void DataUpdate()
        {
            var sw = Stopwatch.StartNew();
            sw.Start();
            var isVirtualSerial = virtualSerialComboBox.Text == "True";
            LightFrameData data;
            if (IsDebug)
            {
                data = new LightFrameData(new FrameData.FrameData());
            }
            else
            {
                byte[]? rawData;
                do
                {
                    rawData = serialPortManager.Update(sendDataTextBox.Text);
                } while (rawData == null);
                data = new LightFrameData(new FrameData.FrameData(Conversion.ToSpecifiedText(rawData, Conversion.ConversionType.Hex, System.Text.Encoding.UTF8), isVirtualSerial));
            }

            PlotUpdate(data);

            sw.Stop();
            var time = Math.Round(sw.ElapsedTicks / (decimal)Stopwatch.Frequency, 6);
            Information(
                $"用时:{time}   Max FPS:{Math.Round(1 / time, 2)}");
        }

        public LightFrameData LastLightFrameData;
        public bool peaking;
        public bool color;

        public LightFrameData DarkFieldData;

        public int frameCount = 0;

        public void PlotUpdate(LightFrameData lightFrameData)
        {

            var beforeRefresh = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            plot.Plot.Clear();
            LastLightFrameData = lightFrameData;
            var Value = new double[lightFrameData.Value.Length];
            if (lightFrameData.frame > 1)
            {
                for (int i = 0; i < Value.Length; i++)
                {
                    Value[i] = lightFrameData.Value[i] / lightFrameData.frame;
                }
            }
            else Value = lightFrameData.Value;

            if ((bool)DarkFieldCheckBox.IsChecked && DarkFieldData != null)
            {
                for (int i = 0; i < Value.Length; i++)
                {
                    Value[i] -= DarkFieldData.Value[i];
                    if (Value[i] < 0) Value[i] = 0;
                }
            }

            double[] WaveLength = lightFrameData.WaveLength;

            for (int i = 0; i < WaveLength.Length; i++)
            {
                WaveLength[i] += -20.8 * Convert.ToDouble(MicrometerTextBox.Text.Split("(")[0]) + 286.1664 + Convert.ToDouble(deltaTextBox.Text);//466.99
            }
            plot.Plot.Add.SignalXY(WaveLength, Value);
            if ((bool)colorCheckBox.IsChecked || color)
            {
                var points = new Coordinates[WaveLength.Length + 2];

                points[0] = new Coordinates(WaveLength.First(), 0);
                for (int i = 1; i <= WaveLength.Length; ++i)
                {
                    points[i] = new Coordinates(WaveLength[i - 1], Value[i - 1]);
                }
                points[WaveLength.Length + 1] = new Coordinates(WaveLength.Last(), 0);

                var polygon = plot.Plot.Add.Polygon(points);
                polygon.FillHatch = new Gradient
                {
                    Colors = lightFrameData.Color
                };
                polygon.LineWidth = 0;
            }

            if ((bool)autoPeakingCheckBox.IsChecked || peaking)
            {
                var peaks = new SymmetricZeroAreaPeaking(W: Convert.ToInt32(peakingWindowTextBox.Text), percent: Convert.ToDouble(peakingValueTextBox.Text)).Apply(lightFrameData);
                foreach (var peak in peaks)
                {
                    var line = plot.Plot.Add.VerticalLine(peak.Index);
                    line.LinePattern = LinePattern.Dashed;
                    line.Text = Math.Round(peak.Index, 2).ToString();
                    line.LabelOppositeAxis = true;
                    line.ExcludeFromLegend = true;
                }
            }

            plot.Plot.Axes.SetLimits(WaveLength.Min(), WaveLength.Max(), 0, 4500);
            plot.Refresh();

            findPeakButton.IsEnabled = true;
            colorButton.IsEnabled = true;
            autoPeakingCheckBox.IsEnabled = true;
            colorCheckBox.IsEnabled = true;

            frameCount++;
            var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            string data_delay = string.Format("{0,3}", beforeRefresh - lightFrameData.Timestamp);
            string render_delay = string.Format("{0,3}", now - beforeRefresh);
            string total_delay = string.Format("{0,3}", now - lightFrameData.Timestamp);
            var info =
                $"数据延迟:{data_delay}ms  渲染延迟:{render_delay}ms  总延迟:{total_delay}ms";
            if (lightFrameData.frame > 1) info += $"  累计帧数:{lightFrameData.frame}";
            Information(info);
        }

        public void SendDataButton_Click(object sender, RoutedEventArgs e)
        {
            peaking = false;
            color = false;
            this.Dispatcher.BeginInvoke(new Action(DataUpdate));
        }

        public void StartWorkButton_Click(object sender, RoutedEventArgs e)
        {
            if (serialPortManager.IsOpen() || IsDebug)
            {
                peaking = false;
                color = false;
                if (!FrameDataServer.IsRunning)
                {
                    startWorkButton.Content = "停止";
                    //FpsComboBox.IsEnabled = false;
                    sendDataButton.IsEnabled = false;
                    IntCheckBox.IsEnabled = false;
                    if ((bool)IntCheckBox.IsChecked) FrameDataServer.Producer.IsInt = true;
                    else FrameDataServer.Producer.IsInt = false;
                    FrameDataServer.Start();
                }
                else
                {
                    startWorkButton.Content = "开始";
                    //FpsComboBox.IsEnabled = true;
                    sendDataButton.IsEnabled = true;
                    IntCheckBox.IsEnabled = true;
                    FrameDataServer.Stop();
                }
            }
            else
            {
                Alert("Oops，端口未打开；请打开端口后再试。");
            }
        }

        private void FindPeakButton_OnClick(object sender, RoutedEventArgs e)
        {
            peaking = true;
            PlotUpdate(LastLightFrameData);
        }

        private void ColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            color = true;
            PlotUpdate(LastLightFrameData);
        }

        public void ResetAxes(IPlotControl plotControl)
        {
            if (LastLightFrameData != null)
            {
                plotControl.Plot.Axes.SetLimits(LastLightFrameData.WaveLength.Min(), LastLightFrameData.WaveLength.Max(), 0, 4500);
                plotControl.Refresh();
                //PlotUpdate(LastLightFrameData);
            }
        }
    }
}
