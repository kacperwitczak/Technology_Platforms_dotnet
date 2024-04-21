using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_Regex.Extensions;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace WPF_Regex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string root_path = "";

        #region FileFunctions
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new System.Windows.Forms.FolderBrowserDialog()
            {
                Description = "Select directory to open"
            };

            var result = inputDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                statusBarTextBlock.Text = "Opening...";
                var directory = inputDialog.SelectedPath;
                root_path = directory;
                DisplayTreeView(directory);
                statusBarTextBlock.Text = "Ready";
            }
        }

        #endregion FileFunctions

        #region TreeLogic
        private void DisplayTreeView(string path)
        {
            treeView.Items.Clear();
            var root = BuildTreeView(null, path);
            treeView.Items.Add(root);
        }

        private TreeViewItem BuildTreeView(TreeViewItem? parent, string path)
        {
            TreeViewItem root = CreateDirectoryTreeViewItem(parent, path);
            foreach (var directory in Directory.GetDirectories(path))
            {
                var dir = BuildTreeView(root, directory);
                root.Items.Add(dir);
            }

            foreach (var file in Directory.GetFiles(path))
            {
                TreeViewItem item = CreateFileTreeViewItem(root, file);
                root.Items.Add(item);
            }

            return root;
        }
        #endregion

        #region CreateTreeView

        private TreeViewItem CreateDirectoryTreeViewItem(TreeViewItem? parent, string path)
        {
            FileInfo rootInfo = new FileInfo(path);
            var root = new TreeViewItem()
            {
                Header = rootInfo.Name,
                Tag = rootInfo.FullName
            };

            var dirMenu = CreateDirectoryContextMenu(parent, root, path);
            root.ContextMenu = dirMenu;

            return root;
        }

        private TreeViewItem CreateFileTreeViewItem(TreeViewItem? parent, string file)
        {
            FileInfo fileInfo = new FileInfo(file);
            var root = new TreeViewItem()
            {
                Header = fileInfo.Name,
                Tag = fileInfo.FullName
            };
            var fileMenu = CreateFileContextMenu(parent, root, file);
            root.ContextMenu = fileMenu;

            return root;
        }
        #endregion CreateTreeView

        #region CreateContextMenu

        private ContextMenu CreateDirectoryContextMenu(TreeViewItem? parent,TreeViewItem current, string path)
        {
            ContextMenu menu = new ContextMenu();

            menu.Items.Add(CreateCreateMenuItem(current, path));
            menu.Items.Add(CreateDeleteMenuItem(parent, current, path));

            return menu;
        }

        private ContextMenu CreateFileContextMenu(TreeViewItem? parent, TreeViewItem current, string path)
        {
            ContextMenu menu = new ContextMenu();

            menu.Items.Add(CreateOpenMenuItem(path));
            menu.Items.Add(CreateDeleteMenuItem(parent, current, path));

            return menu;
        }
        #endregion CreateContextMenu

        #region CreateMenuItem
        private MenuItem CreateCreateMenuItem(TreeViewItem current, string path)
        {
            MenuItem createItem = new MenuItem()
            {
                Header = "Create"
            };

            createItem.Click += (sender, e) =>
            {
                var inputDialog = new CreateFileForm();
                inputDialog.ShowDialog();

                if (inputDialog.DialogResult)
                {
                    string fileName = inputDialog.FileName;
                    if (IsNameValid(fileName))
                    {
                        string path_to_new_file = Path.Combine(path, fileName);
                        if (inputDialog.IsFileSelected)
                        {
                            File.Create(path_to_new_file);
                            current.Items.Add(CreateFileTreeViewItem(current, path_to_new_file));
                        }
                        else if (inputDialog.IsDirectorySelected)
                        {
                            Directory.CreateDirectory(path_to_new_file);
                            current.Items.Add(CreateDirectoryTreeViewItem(current, path_to_new_file));
                        }

                        FileInfo fileInfo = new FileInfo(path_to_new_file);
                        fileInfo.Attributes = inputDialog.IsReadOnlyChecked ? (fileInfo.Attributes | FileAttributes.ReadOnly) : fileInfo.Attributes;
                        fileInfo.Attributes = inputDialog.IsReadOnlyChecked ? (fileInfo.Attributes | FileAttributes.Hidden) : fileInfo.Attributes;
                        fileInfo.Attributes = inputDialog.IsReadOnlyChecked ? (fileInfo.Attributes | FileAttributes.Archive) : fileInfo.Attributes;
                        fileInfo.Attributes = inputDialog.IsReadOnlyChecked ? (fileInfo.Attributes | FileAttributes.System) : fileInfo.Attributes;

                    }
                    else
                    {
                        MessageBox.Show("File name should contain up to 8 characters and .txt .php or .html extension!");
                    }
                }
            };
            return createItem;
        }

        private MenuItem CreateOpenMenuItem(string path)
        {
            MenuItem openItem = new MenuItem()
            {
                Header = "Open",
                Tag = path
            };

            openItem.Click += (sender, e) =>
            {
                string fileBody = File.ReadAllText(path);
                fileBodyTextBlock.Text = fileBody;
            };
            return openItem;
        }

        private MenuItem CreateDeleteMenuItem(TreeViewItem? parent, TreeViewItem current, string path)
        {
            MenuItem deleteItem = new MenuItem()
            {
                Header = "Delete",
                Tag = path
            };

            deleteItem.Click += (semder, e ) => 
            {
                FileAttributes attr = File.GetAttributes(path);
                attr = attr & ~FileAttributes.ReadOnly;
                File.SetAttributes(path, attr);

                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    Directory.Delete(path, true);
                }
                else
                {
                    File.Delete(path);
                }

                if (parent != null)
                {
                    parent.Items.Remove(current);
                }
                else
                {
                    this.root_path = "";
                    treeView.Items.Clear();
                }
            };

            return deleteItem;
        }
        #endregion CreateMenuItem

        private void TreeView_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeViewItem = treeView.SelectedItem as TreeViewItem;
            if (treeViewItem != null)
            {
                string path = treeViewItem.Tag.ToString();
                FileInfo fileInfo = new FileInfo(path);
                string attr = fileInfo.GetAttributesDOS();

                statusBarTextBlock.Text = attr;
            }
        }

        private bool IsNameValid(string filename)
        {
            string pattern = @"^[\w-~]{1,8}(\.(txt|php|html))?$";
            return Regex.IsMatch(filename, pattern, RegexOptions.IgnoreCase);
        }
    }
}
