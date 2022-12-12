using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;
using System;
using System.Runtime.InteropServices;

namespace MdbToGdb
{
    [Guid("3554BFC7-94F9-4d28-B3FE-14D17599B35B"), ComVisible(true)]
    public class ConvertMdbToGDBWorkspaceFactory : IGPFunctionFactory
    {
        #region "Component Category Registration"
        [ComRegisterFunction()]
        static void Reg(string regKey) => GPFunctionFactories.Register(regKey);

        [ComUnregisterFunction()]
        static void Unreg(string regKey) => GPFunctionFactories.Unregister(regKey);
        #endregion

        private IGPFunctionName CreateGPFunctionNames(long index)
        {
            IGPFunctionName functionName = new GPFunctionNameClass();
            functionName.MinimumProduct = esriProductCode.esriProductCodeAdvanced;
            IGPName name;
            switch (index)
            {
                case (0):
                    name = (IGPName)functionName;
                    // name.Category = "Convert";
                    name.Description = "MDB içerisindeki FeatureClass ve Table objectClasslarını GDB içerisine aktarır.";
                    name.DisplayName = "MDB to GDB";
                    name.Name = "ORBIS Workspace";
                    name.Factory = this;
                    break;
            }
            return functionName;
        }

        #region IGPFunctionFactory Members

        public string Name => "ORBIS Workspace";

        public string Alias => "Convert MDB to GDB Workspace";

        public UID CLSID
        {
            get
            {
                UID id = new UIDClass();
                id.Value = GetType().GUID.ToString("B");
                return id;
            }
        }

        public IGPFunction GetFunction(string Name)
        {
            if (Name == "ORBIS Workspace")
                return new ConvertMdbToGDBWorkspace();
            return null;
        }

        public IGPName GetFunctionName(string Name)
        {
            if (Name == "ORBIS Workspace")
                return (IGPName)CreateGPFunctionNames(0);
            return null;
        }

        public IEnumGPName GetFunctionNames()
        {
            IArray nameArray = new EnumGPNameClass();
            nameArray.Add(CreateGPFunctionNames(0));
            return (IEnumGPName)nameArray;
        }

        public IEnumGPEnvironment GetFunctionEnvironments() => null;
        #endregion
    }
}
