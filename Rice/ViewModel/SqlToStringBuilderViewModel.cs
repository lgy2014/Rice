using Abp.Dependency;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Rice.ViewModel
{
    public class SqlToStringBuilderViewModel : ViewModelBase
    {
        private string _textSource;
        public const string TextSourcePropertyName = "TextSource";
        public string TextSource
        {
            get { return _textSource; }
            set
            {
                _textSource = value;
                RaisePropertyChanged(TextSourcePropertyName);
            }
        }
        private string _textTarget;
        public const string TextTargetPropertyName = "TextTarget";
        public string TextTarget
        {
            get { return _textTarget; }
            set
            {
                _textTarget = value;
                RaisePropertyChanged(TextTargetPropertyName);
            }
        }
        private string _labMsg;
        public const string labMsgPropertyName = "labMsg";
        public string labMsg
        {
            get { return _labMsg; }
            set
            {
                _labMsg = value;
                RaisePropertyChanged(labMsgPropertyName);
            }
        }

        public SqlToStringBuilderViewModel()
        {
            Run = new RelayCommand(RunCommand);
            Copy = new RelayCommand(CopyCommand);
            Clear = new RelayCommand(ClearCommand);

        }


        public ICommand Run { get; private set; }
        public ICommand Copy { get; private set; }
        public ICommand Clear { get; private set; }

        private void RunCommand()
        {
            if (string.IsNullOrEmpty(TextSource))
            {
                return;
            }

            string[] lines = TextSource.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            StringBuilder sqlBuilder = new StringBuilder(TextSource.Length + 100);
            sqlBuilder.AppendLine("StringBuilder sqlBuilder = new StringBuilder(" + (TextSource.Length + 10) + ");");
            foreach (var item in lines)
            {
                sqlBuilder.AppendLine("sqlBuilder.AppendLine(\"" + item + "\");");
            }
            sqlBuilder.AppendLine("string ret = sqlBuilder.ToString();");
            string ret = sqlBuilder.ToString();
            TextTarget = ret;
        }

        private void CopyCommand()
        {
            if (string.IsNullOrEmpty(TextTarget))
            {
                return;
            }

            //copy
            Clipboard.SetDataObject(TextTarget);
            
            labMsg = "已复制到剪贴板";

            ThreadPool.QueueUserWorkItem(a =>
            {
                Thread.Sleep(5000);
                labMsg = string.Empty;
            });
        }

        private void ClearCommand()
        {
            TextSource = string.Empty;
            TextTarget = string.Empty;
        }
    }
}
