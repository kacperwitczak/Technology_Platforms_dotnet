using System.Windows;

namespace WPF_Regex
{
    public partial class CreateFileForm : Window
    {
        public CreateFileForm()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                System.Windows.MessageBox.Show("Name cant be blanc!");
                this.DialogResult = false;
            } else if (!IsDirectorySelected && !IsFileSelected)
            {
                System.Windows.MessageBox.Show("File type can't be empty!");
                this.DialogResult = false;
            } else
            {
                this.DialogResult = true;
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        public bool DialogResult { get; private set; }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private bool _isFileSelected;
        public bool IsFileSelected
        {
            get { return _isFileSelected; }
            set
            {
                _isFileSelected = value;
                RadioFile.IsChecked = value;
                RadioDirectory.IsChecked = !value;
            }
        }

        private bool _isDirectorySelected;
        public bool IsDirectorySelected
        {
            get { return _isDirectorySelected; }
            set
            {
                _isDirectorySelected = value;
                RadioDirectory.IsChecked = value;
                RadioFile.IsChecked = !value;
            }
        }

        private bool _isReadOnlyChecked;
        public bool IsReadOnlyChecked
        {
            get { return _isReadOnlyChecked; }
            set { _isReadOnlyChecked = value; }
        }

        private bool _isArchiveChecked;
        public bool IsArchiveChecked
        {
            get { return _isArchiveChecked; }
            set { _isArchiveChecked = value; }
        }

        private bool _isHiddenChecked;
        public bool IsHiddenChecked
        {
            get { return _isHiddenChecked; }
            set { _isHiddenChecked = value; }
        }

        private bool _isSystemChecked;
        public bool IsSystemChecked
        {
            get { return _isSystemChecked; }
            set { _isSystemChecked = value; }
        }

        private void RadioFile_Checked(object sender, RoutedEventArgs e)
        {
            IsFileSelected = true;
        }

        private void RadioDirectory_Checked(object sender, RoutedEventArgs e)
        {
            IsDirectorySelected = true;
        }

        private void CheckboxReadOnly_Checked(object sender, RoutedEventArgs e)
        {
            IsReadOnlyChecked = true;
        }

        private void CheckboxArchive_Checked(object sender, RoutedEventArgs e)
        {
            IsArchiveChecked = true;
        }

        private void CheckboxHidden_Checked(object sender, RoutedEventArgs e)
        {
            IsHiddenChecked = true;
        }

        private void CheckboxSystem_Checked(object sender, RoutedEventArgs e)
        {
            IsSystemChecked = true;
        }
    }
}
