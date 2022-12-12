using ESRI.ArcGIS;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MdbToGdb
{
    public class MdbToGdb
    {
        public string Convert(string mdbPath, IGPMessages message)
        {
            string gdbPath = "";
            CreateFileGDB createFileGDBTool = null;
            ESRI.ArcGIS.Geoprocessor.Geoprocessor gp = null;
            IGpEnumList list = null;
            try
            {
                RuntimeManager.Bind(ProductCode.EngineOrDesktop);
                string gdbName = Path.GetFileNameWithoutExtension(mdbPath) + ".gdb";
                string dirName = Path.GetDirectoryName(mdbPath);
                createFileGDBTool = new CreateFileGDB(dirName, gdbName);
                gp = new ESRI.ArcGIS.Geoprocessor.Geoprocessor();
                gp.OverwriteOutput = true;
                gp.SetEnvironmentValue("workspace", mdbPath);
                gp.Execute(createFileGDBTool, null);
                gdbPath = Path.Combine(dirName, gdbName);
                list = gp.ListFeatureClasses("", "", "");
                ExecuteCopyTool(gp, list, message, gdbName, gdbPath);
                list = gp.ListDatasets("", "");
                ExecuteCopyTool(gp, list, message, gdbName, gdbPath, "FeatureDataset");
                list = gp.ListTables("", "");
                ExecuteCopyTool(gp, list, message, gdbName, gdbPath);
                message.AddMessage("Dönüştürme işlemi başarıyla sonuçlandı.");
            }
            catch (Exception e)
            {
                string errPath = Path.Combine(Assembly.GetExecutingAssembly().Location, "MdbToGdb Error.txt");
                message.AddError(0, "Hata oluştu. Ayrıntılı hata mesajı için. " + errPath);
                string err = $"Error: {DateTime.Now} MdbPath: {mdbPath}\r\n{e.Message}\r\n{e.StackTrace}\r\n\r\n";
                if (File.Exists(errPath))
                    err = File.ReadAllText(errPath, Encoding.UTF8) + err;
                File.WriteAllText(errPath, err, Encoding.UTF8);
            }
            finally
            {
                ReleaseObjects(createFileGDBTool, gp, list);
            }
            return gdbPath;
        }

        protected void ReleaseObjects(params object[] objects)
        {
            foreach (var o in objects)
                if (o != null)
                    Marshal.ReleaseComObject(o);
        }

        protected void ExecuteCopyTool(ESRI.ArcGIS.Geoprocessor.Geoprocessor gp, IGpEnumList list, IGPMessages message, string gdbName, string gdbPath, string dataType = null)
        {
            string data;
            while ((data = list.Next()) != "")
            {
                message.AddMessage($"Copying {data} to {gdbName}");
                gp.Execute(CopyTool(data, $"{gdbPath}\\{data}", dataType), null);
                message.AddMessage($"Copy {data} to {gdbName}");
            }
        }

        protected Copy CopyTool(string inData, string outData, string dataType = null) => new Copy { in_data = inData, out_data = outData, data_type = dataType };
    }
}
