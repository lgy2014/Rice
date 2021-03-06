﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Text;

namespace Rice.ViewModel
{
    public class BatchRenameModel : ViewModelBase
    {
        private const string tokenAlertMsg = "AlertMsg";
        private const string tokenOpenDialog = "OpenDialog";
        private const string tokenCancel = "Cancel";
        private string[] filetypes;
        public BatchRenameModel()
        {
            GetDir = new RelayCommand(GetDirCommand);
            SearchFile = new RelayCommand(SearchCommand);

            _DGFiles = new ObservableCollection<FileItem>();

            _FileTypes = new ObservableCollection<ComboxEntry>();
            _FileTypes.Add(new ComboxEntry() { Entry = "*.*" });
            ComboxEntry ce = new ComboxEntry() { Entry = "*.sln;*.csproj;*.config;*.aspx;*.cs; *.txt;*.cshtml;*.html;*.htm;*.asax;*.ashx;*.js;*.css;*.json;*.xml" };
            _FileTypes.Add(ce);
            _FileTypes.Add(new ComboxEntry() { Entry = "*.ini;*.reg;*.bat;*.cmd;" });
            _FileTypes.Add(new ComboxEntry() { Entry = "*.xml;" });
            FileTypesEntry = ce;

            _ExcludeSubDirs = new ObservableCollection<ComboxEntry>();
            ComboxEntry direntry = new ComboxEntry() { Entry = "bin;obj;.vs;.git;packages;" };
            _ExcludeSubDirs.Add(direntry);
            _DirEntry = direntry;

            TextChangedCommand = new RelayCommand<string>(ChangeFilesCommand);
            Run = new RelayCommand(RunCommand);
            ChangeFilesCommandFromCommand = new RelayCommand<string>(ChangeFilesCommandFrom);

            RunReplace = new RelayCommand(RunReplaceCommand);

            RunReplaceIsEnabled = true;
            //
            TextFrom = "AbpCompanyName.AbpProjectName";
            TextTo = "Lgy.LIS";
            TextDir = string.Empty;
    }

        #region 属性

        private string _textFrom;
        public const string TextFromPropertyName = "TextFrom";
        public string TextFrom
        {
            get
            {
                return _textFrom;
            }
            set
            {
                _textFrom = value;
                RaisePropertyChanged(TextFromPropertyName);
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

        private bool _RunReplaceIsEnabled;
        public const string RunReplaceIsEnabledPropertyName = "RunReplaceIsEnabled";
        public bool RunReplaceIsEnabled
        {
            get
            {
                return _RunReplaceIsEnabled;
            }
            set
            {
                _RunReplaceIsEnabled = value;
                RaisePropertyChanged(RunReplaceIsEnabledPropertyName);
            }
        }

        public ICommand TextChangedCommand { get; private set; }
        public ICommand ChangeFilesCommandFromCommand { get; private set; }

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
            MessengerInstance.Send<string>(null, tokenOpenDialog);
            //Messenger.Default.Send<string>(null, "OpenDialog");
        }

        private void SearchCommand()
        {
            if (string.IsNullOrEmpty(TextDir))
            {
                return;
            }

            if (_FileTypesEntry == null)
            {
                return;
            }

            if (_DirEntry == null)
            {
                return;
            }

            try
            {
                DGFiles.Clear();
                GetFiles(TextDir);
            }
            catch (System.Exception ex)
            {
                MessengerInstance.Send<string>(ex.ToString(), tokenAlertMsg);
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
                    _DGFiles.Add(new FileItem() { FileInfomation = f, FirstName = f.Name, PreviewName = f.Name,FullName=f.FullName,Path=f.DirectoryName });
                }
            }

            if (!IsChecked) return;

            string[] subDirs = Directory.GetDirectories(textDir);
            foreach (string dir in subDirs)
            {
                GetFiles(dir);
            }

        }

        private void ChangeFilesCommand(string targetText)
        {
            TextTo = targetText ?? string.Empty;
            if (DGFiles == null || DGFiles.Count == 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(TextFrom))
            {
                return;
            }

            foreach (FileItem item in DGFiles)
            {
                item.PreviewName = item.FirstName.Replace(TextFrom, targetText);
            }
        }

        private void ChangeFilesCommandFrom(string fromText)
        {
            TextFrom = fromText ?? string.Empty;
            if (DGFiles == null || DGFiles.Count == 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(fromText))
            {
                //foreach (FileItem item in DGFiles)
                //{
                //    item.PreviewName = item.FileInfomation.Name;
                //}
                return;
            }
            else
            {
                foreach (FileItem item in DGFiles)
                {
                    string _filename = item.FirstName.Replace(fromText, TextTo);
                    if (string.IsNullOrEmpty(_filename))
                    {
                        Messenger.Default.Send<string>("文件名不能为空。", tokenAlertMsg);
                        break;
                    }
                    item.PreviewName = _filename;
                }
            }
        }

        public ICommand Run { get; private set; }

        private void RunCommand()
        {
            if (string.IsNullOrEmpty(TextFrom))
            {
                return;
            }

            if (TextTo == TextFrom)
            {
                return;
            }

            foreach (FileItem f in DGFiles)
            {
                if (f.FirstName == f.PreviewName)
                {
                    continue;
                }
                f.FileInfomation.MoveTo(f.FileInfomation.FullName.Replace(f.FirstName, f.PreviewName));
                f.Result = "完成";
            }
        }

        public ICommand RunReplace
        {
            get; private set;
        }

        private void RunReplaceCommand()
        {
            RunReplaceIsEnabled = false;
            ThreadPool.QueueUserWorkItem(a=> {
                foreach (FileItem item in DGFiles)
                {
                    string content = File.ReadAllText(item.FileInfomation.FullName,Encoding.UTF8);
                    content = content.Replace(TextFrom, TextTo);
                    File.WriteAllText(item.FileInfomation.FullName, content,Encoding.UTF8);
                    item.Result = "完成";
                }

                RunReplaceIsEnabled = true;
            });
            

            //DGFiles = DGFiles;
        }

        public ICommand CancelCommand { get; private set; }

        public void Cancel()
        {
            Messenger.Default.Send<string>(null,tokenCancel);
        }
        #endregion
    }

    public class FileItem : INotifyPropertyChanged
    {
        public string FirstName { get; set; }

        private string _PreviewName;
        public string PreviewName
        {
            get { return _PreviewName; }
            set
            {
                _PreviewName = value;
                OnPropertyChanged("PreviewName");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _Result;
        public string Result
        {
            get { return _Result; }
            set
            {
                _Result = value;
                OnPropertyChanged("Result");
            }
        }
        public string FullName { get; set; }
        public string Path { get; set; }

        public FileInfo FileInfomation { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ComboxEntry
    {
        public string Entry { get; set; }
    }
}
