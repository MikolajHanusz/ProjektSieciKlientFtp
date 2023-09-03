using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;

namespace KlientFTP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        FolderBrowserDialog dialog = new FolderBrowserDialog();
        Klient kl = new Klient();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtLocalPath.Text = dialog.SelectedPath;
        }

        private void GetFtpContent(ArrayList dirs)
        {
            lbFtpDir.Items.Clear();
            lbFtpDir.Items.Add("[..]");
            dirs.Sort();
            foreach (string dir in dirs) 
            {
                string pos=dir.Substring(dir.LastIndexOf(' ') + 1,dir.Length - dir.LastIndexOf(' ')-1);
                if(pos != ".." && pos != ".")
                    lbFtpDir.Items.Add(pos);
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if(txtSerwer.Text !=String.Empty)
                try
                {
                    string serwerName= txtSerwer.Text;
                    kl = new Klient(serwerName, txtLogin.Text, txtPassword.Password);
                    GetFtpContent(kl.GetDirs());
                    txtFtpPath.Text = kl.FtpDir;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Błąd");
                }
            else
            {
                System.Windows.MessageBox.Show("Server Name");
                txtSerwer.Text = string.Empty;
            }
        }

        private void lbFtpDir_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int idx = lbFtpDir.SelectedIndex;
            try
            {
                if (idx > -1)
                {
                    if (idx == 0)
                    {
                        GetFtpContent(kl.ChangeDirUp());
                        txtFtpPath.Text = kl.FtpDir;
                    }
                    else
                    {
                        string dir = lbFtpDir.Items[idx].ToString();
                        if (dir.Contains('.'))
                        {
                            txtFtpPath.Text += "/" + dir;
                            return;
                        }
                        GetFtpContent(kl.ChangeDir(dir));
                        txtFtpPath.Text = kl.FtpDir;
                    }
                }
            }
            catch (Exception ex) 
            {
                System.Windows.MessageBox.Show("Błąd");
            }
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            kl.DownloadFile(txtFtpPath.Text, txtLocalPath.Text);
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            txtSerwer.Clear();
            txtFtpPath.Clear();
            txtLogin.Clear();
            txtPassword.Clear();
            lbFtpDir.Items.Clear();
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            try
            {
                ofd.ShowDialog();
                kl.UploadFile(txtFtpPath.Text, ofd.FileName);
            }
            catch (Exception ex) 
            {
                System.Windows.MessageBox.Show("Błąd");
            }
        }
    }
}
