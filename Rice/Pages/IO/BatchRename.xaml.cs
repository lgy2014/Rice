using GalaSoft.MvvmLight.Messaging;
using Rice.ViewModel;
using System.Windows.Controls;
using System;
using System.Windows;

namespace Rice.Pages.IO
{
    /// <summary>
    /// Interaction logic for BatchRename.xaml
    /// </summary>
    public partial class BatchRename : UserControl
    {
        public BatchRenameModel BatchRenameModel { get; set; }
        public BatchRename()
        {
            InitializeComponent();

            ViewModelLocator locator = FindResource("Locator") as ViewModelLocator;
            if (null != locator)
            {
                BatchRenameModel = locator.BatchRenameModel;
            }
            DataContext = BatchRenameModel;

            Messenger.Default.Register<string>(this, "OpenDialog", OpenDialog);
            Messenger.Default.Register<string>(this, "AlertMsg", AlertMsg);
            Messenger.Default.Register<string>(this, "Cancel", CloseDialog);
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }

        private void CloseDialog(string obj)
        {
            Application.Current.Shutdown();
        }

        private void OpenDialog(string a)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowNewFolderButton = false;
            System.Windows.Forms.DialogResult ret = fbd.ShowDialog();
            if (ret== System.Windows.Forms.DialogResult.OK)
            {
                txtDir.Text = fbd.SelectedPath;
            }
        }

        private void AlertMsg(string msg)
        {
            System.Windows.MessageBox.Show(msg);
        }
    }
    
}
