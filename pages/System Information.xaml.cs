using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Flarebook.pages
{
    /// <summary>
    /// System_Information.xaml 的交互逻辑
    /// </summary>
    public partial class System_Information : Page
    {
        public ObservableCollection<HardwareInfo> HardwareInfos { get; set; }
        public System_Information()
        {
            InitializeComponent();
            HardwareInfos = new ObservableCollection<HardwareInfo>();
            HardwareListView.ItemsSource = HardwareInfos;
            LoadHardwareInfo();
        }

        private void LoadHardwareInfo()
        {
            GetHardwareInfo("Win32_Processor", "Name", "处理器名称");
            GetHardwareInfo("Win32_Processor", "NumberOfCores", "处理器核心数");
            GetHardwareInfo("Win32_Processor", "MaxClockSpeed", "处理器最大频率 (MHz)");
            GetHardwareInfo("Win32_PhysicalMemory", "Capacity", "内存容量 (Bytes)");
            GetHardwareInfo("Win32_PhysicalMemory", "Speed", "内存频率 (MHz)");
            GetHardwareInfo("Win32_BaseBoard", "Manufacturer", "主板制造商");
            GetHardwareInfo("Win32_BaseBoard", "Product", "主板型号");
            GetHardwareInfo("Win32_BaseBoard", "SerialNumber", "主板序列号");
            GetHardwareInfo("Win32_DiskDrive", "Model", "磁盘驱动器型号");
            GetHardwareInfo("Win32_DiskDrive", "Size", "磁盘大小 (Bytes)");
            GetHardwareInfo("Win32_VideoController", "Name", "显卡名称");
            GetHardwareInfo("Win32_VideoController", "AdapterRAM", "显存大小 (Bytes)");
            GetHardwareInfo("Win32_NetworkAdapter", "Name", "网络适配器名称");
            GetHardwareInfo("Win32_NetworkAdapter", "MACAddress", "MAC 地址");
        }

        private void GetHardwareInfo(string query, string property, string component)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT * FROM {query}");
            foreach (ManagementObject obj in searcher.Get())
            {
                HardwareInfos.Add(new HardwareInfo
                {
                    Component = component,
                    Details = obj[property]?.ToString()
                });
            }
        }

        private void ExportToTxt_Click(object sender, RoutedEventArgs e)
        {
            // 创建 SaveFileDialog 对象
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                // 设置默认文件名
                FileName = "HardwareInfo",
                // 设置默认保存路径，可以是用户的文档文件夹或其他位置
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                // 设置文件类型过滤器
                Filter = "Text Files (*.txt)|*.txt"
            };

            // 显示保存文件对话框
            if (saveFileDialog.ShowDialog() == true)
            {
                // 获取用户选择的文件路径
                string filePath = saveFileDialog.FileName;

                // 导出硬件信息到文本文件
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var info in HardwareInfos)
                    {
                        writer.WriteLine($"{info.Component}: {info.Details}");
                    }
                }

                // 显示导出成功的提示信息
                MessageBox.Show("硬件信息已导出到文件", "导出成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public class HardwareInfo
    {
        public string Component { get; set; }
        public string Details { get; set; }
    }
}
