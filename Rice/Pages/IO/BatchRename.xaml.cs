using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Rice.Pages.IO
{
    /// <summary>
    /// Interaction logic for BatchRename.xaml
    /// </summary>
    public partial class BatchRename : UserControl
    {
        public BatchRename()
        {
            InitializeComponent();

            ObservableCollection<FileItem> list = GetList();
            DG1.DataContext = list;
        }

        private ObservableCollection<FileItem> GetList()
        {
            ObservableCollection<FileItem> list = new ObservableCollection<FileItem>();
            list.Add(new FileItem() { FirstName = "001", PreviewName = "002",Result = "success" });

            return list;
        }
    }

    class FileItem {
        public string FirstName { get; set; }
        public string PreviewName { get; set; }
        public string Result { get; set; }
        public string FullName { get; set; }
        public string Path { get; set; }

    }
}
