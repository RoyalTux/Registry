using Microsoft.Win32;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Registry
{
    public partial class MainWindow : Window
    {
        static RegistryKey classesRoot;
        static RegistryKey extensionRoot;
        static string curIcon;
        static string curProgramm;

        public MainWindow()
        {
            InitializeComponent();
            classesRoot = Microsoft.Win32.Registry.ClassesRoot;
            extensionRoot = classesRoot;
        }

        public string CurIcon
        {
            get => curIcon;
            set
            {
                curIcon = value;
                IconTextBox.Text = curIcon;
            }
        }

        public string CurProgramm
        {
            get => curProgramm;
            set
            {
                curProgramm = value;
                ProgrammTextBox.Text = curProgramm;
            }
        }

        private void ExtensionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Find(ExtensionTextBox.Text);
        }

        private void Find(string text)
        {
            try
            {
                if (!text.Contains("."))
                {
                    StringBuilder newText = new StringBuilder(text.Length + 1);
                    newText.Append(".");
                    newText.Append(text);
                    text = newText.ToString();
                }
                var curExtension = extensionRoot.OpenSubKey(text);
                if (curExtension != null)
                {
                    string defaultCurExtension = (string)curExtension.GetValue(curExtension.GetValueNames()[0]);
                    var curExtensionDefaultValue = extensionRoot.OpenSubKey(defaultCurExtension);

                    GetProgramm(curExtensionDefaultValue.OpenSubKey("shell"));

                    GetIcon(curExtensionDefaultValue.OpenSubKey("DefaultIcon"));
                }
                else
                {
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetProgramm(RegistryKey registryKey)
        {
            string action = (string)registryKey.GetValue(registryKey.Name);
            if (action != null)
            {
                var curProgrammKey = registryKey.OpenSubKey(action).OpenSubKey("command");
                CurProgramm = (string)curProgrammKey.GetValue(curProgrammKey.GetValueNames()[0]);
            }
            else
            {
                var curProgrammKey = registryKey.OpenSubKey("Open").OpenSubKey("command");
                CurProgramm = (string)curProgrammKey.GetValue(curProgrammKey.GetValueNames()[0]);
            }
        }

        private void GetIcon(RegistryKey registryKey)
        {
            CurIcon = (string)registryKey.GetValue(null);
        }
    }
}
