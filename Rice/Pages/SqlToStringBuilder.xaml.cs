using System.Windows.Controls;
using Rice.ViewModel;

namespace Rice.Pages
{
    /// <summary>
    /// SqlToStringBuilder.xaml 的交互逻辑
    /// </summary>
    public partial class SqlToStringBuilder : Page
    {
        public SqlToStringBuilderViewModel SqlToStringBuilderViewModel { get; set; }

        public SqlToStringBuilder()
        {
            InitializeComponent();
            ViewModelLocator locator = FindResource("Locator") as ViewModelLocator;
            if (null !=locator)
            {
                SqlToStringBuilderViewModel = locator.SqlToStringBuilderViewModel;
            }
            DataContext = SqlToStringBuilderViewModel;
        }
    }
}
