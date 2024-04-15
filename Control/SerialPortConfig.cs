using SpectrographWPF.SerialPortControl;
using SpectrographWPF.Utils;
using System.Windows;

namespace SpectrographWPF
{
    public partial class MainWindow : Window
    {
        public void FindPortButton_Click(object sender, RoutedEventArgs e)
        {
            string[] portList = serialPortManager.FindPort();
            if (portList != null)
            {
                portsComboBox.Items.Clear();
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
            try
            {
                serialPortManager.OpenPort(portsComboBox.Text);
                if (serialPortManager.IsOpen())
                {
                    openClosePortButton.Content = "关闭端口";
                    Information("端口已打开。");
                }
            }
            catch
            {
                Alert("Oops，打开端口失败；请检查端口是否被占用。");
                return;
            }
        }

        public void SendDataButton_Click(object sender, RoutedEventArgs e)
        {
            serialPortManager.WriteData(sendDataTextBox.Text);
            string rewData = serialPortManager.ReadData() + " ";
            string data = Conversion.ToSpecifiedText(rewData, Conversion.ConversionType.Hex, System.Text.Encoding.UTF8);
            recvDataRichTextBox.AppendText(data);
            Information("数据已发送。");
        }
    }
}
