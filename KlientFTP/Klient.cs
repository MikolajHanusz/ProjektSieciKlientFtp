using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace KlientFTP
{
    internal class Klient
    {
        public string Host     { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        private string ftpDir;
        public string FtpDir
        {
            get 
            {
                if (ftpDir.StartsWith("ftp://"))
                    return ftpDir;
                else
                    return "ftp://" + ftpDir;
            }
            set 
            { 
                ftpDir = value; 
            }
        }

        public Klient() { }
        public Klient(string host, string user, string pswd) 
        {
            Host = host;
            UserName = user;
            Password = pswd;
            FtpDir = host;
        }

        public ArrayList GetDirs() 
        {
            ArrayList dirs = new ArrayList();
            FtpWebRequest req;
            try
            {
                req = (FtpWebRequest)WebRequest.Create(FtpDir);
                req.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                req.Credentials = new NetworkCredential(this.UserName, this.Password);
                req.KeepAlive = false;
                using (FtpWebResponse res = (FtpWebResponse)req.GetResponse()) 
                {
                    Stream str = res.GetResponseStream();
                    using (StreamReader sr = new StreamReader(str)) 
                    {
                        string directory;
                        while((directory = sr.ReadLine()) != null) 
                        {
                            dirs.Add(directory);
                        }
                    }
                }
                return dirs;
            }
            catch (Exception e) 
            {
                throw new Exception("Connection error: "+ Host, e);
            }
        }

        public ArrayList ChangeDir(string dir)
        {
            ftpDir += "/" + dir;
            return GetDirs();
        }
        public ArrayList ChangeDirUp()
        {
            if(ftpDir != "ftp://"+Host)
            {
                ftpDir = ftpDir.Remove(ftpDir.LastIndexOf("/"), ftpDir.Length - ftpDir.LastIndexOf("/"));
                return GetDirs();
            }
            else 
                return GetDirs();
        }

        public void DownloadFile(string ftpName, string localName)
        {
            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential(this.UserName, this.Password);
                byte[] fileData = request.DownloadData(ftpName);
                string name = ftpName.Substring(ftpName.LastIndexOf("/"));              
                using (FileStream file = System.IO.File.Create(localName + name))
                {
                    file.Write(fileData, 0, fileData.Length);
                    file.Close();
                    System.Windows.MessageBox.Show("File downloaded");
                }
            }
        }

        public async void UploadFile(string ftpName, string localName)
        {

            string name = ftpName + "/" + Path.GetFileName(localName);
            var builder = new StringBuilder();
            int count = 0;
            int i = 0;
            foreach (var c in name)
            {
                builder.Append(c);
                if (c == '/') i++;
                if(i == 3)
                {
                    i = 0;
                    builder.Append('/');
                }
            }
            name = builder.ToString();    

            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(name);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(this.UserName, this.Password);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = true;

            FileStream stream = System.IO.File.OpenRead(localName);
            byte[] buffer = new byte[stream.Length];

            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            Stream reqStream = request.GetRequestStream();
            reqStream.Write(buffer, 0, buffer.Length);
            reqStream.Close();
            System.Windows.MessageBox.Show("File Uploaded");
        }
    }
}
