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
        // Register the Function Factory with the ESRI Geoprocessor Function Factory Component Category.
        #region "Component Category Registration"
        [ComRegisterFunction()]
        static void Reg(string regKey) => GPFunctionFactories.Register(regKey);

        [ComUnregisterFunction()]
        static void Unreg(string regKey) => GPFunctionFactories.Unregister(regKey);
        #endregion

        // Utility Function added to create the function names.
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

        // Implementation of the Function Factory
        #region IGPFunctionFactory Members

        // This is the name of the function factory. 
        // This is used when generating the Toolbox containing the function tools of the factory.
        public string Name => "ORBIS Workspace";

        // This is the alias name of the factory.
        public string Alias => "Convert MDB to GDB Workspace";

        // This is the class id of the factory. 
        public UID CLSID
        {
            get
            {
                UID id = new UIDClass();
                id.Value = GetType().GUID.ToString("B");
                return id;
            }
        }

        // This method will create and return a function object based upon the input name.
        public IGPFunction GetFunction(string Name)
        {
            if (Name == "ORBIS Workspace")
                return new ConvertMdbToGDBWorkspace();
            return null;
        }

        // This method will create and return a function name object based upon the input name.
        public IGPName GetFunctionName(string Name)
        {
            if (Name == "ORBIS Workspace")
                return (IGPName)CreateGPFunctionNames(0);
            return null;
        }

        // This method will create and return an enumeration of function names that the factory supports.
        public IEnumGPName GetFunctionNames()
        {
            IArray nameArray = new EnumGPNameClass();
            nameArray.Add(CreateGPFunctionNames(0));
            return (IEnumGPName)nameArray;
        }

        // This method will create and return an enumeration of GPEnvironment objects. 
        // If tools published by this function factory required new environment settings, 
        //then you would define the additional environment settings here. 
        // This would be similar to how parameters are defined. 
        public IEnumGPEnvironment GetFunctionEnvironments()
        {
            return null;
        }
        #endregion
    }
}
