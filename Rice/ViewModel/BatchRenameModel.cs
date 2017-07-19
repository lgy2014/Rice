using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.IO;

namespace Rice.ViewModel
{
    public class BatchRenameModel : ViewModelBase
    {
        private string[] filetypes;
        public BatchRenameModel()
        {
            GetDir = new RelayCommand(GetDirCommand);
            SearchFile = new RelayCommand(SearchCommand);

            _DGFiles = new ObservableCollection<FileItem>();

            _FileTypes = new ObservableCollection<ComboxEntry>();
            _FileTypes.Add(new ComboxEntry() { Entry = "*.*" });
            ComboxEntry ce = new ComboxEntry() { Entry = "*.sln;*.csproj;*.config;*.aspx;*.cs; *.txt;*.cshtml;*.html;*.asax;*.ashx;*.js;*.css;" };
            _FileTypes.Add(ce);
            _FileTypes.Add(new ComboxEntry() { Entry = "*.ini;*.reg;*.bat;*.cmd;" });
            _FileTypes.Add(new ComboxEntry() { Entry = "*.htm;*.html;*.xml;" });
            FileTypesEntry = ce;

            _ExcludeSubDirs = new ObservableCollection<ComboxEntry>();
            ComboxEntry direntry = new ComboxEntry() { Entry = "bin;obj;.vs;.git;packages;" };
            _ExcludeSubDirs.Add(direntry);
            _DirEntry = direntry;
        }

        #region 属性

        private string _textFrom;
        public const string TexFromPropertyName = "TextFrom";
        public string TextFrom
        {
            get
            {
                return _textFrom;
            }
            set
            {
                _textFrom = value;
                RaisePropertyChanged(TexFromPropertyName);
            }
        }

        private string _textTo;
        public const string TextToPropertyName = "TextTo";
        public string TextTo
        {
            get
            {
                return _textTo;
            }
            set
            {
                _textTo = value;
                RaisePropertyChanged(TextToPropertyName);
            }
        }

        private string _textDir;
        public const string TextDirPropertyName = "TextDir";
        public string TextDir
        {
            get
            {
                return _textDir;
            }
            set
            {
                _textDir = value;
                RaisePropertyChanged(TextDirPropertyName);
            }
        }

        private bool _IsChecked = false;
        public const string IsCheckedPropertyName = "IsChecked";
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                _IsChecked = value;
                RaisePropertyChanged(IsCheckedPropertyName);
            }
        }

        private ObservableCollection<ComboxEntry> _FileTypes;
        public const string FileTypesPropertyName = "FileTypes";
        public ObservableCollection<ComboxEntry> FileTypes
        {
            get
            {
                return _FileTypes;
            }
            set
            {
                _FileTypes = value;
                RaisePropertyChanged(FileTypesPropertyName);
            }
        }

        private ComboxEntry _FileTypesEntry;
        public const string FileTypesEntryPropertyName = "FileTypesEntry";
        public ComboxEntry FileTypesEntry
        {
            get
            {
                return _FileTypesEntry;
            }
            set
            {
                _FileTypesEntry = value;
                RaisePropertyChanged(FileTypesEntryPropertyName);
                filetypes = _FileTypesEntry.Entry.Split(';');
            }
        }

        private ObservableCollection<ComboxEntry> _ExcludeSubDirs;
        public const string ExcludeSubDirsPropertyName = "ExcludeSubDirs";
        public ObservableCollection<ComboxEntry> ExcludeSubDirs
        {
            get
            {
                return _ExcludeSubDirs;
            }
            set
            {
                _ExcludeSubDirs = value;
                RaisePropertyChanged(ExcludeSubDirsPropertyName);
            }
        }

        private ComboxEntry _DirEntry;
        public const string DirEntryPropertyName = "DirEntry";
        public ComboxEntry DirEntry
        {
            get
            {
                return _DirEntry;
            }
            set
            {
                _DirEntry = value;
                RaisePropertyChanged(DirEntryPropertyName);
            }
        }

        private ObservableCollection<FileItem> _DGFiles;
        public const string DGFilesPropertyName = "DGFiles";
        public ObservableCollection<FileItem> DGFiles
        {
            get
            {
                return _DGFiles;
            }
            set
            {
                _DGFiles = value;
                RaisePropertyChanged(TextDirPropertyName);
            }
        }
        #endregion


        #region 方法
        public ICommand GetDir
        {
            get; private set;
        }

        public ICommand SearchFile
        {
            get; private set;
        }

        private void GetDirCommand()
        {
            MessengerInstance.Send<string>(null, "OpenDialog");
            //Messenger.Default.Send<string>(null, "OpenDialog");
        }

        private void SearchCommand()
        {
            if (string.IsNullOrEmpty(TextDir))
            {
                return;
            }

            if (_FileTypesEntry==null)
            {
                return;
            }

            if (_DirEntry==null)
            {
                return;
            }

            try
            {
                GetFiles(TextDir);
            }
            catch (System.Exception ex)
            {
                MessengerInstance.Send<string>(ex.ToString(), "AlertMsg");
            }
        }

        private void GetFiles(string textDir)
        {
            DirectoryInfo di = new DirectoryInfo(textDir);
            if (di.Name.Contains(DirEntry.Entry))
            {
                return;
            }

            foreach (string str in filetypes)
            {
                FileInfo[] files = di.GetFiles(str);
                foreach (FileInfo f in files)
                {
                    _DGFiles.Add(new FileItem() { FileInfomation=f,FirstName=f.Name,PreviewName=f.Name});
                }
            }

            if (!IsChecked) return;

            string[] subDirs = Directory.GetDirectories(textDir);
            foreach (string dir in subDirs)
            {
                GetFiles(dir);
            }
            
        }

        private void ChangeFilesCommand()
        {
            if (DGFiles == null || DGFiles.Count == 0)
            {
                return;
            }
            //
            if (string.IsNullOrEmpty(TextFrom))
            {
                return;
            }
            foreach (FileItem item in DGFiles)
            {
                item.PreviewName = item.FileInfomation.Name.Split('.')[0].Replace(TextFrom, TextTo) + item.FileInfomation.Extension;
            }

        }

        #endregion
    }

    public class FileItem
    {
        public string FirstName { get; set; }
        public string PreviewName { get; set; }
        public string Result { get; set; }
        public string FullName { get; set; }
        public string Path { get; set; }

        public FileInfo FileInfomation { get; set; }
    }

    public class ComboxEntry
    {
        public string Entry { get; set; }
    }
}
