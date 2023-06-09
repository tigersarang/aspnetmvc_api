using HtmlTest.ViewModels;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HtmlTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            _vm.TestRequested += _vm_TestRequested;
            DataContext = _vm;
        }

        private async void _vm_TestRequested(string username, string password)
        {
            await webView.EnsureCoreWebView2Async();

            webView.CoreWebView2.Navigate("https://localhost:7188/Account/Login");

            await Task.Delay(1000);

            await webView.CoreWebView2.ExecuteScriptAsync($@"
                document.getElementById('userName').value = '{username}';
                document.getElementById('password').value = '{password}';
                document.getElementById('btnLogin').click();
            ");

            await Task.Delay(2000);

            await webView.CoreWebView2.ExecuteScriptAsync($@"
                document.getElementById('aProduct').click();
            ");

            await Task.Delay(1000);

            await webView.CoreWebView2.ExecuteScriptAsync($@"
                document.getElementById('product-1').click();
            ");            
        }
    }
}
