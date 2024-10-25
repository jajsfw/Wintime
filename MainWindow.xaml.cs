using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Flarebook
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // 处理菜单项点击事件
            var menuItem = sender as MenuItem;
            MessageBox.Show($"Clicked on {menuItem.Header}");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 选中第一个ListViewItem并加载对应的页面
            var listViewItem = s.Items[0] as ListViewItem;
            if (listViewItem != null)
            {
                string pageName = listViewItem.Tag.ToString();
                NavigateToPage(pageName);
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && s.SelectedItem is ListViewItem selectedItem)
            {
                string pageName = selectedItem.Tag.ToString();
                NavigateToPage(pageName);
            }
        }

        private void NavigateToPage(string pageName)
        {
            if (!string.IsNullOrEmpty(pageName))
            {
                try
                {
                    contentFrame.Navigate(new Uri(pageName, UriKind.Relative));
                }
                catch (Exception ex)
                {
                    // 处理异常，例如记录日志
                    System.Diagnostics.Debug.WriteLine("Error navigating to page: " + ex.Message);
                }
            }
        }

        // NavigationFailed 事件的处理程序
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            // 你可以在这里处理导航失败的情况，例如记录日志或显示自定义消息
            System.Diagnostics.Debug.WriteLine("Navigation failed: " + e.Uri.ToString());

            // 吞掉异常，不显示默认的错误消息
            e.Handled = true;
        }
    }
}
