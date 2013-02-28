using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;


namespace Tuple.Infra.Log
{
    /// <summary>
    /// This is an advanced useage, where you want to intercept the logging messages and devert them somewhere
    /// besides ETW.
    /// </summary>
    public sealed class StorageFileEventListener : EventListener
    {
        /// <summary>
        /// Storage file to be used to write logs
        /// </summary>
        private StorageFile m_StorageFile = null;

        /// <summary>
        /// Name of the current event listener
        /// </summary>
        private string m_Name;

        /// <summary>
        /// The format to be used by logging.
        /// </summary>
        /// 
        private string m_FormatHtml = "<table><tr><td width=\"200\"><font face=Arial size=3 color={3}>{0:yyyy-MM-dd HH\\:mm\\:ss\\:ffff}</td><td width=\"100\"><font face=Arial size=3 color={3}>{1}</td><td width=\"1000\"><font face=Arial size=3 color={3}>'{2}'</td></tr></font>";


        private String m_AutoRefresh = "<html><head><title>SET Log file</title><meta http-equiv=\"refresh\" content=\"1\">";

        private SemaphoreSlim m_SemaphoreSlim = new SemaphoreSlim(1);

        public StorageFileEventListener(string name)
        {

            this.m_Name = name;
            Debug.WriteLine("StorageFileEventListener for {0} has name {1}", GetHashCode(), name);
            AssignLocalFile();
        }

        private async void AssignLocalFile()
        {

            try
            {
                m_StorageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(m_Name.Replace(" ", "_") + ".html");
            }
            catch (Exception) 
            {
                //File Not found
            }

            if (m_StorageFile == null)
            {
                m_StorageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(m_Name.Replace(" ", "_") + ".html",CreationCollisionOption.OpenIfExists);
                WriteToFile(m_AutoRefresh);
            }


            Debug.WriteLine("StorageFileEventListener path =  {0} ", m_StorageFile.Path);


            /////////////////////////
            //Add file name to clip board
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(m_StorageFile.Path);
            Clipboard.SetContent(dataPackage);

        }


        private async void WriteToFile(string line)
        {
            await m_SemaphoreSlim.WaitAsync();

            await Task.Run(async () =>
            {
                try
                {
                    await FileIO.AppendTextAsync(m_StorageFile, line + Environment.NewLine);
                }
                catch (Exception)
                {
                    // TODO:
                }
                finally
                {
                    m_SemaphoreSlim.Release();
                }
            });
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (m_StorageFile == null) 
                return;

            var newFormatedLine = string.Format(m_FormatHtml, DateTime.Now, eventData.Level, eventData.Payload[0],SevirityToColor(eventData.Level));
            //Debug.WriteLine(newFormatedLine);
            WriteToFile(newFormatedLine);
        }


        private String SevirityToColor(EventLevel level)
        {
            switch (level)
            {
                case EventLevel.Critical:
                    return "red";
                case EventLevel.Error:
                    return "orange";
                case EventLevel.Warning:
                    return "gold";
                case EventLevel.Informational:
                    return "blue";
                case EventLevel.Verbose:
                    return "green";
                default:
                    return "blue";

            }

        }
    }
}