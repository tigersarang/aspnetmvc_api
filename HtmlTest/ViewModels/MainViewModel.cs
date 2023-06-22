using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HtmlTest.ViewModels
{

    public class MainViewModel : BaseViewModel
    {
        public event Action<string, string> TestRequested = delegate { };
        public MainViewModel()
        {
            TestCommand = new RelayCommand(Test, canTest);
        }

        private bool canTest(object? obj)
        {
            return true;
        }

        private void Test(object? obj)
        {
            TestRequested.Invoke("fff", "11111111");
        }

        public ICommand TestCommand { get; set; }

    }
}
