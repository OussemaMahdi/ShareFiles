using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OAuthProtocol;
using Dropbox.Api;

namespace MyClasses
{
    public static class ConnectionDropbox
    {
        private const string ConsumerKey = "k5qxf2c9galzyry";
        private const string ConsumerSecret = "ntvc94mbx1weo9m";

        private static OAuthToken GetAccessToken()
        {
            var oauth = new OAuth();

            var requestToken = oauth.GetRequestToken(new Uri(DropboxRestApi.BaseUri), ConsumerKey, ConsumerSecret);

            var authorizeUri = oauth.GetAuthorizeUri(new Uri(DropboxRestApi.AuthorizeBaseUri), requestToken);
            System.Diagnostics.Process.Start(authorizeUri.AbsoluteUri);
            System.Threading.Thread.Sleep(5000); // Leave some time for the authorization step to complete

            return oauth.GetAccessToken(new Uri(DropboxRestApi.BaseUri), ConsumerKey, ConsumerSecret, requestToken);
        }

        private static OAuthToken accessToken = new OAuthToken("p7qoklhln5aj0ua9", "8ixcpkxdpnz2qya");
        private static DropboxApi api = new DropboxApi(ConsumerKey, ConsumerSecret, accessToken);      

        public static DropboxApi getAPI
        {
            get
            {
                return api;
            }        
        }

        public static void DeposeFile(RecivedFiles f)
        {
            api.UploadFile("dropbox", f.getNom(),f.getPathOfSave());
        }

        public static void DownloadFile(RecivedFiles f,string dest)
        {
            FileSystemInfo file = api.DownloadFile("dropbox", f.getNom());
            file.Save(dest);
        }
        

        //public void copyDirectory(string DirectoryPath, string savePath)
        //{
        //    savePath = savePath +@"\"+ getNameFromPath(DirectoryPath);
        //    Directory.CreateDirectory(savePath);
        //    string[] Dirs = Directory.GetDirectories(DirectoryPath);
        //    foreach (string Dir in Dirs)
        //        copyDirectory(Dir, savePath);
        //    string[] Fils = Directory.GetFiles(DirectoryPath);
        //    foreach (string Fil in Fils)
        //    {
        //        string sourceFile = DirectoryPath + @"\" + getNameFromPath(Fil);
        //        string destFile = savePath + @"\" + getNameFromPath(Fil);
        //        System.IO.File.Copy(sourceFile, destFile, true);
        //    }
        //}

        //public void DeposeDirectory(string DirectoryPath, string savePath)
        //{
        //    if (savePath != "")
        //        savePath = savePath + "/" + getNameFromPath(DirectoryPath);
        //    else
        //        savePath = savePath + getNameFromPath(DirectoryPath);
        //    var accessToken = new OAuthToken("p7qoklhln5aj0ua9", "8ixcpkxdpnz2qya");
        //    var api = new Dropbox.Api.DropboxApi(ConsumerKey, ConsumerSecret, accessToken);
        //    var flod = api.CreateFolder("dropbox", savePath);
        //    string[] Dirs = Directory.GetDirectories(DirectoryPath);
        //    foreach (string Dir in Dirs)
        //        DeposeDirectory(Dir, savePath);
        //    string[] Fils = Directory.GetFiles(DirectoryPath);
        //    foreach (string Fil in Fils)
        //    {
        //        string sourceFile = DirectoryPath + @"\" + getNameFromPath(Fil);
        //        string destFile = savePath + "/" + getNameFromPath(Fil);
        //        var file = api.UploadFile("dropbox", destFile, sourceFile);
        //    }
        //}

    }
}
