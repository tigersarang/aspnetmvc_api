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
using System.Xml.Linq;

namespace HtmlTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm = new MainViewModel();
        private int curProductId = 0;

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
                        
            await LoginAsync(username, password);

            await ProductAsync();
        }

        private async Task ProductAsync()
        {
            await CreateProductAsync();

            await UpdateProductAsync();

            await ReplyAsync();

            await DeleteProductAsync();

            await PaginationAsync();

            await webView.CoreWebView2.ExecuteScriptAsync($@"
                alert('All inspections are completed.');
            ");
        }

        private async Task PaginationAsync()
        {

            webView.CoreWebView2.Navigate("https://localhost:7188/Product");
            await Task.Delay(1000);

            await webView.CoreWebView2.ExecuteScriptAsync($@"
                document.getElementById('btn-2').click();
            ");

            await Task.Delay(1000);
        }

        private async Task DeleteProductAsync()
        {
            await webView.CoreWebView2.ExecuteScriptAsync($@"
                document.getElementById('btnDelete').click();
            ");

            await Task.Delay(1000);

        }

        private async Task ReplyAsync()
        {

            await webView.CoreWebView2.ExecuteScriptAsync($@"
                document.getElementById('content').value = 'reply1';
                document.getElementById('btnReply').click();
            ");

            await Task.Delay(1000);

            await webView.CoreWebView2.ExecuteScriptAsync($@"
                document.getElementById('content').value = 'reply2';
                document.getElementById('btnReply').click();
            ");

            await Task.Delay(1000);
        }

        private async Task UpdateProductAsync()
        {

            await webView.CoreWebView2.ExecuteScriptAsync($@"
                document.getElementById('btnUpdate').click();
            ");

            await Task.Delay(1000);

            await webView.CoreWebView2.ExecuteScriptAsync($@"
                document.getElementById('Name').value = 'test2';
                document.getElementById('Price').value = '1002';
                document.getElementById('Content').value = '1002';
                document.getElementById('btnSubmit').click();
            ");

            await Task.Delay(1000);

            await webView.CoreWebView2.ExecuteScriptAsync($@"
                document.getElementById('Name').value = 'test2';
                document.getElementById('Price').value = '1002';
                document.getElementById('Content').value = '1002';
                document.getElementById('btnSubmit').click();
            ");

            await Task.Delay(1000);
        }

        private async Task CreateProductAsync()
        {

            try
            {
                webView.CoreWebView2.Navigate("https://localhost:7188/Product");
                await Task.Delay(1000);

                await webView.CoreWebView2.ExecuteScriptAsync($@"
                document.getElementById('create').click();
            ");

                await Task.Delay(2000);

                await webView.CoreWebView2.ExecuteScriptAsync(@"
                    document.getElementById('Name').value = 'test1';
                    document.getElementById('Price').value = '1001';
                    document.getElementById('Content').value = '1001';
                    document.getElementById('btnSubmit').click();
                ");


                // 기존 서버에 있는 파일 추가.
                //    await webView.CoreWebView2.ExecuteScriptAsync(@"

                //    var mockFile = {name: 'README.md', size:1111 };      
                //    myDropzone.displayExistingFile(mockFile);
                //    document.getElementById('Name').value = 'test1';
                //    document.getElementById('Price').value = '1001';
                //    document.getElementById('Content').value = '1001';
                //    // document.getElementById('btnSubmit').click();
                //");

                await Task.Delay(2000);

                curProductId = int.Parse(webView.Source.ToString().Split("/").Last());
            } catch(Exception ex)
            {

            }
        }

        private async Task LoginAsync(string username, string password)
        {

            await webView.CoreWebView2.ExecuteScriptAsync($@"
                document.getElementById('userName').value = '{username}';
                document.getElementById('password').value = '{password}';
                document.getElementById('btnLogin').click();
            ");

            await Task.Delay(2000);
        }
    }
}
