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
                DisplayTreeView(directory);
                statusBarTextBlock.Text = "Ready";
            }
        }

        private void DisplayTreeView(string path)
        {
            treeView.Items.Clear();
            var root = BuildTreeView(path);
            treeView.Items.Add(root);
        }

        private TreeViewItem BuildTreeView(string path)
        {
            FileInfo rootInfo = new FileInfo(path);
            var root = new TreeViewItem()
            {
                Header = rootInfo.Name,
                Tag = rootInfo.FullName
            };

            var dirMenu = CreateDirectoryContextMenu(path);
            root.ContextMenu = dirMenu;

            foreach (var directory in Directory.GetDirectories(path))
            {
                var dir = BuildTreeView(directory);
                
                root.Items.Add(dir);
            }

            foreach (var file in Directory.GetFiles(path))
            {
                FileInfo fileInfo = new FileInfo(file);
                var item = new TreeViewItem()
                {
                    Header = fileInfo.Name,
                    Tag = fileInfo.FullName
                };
                var fileMenu = CreateFileContextMenu(file);
                item.ContextMenu = fileMenu;
                root.Items.Add(item);
            }

            return root;
        }

        private ContextMenu CreateDirectoryContextMenu(string path)
        {
            ContextMenu menu = new ContextMenu();

            MenuItem createItem = new MenuItem()
            {
                Header = "Create"
            };

            createItem.Click += (sender, e) => CreateNewFile(path);

            menu.Items.Add(CreateDeleteMenuItem(path));
            menu.Items.Add(createItem);

            return menu;
        }

        private ContextMenu CreateFileContextMenu(string path)
        {
            ContextMenu menu = new ContextMenu();

            MenuItem openItem = new MenuItem()
            {
                Header = "Open",
                Tag = path
            };
            openItem.Click += DisplayFile;

            menu.Items.Add(CreateDeleteMenuItem(path));
            menu.Items.Add(openItem);

            return menu;
        }

        private MenuItem CreateDeleteMenuItem(string path)
        {
            MenuItem deleteItem = new MenuItem()
            {
                Header = "Delete",
                Tag = path
            };
            deleteItem.Click += Delete_Click;

            return deleteItem;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem? deleteItem = sender as MenuItem;
            if (deleteItem != null)
            {
                string path = deleteItem.Tag.ToString();
                FileAttributes attr = File.GetAttributes(path);
                if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    attr = attr & ~FileAttributes.ReadOnly;
                    File.SetAttributes(path, attr);
                }

                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    Directory.Delete(path, true);
                }
                else
                {
                    File.Delete(path);
                }

                TreeViewItem selected = treeView.SelectedItem as TreeViewItem;
                if (selected != null && selected.Parent is ItemsControl parent)
                {
                    parent.Items.Remove(selected);
                }
            }
        }

        private void DisplayFile(object sender, RoutedEventArgs e)
        {
            MenuItem openItem = sender as MenuItem;
            if (openItem != null)
            {
                string fileBody = File.ReadAllText(openItem.Tag.ToString());

                fileBodyTextBlock.Text = fileBody;
            }
        }

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

        private void CreateNewFile(string directory)
        {
            var inputDialog = new CreateFileForm();
            inputDialog.ShowDialog();

            if (inputDialog.DialogResult)
            {
                string fileName = inputDialog.FileName;
                if (IsNameValid(fileName))
                {
                    string path = Path.Combine(directory, fileName);
                    if (inputDialog.IsFileSelected)
                    {
                        File.Create(path);
                    } else if(inputDialog.IsDirectorySelected)
                    {
                        Directory.CreateDirectory(path);
                    }

                    FileInfo fileInfo = new FileInfo(path);
                    fileInfo.Attributes = inputDialog.IsReadOnlyChecked ? (fileInfo.Attributes | FileAttributes.ReadOnly) : fileInfo.Attributes;
                    fileInfo.Attributes = inputDialog.IsReadOnlyChecked ? (fileInfo.Attributes | FileAttributes.Hidden) : fileInfo.Attributes;
                    fileInfo.Attributes = inputDialog.IsReadOnlyChecked ? (fileInfo.Attributes | FileAttributes.Archive) : fileInfo.Attributes;
                    fileInfo.Attributes = inputDialog.IsReadOnlyChecked ? (fileInfo.Attributes | FileAttributes.System) : fileInfo.Attributes;
                    DisplayTreeView(directory);
                }
                else
                {
                    MessageBox.Show("File name should contain up to 8 characters and .txt .php or .html extension!");
                }
            }

        }

        private bool IsNameValid(string filename)
        {
            string pattern = @"^[\w-~]{1,8}(\.(txt|php|html))?$";
            return Regex.IsMatch(filename, pattern, RegexOptions.IgnoreCase);
        }
    }
}
