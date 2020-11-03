﻿using System;
using System.ServiceProcess;
using System.IO;
using System.Threading;

namespace FileWatcherService
{
    public partial class Service1 : ServiceBase
    {
        Logger logger;

        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            if (args == null) logger = new Logger();
            else logger = new Logger(args[0], args[1], args[2]);
            Thread loggerThread = new Thread(new ThreadStart(logger.Start));
            loggerThread.Start();
        }

        protected override void OnStop()
        {
            logger.Stop();
            Thread.Sleep(1000);
        }

        public void OnDebug()
        {
            OnStart(null);
        }
    }

    class Logger
    {
        string sourceDirectory;
        string archiveDirectory;
        string targetDirectory;

        ArchiveFile archive;
        EncryptFile encrypt;

        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;
        public Logger(string sourceDirectory = "C:\\source", string archiveDirectory = "C:\\archive", string targetDirectory = "C:\\target")
        {
            this.sourceDirectory = sourceDirectory;
            this.archiveDirectory = archiveDirectory;
            this.targetDirectory = targetDirectory;
            watcher = new FileSystemWatcher(this.sourceDirectory);
            watcher.Created += Watcher_Created;
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string filePath = e.FullPath;
            string result = ArchiveFile(filePath);
            if (result != null)
            {
                TargetFile(result);
            }
        }

        private string ArchiveFile(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            string path = archiveDirectory + "\\" +  file.Name;
            try
            {
                if (!File.Exists(path))
                {
                    var fl = File.Create(path);
                    fl.Close();
                    file.CopyTo(path, true);
                }
                encrypt = new EncryptFile(path);
                encrypt.Encrypt();
                archive = new ArchiveFile(path);
                archive.Compress();
                return archive.CompressedFileName;
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(sourceDirectory, true))
                {
                    writer.WriteLine(String.Format("{0} {1}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), ex.Message));
                    writer.Flush();
                }
                return null;
            }

        }

        private void TargetFile(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            string path = targetDirectory +  "\\" + file.Name;
            try
            {
                if (!File.Exists(path))
                {
                    var fl = File.Create(path);
                    fl.Close();
                    file.CopyTo(path, true);
                }
                archive = new ArchiveFile(path);
                archive.Decompress();
                encrypt = new EncryptFile(archive.DecompressedFileName);
                encrypt.Decrypt();
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(archiveDirectory, true))
                {
                    writer.WriteLine(String.Format("{0} {1}",
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), ex.Message));
                    writer.Flush();
                }
                return;
            }
        }


    }
}