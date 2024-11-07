using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace MdbToGdb
{
    public class MdbToGdb
    {
        private readonly object kitle = new object();
        private readonly string logPath = @"\\ogmdata.ogm.gov.tr\Orbis\YeniOrbisRapor\orbis_harita\ORBIS\Geoprocessing\MdbToGdb\MdbToGdb Log.txt";
        private readonly DateTime start = DateTime.Now;
        private DateTime time = DateTime.MinValue;
        private string allLog;
        private IGeoProcessor2 gp;
        private IVariantArray param = null;
        private IGeoProcessorResult result = null;
        private AccessWorkspaceFactory mdbFactory = null;
        private IWorkspace mdbWorkspace = null;
        private IEnumDataset enumDataset = null;
        private IDataset ds = null;

        public void Convert(string mdbPath)
        {
            Log("Dönüştürme işlemi başladı. MdbPath: " + mdbPath);
            try
            {
                string gdbName = Path.GetFileNameWithoutExtension(mdbPath);
                string dirName = Path.GetDirectoryName(mdbPath);
                gp = new GeoProcessorClass();
                string gdbPath = ExecuteTool("CreateFileGDB", dirName, gdbName);
                mdbFactory = new AccessWorkspaceFactoryClass();
                mdbWorkspace = mdbFactory.OpenFromFile(mdbPath, 0);
                enumDataset = mdbWorkspace.Datasets[esriDatasetType.esriDTAny];
                while ((ds = enumDataset.Next()) != null)
                {
                    string inData = $"{mdbPath}\\{ds.Name}";
                    string outData = $"{gdbPath}\\{ds.Name}";
                    if (ds.Type == esriDatasetType.esriDTFeatureClass)
                        ExecuteTool("Copy", inData, outData);
                    else if (ds.Type == esriDatasetType.esriDTFeatureDataset)
                        ExecuteTool("Copy", inData, outData, "FeatureDataset");
                    else if (ds.Type == esriDatasetType.esriDTTable)
                        ExecuteTool("Copy", inData, outData);
                }
                Console.Write(gdbPath);
                Log($"Dönüştürme işlemi bitti.\r\nGdbPath: {gdbPath}");
            }
            catch (COMException e)
            {
                string mes = ESRIErrorCodeInterpreter.GetComExceptionMessage(e);
                Log($"{mes}\r\n{e.StackTrace}", true);
            }
            catch (Exception e)
            {
                Log($"{e.HResult} {e.Message}\r\n{e.StackTrace}", true);
            }
            finally
            {
                ReleaseObjects(gp, param, result, mdbFactory, mdbWorkspace, enumDataset, ds);
                Log(new string('-', 100), addHeader: false, finish: true);
            }
        }

        private void Log(string log, bool error = false, bool addHeader = true, bool finish = false)
        {
            if (addHeader)
            {
                string elapsed = "";
                if (time != DateTime.MinValue)
                    elapsed = $" Elapsed Time: {(DateTime.Now - time).TotalSeconds} sec";
                time = DateTime.Now;
                log = $"{(error ? "Error" : "Info")}: {time}{elapsed}\r\n{log}";
            }
            if (finish)
                log += $" Total Time: {(DateTime.Now - start).TotalSeconds} sec";
            allLog += log + "\r\n\r\n";
            if (finish)
                Write(allLog);
        }

        private void Write(string log)
        {
            lock (kitle)
            {
                int i = 0;
                again:
                try
                {
                    using (var stream = new System.IO.FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        using (var writer = new StreamWriter(stream, Encoding.UTF8))
                        {
                            writer.Write(log);
                            writer.Flush();
                            writer.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    if (i++ < 5)
                    {
                        Thread.Sleep(1000);
                        goto again;
                    }
                    else
                        Log($"{e.HResult} {e.Message}\r\n{e.StackTrace}", true);
                }
            }
        }

        private void ReleaseObjects(params object[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    Marshal.ReleaseComObject(objects[i]);
                    objects[i] = null;
                }
            }
        }

        private string ExecuteTool(string toolName, params object[] parameters)
        {
            param = new VarArrayClass();
            string par = "";
            foreach (var p in parameters)
            {
                par += $"{(par.Length == 0 ? "" : ", ")}{p}";
                param.Add(p);
            }
            Log($"{toolName}({par}) started.");
            result = gp.Execute(toolName, param, null);
            string value = (string)result.ReturnValue;
            Log($"{toolName}({par}) finished.\r\nResult: {value}");
            return value;
        }
    }
}
