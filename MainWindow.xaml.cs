using SpectrographWPF.FrameData;
using SpectrographWPF.Manager;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualBasic;

namespace SpectrographWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            FrameDataConsumer.DPlotUpdate = new FrameDataConsumer.PlotUpdateDelegate(PlotUpdate);

            plot.Plot.Axes.SetLimits(400, 700, 0, 4000);
            //plot.Plot.XLabel("Pixel");
            //plot.Plot.YLabel("Amplitude");
            plot.Plot.Title("Spectrograph");

            var portList = SerialPortManager.FindPort();
            if (portList.Length > 0)
            {
                portsComboBox.Items.Clear();
                portsComboBox.ItemsSource = portList;
                portsComboBox.SelectedIndex = 0;
                portsComboBox.IsEnabled = true;
                openClosePortButton.IsEnabled = true;
                Information($"查找到可以使用的端口{portsComboBox.Items.Count}个。");
            }
            else
            {
                Alert("Oops，没有查找到可用端口；您可以点击“查找”按钮手动查找。");
                openClosePortButton.IsEnabled = false;
            }
        }

        public SerialPortManager serialPortManager = SerialPortManager.Instance;

        public double deltax = 0;


        private void Information(string message)
        {
            if (serialPortManager.IsOpen())
            {
                // #FFCA5100
                statusBar.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xCA, 0x51, 0x00));
            }
            else
            {
                // #FF007ACC
                statusBar.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0x00, 0x7A, 0xCC));
            }

            string time = "[" + DateAndTime.Now.ToLongTimeString() + "] ";
            statusInfoTextBlock.Text = time + message;
        }

        private void Alert(string message)
        {
            // #FF68217A
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                statusBar.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0xFF, 0x21, 0x2A));
                string time = "[" + DateAndTime.Now.ToLongTimeString() + "] ";
                statusInfoTextBlock.Text = time + message;
            }));
        }
    }
}