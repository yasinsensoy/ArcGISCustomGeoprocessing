using ESRI.ArcGIS;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MdbToGdb
{
    public class MdbToGdb
    {
        public string Convert(string mdbPath, IGPMessages message)
        {
            string gdbPath = "";
            try
            {
                RuntimeManager.Bind(ProductCode.EngineOrDesktop);
                string gdbName = Path.GetFileNameWithoutExtension(mdbPath) + ".gdb";
                string dirName = Path.GetDirectoryName(mdbPath);
                CreateFileGDB createFileGDBTool = new CreateFileGDB(dirName, gdbName);
                ESRI.ArcGIS.Geoprocessor.Geoprocessor gp = new ESRI.ArcGIS.Geoprocessor.Geoprocessor();
                gp.OverwriteOutput = true;
                gp.SetEnvironmentValue("workspace", mdbPath);
                gp.Execute(createFileGDBTool, null);
                gdbPath = Path.Combine(dirName, gdbName);
                IGpEnumList lfc = gp.ListFeatureClasses("", "", "");
                string fc;
                while ((fc = lfc.Next()) != "")
                {
                    message.AddMessage("Copying " + fc + " to " + gdbName);
                    gp.Execute(CopyTool(fc, $"{gdbPath}\\{fc}"), null);
                    message.AddMessage("Copy " + fc + " to " + gdbName);
                }
                IGpEnumList lds = gp.ListDatasets("", "");
                string ds;
                while ((ds = lds.Next()) != "")
                {
                    message.AddMessage("Copying " + ds + " to " + gdbName);
                    gp.Execute(CopyTool(ds, $"{gdbPath}\\{ds}", "FeatureDataset"), null);
                    message.AddMessage("Copy " + ds + " to " + gdbName);
                }
                IGpEnumList ltbl = gp.ListTables("", "");
                string tbl;
                while ((tbl = ltbl.Next()) != "")
                {
                    message.AddMessage("Copying " + tbl + " to " + gdbName);
                    gp.Execute(CopyTool(tbl, $"{gdbPath}\\{tbl}"), null);
                    message.AddMessage("Copy " + tbl + " to " + gdbName);
                }
            }
            catch (Exception e)
            {
                message.AddError(e.HResult, e.Message.Trim() + "\r\n\r\n" + e.StackTrace.Trim());
                gdbPath = "";
            }
            finally
            {
                ReleaseObjects();
            }
            return gdbPath;
        }

        protected void ReleaseObjects(params object[] objects)
        {
            foreach (var o in objects)
                Marshal.ReleaseComObject(o);
        }

        protected Copy CopyTool(string inData, string outData, string dataType = null) => new Copy { in_data = inData, out_data = outData, data_type = dataType };
    }
}
