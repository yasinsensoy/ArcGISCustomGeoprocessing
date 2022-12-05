using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;
using System.IO;

namespace MdbToGdb
{
    public class ConvertMdbToGDBWorkspace : IGPFunction2
    {
        private readonly string m_metadatafile = "ORBIS Workspace.xml";
        private readonly IGPUtilities m_GPUtilities;
        private IArray m_Parameters;

        public ConvertMdbToGDBWorkspace()
        {
            m_GPUtilities = new GPUtilitiesClass();
        }

        #region IGPFunction Members
        // Set the name of the function tool. 
        // This name appears when executing the tool at the command line or in scripting. 
        // This name should be unique to each toolbox and must not contain spaces.
        public string Name => "ORBIS Workspace";

        // Set the function tool Display Name as seen in ArcToolbox.
        public string DisplayName => "MDB to GDB";

        // This is the location where the parameters to the Function Tool are defined. 
        // This property returns an IArray of parameter objects (IGPParameter). 
        // These objects define the characteristics of the input and output parameters. 
        public IArray ParameterInfo
        {
            get
            {
                IArray parameters = new ArrayClass();
                IGPParameterEdit3 inputParameter = new GPParameterClass();
                inputParameter.DataType = new GPStringTypeClass();
                IGPString gpStringValue = new GPStringClass();
                gpStringValue.Value = "";
                inputParameter.Value = (IGPValue)gpStringValue;
                inputParameter.Direction = esriGPParameterDirection.esriGPParameterDirectionInput;
                inputParameter.DisplayName = "MDB File Full Path";
                inputParameter.Name = "in_mdb_full_path";
                inputParameter.ParameterType = esriGPParameterType.esriGPParameterTypeRequired;
                parameters.Add(inputParameter);
                IGPParameterEdit3 outputParameter = new GPParameterClass();
                outputParameter.DataType = new GPStringTypeClass();
                IGPString gpStringOutValue = new GPStringClass();
                gpStringOutValue.Value = "";
                outputParameter.Value = (IGPValue)gpStringOutValue;
                outputParameter.Direction = esriGPParameterDirection.esriGPParameterDirectionOutput;
                outputParameter.DisplayName = "GDB Workspace Path";
                outputParameter.Name = "out_gdb_full_path";
                IGPFeatureSchema outputSchema = new GPFeatureSchemaClass();
                IGPSchema schema = (IGPSchema)outputSchema;
                schema.CloneDependency = true;
                outputParameter.Schema = outputSchema as IGPSchema;
                outputParameter.AddDependency("in_mdb_full_path");
                parameters.Add(outputParameter);
                return parameters;
            }
        }

        // Validate: 
        // - Validate is an IGPFunction method, and we need to implement it in case there
        //   is legacy code that queries for the IGPFunction interface instead of the IGPFunction2 
        //   interface.  
        // - This Validate code is boilerplate - copy and insert into any IGPFunction2 code..
        // - This is the calling sequence that the gp framework now uses when it QI's for IGPFunction2..
        public IGPMessages Validate(IArray paramvalues, bool updateValues, IGPEnvironmentManager envMgr)
        {
            if (m_Parameters == null)
                m_Parameters = ParameterInfo;
            if (updateValues)
                UpdateParameters(paramvalues, envMgr);
            IGPMessages validateMsgs = m_GPUtilities.InternalValidate(m_Parameters, paramvalues, updateValues, true, envMgr);
            UpdateMessages(paramvalues, envMgr, validateMsgs);
            return validateMsgs;
        }

        // This method will update the output parameter value with the additional area field.
        public void UpdateParameters(IArray paramvalues, IGPEnvironmentManager pEnvMgr)
        {
            m_Parameters = paramvalues;
            IGPValue parameterValue = m_GPUtilities.UnpackGPValue(m_Parameters.get_Element(0));
            IGPParameter3 outGdbName = (IGPParameter3)m_Parameters.get_Element(1);
            IGPFeatureSchema schema = (IGPFeatureSchema)outGdbName.Schema;
            schema.AdditionalFields = null;
            if (!parameterValue.IsEmpty())
                outGdbName.Value.SetAsText(parameterValue.GetAsText() + ".gdb");
        }

        // Called after returning from the update parameters routine. 
        // You can examine the messages created from internal validation and change them if desired. 
        public void UpdateMessages(IArray paramvalues, IGPEnvironmentManager pEnvMgr, IGPMessages Messages)
        {
            IGPMessage msg = (IGPMessage)Messages;
            if (msg.IsError())
                return;
            IGPParameter parameter = (IGPParameter)paramvalues.get_Element(0);
            IGPValue parameterValue = m_GPUtilities.UnpackGPValue(parameter);
            string mdbFullPath = parameterValue.GetAsText();
            if (!File.Exists(mdbFullPath) || !mdbFullPath.ToLower().EndsWith(".mdb"))
                Messages.ReplaceWarning(0, "Lütfen geçerli bir ACCESS(mdb) workspace dosyası seçiniz.");
        }

        // Execute: Execute the function given the array of the parameters
        public void Execute(IArray paramvalues, ITrackCancel trackcancel, IGPEnvironmentManager envMgr, IGPMessages message)
        {
            IGPParameter parameter = (IGPParameter)paramvalues.get_Element(0);
            string mdbFullPath = parameter.Value.GetAsText();
            FileInfo f = new FileInfo(mdbFullPath);
            MdbToGdb.instance.Convert(f.Directory.FullName, f.Name);
        }

        // This is the function name object for the Geoprocessing Function Tool. 
        // This name object is created and returned by the Function Factory.
        // The Function Factory must first be created before implementing this property.
        public IName FullName
        {
            get
            {
                // Add CalculateArea.FullName getter implementation
                IGPFunctionFactory functionFactory = new ConvertMdbToGDBWorkspaceFactory();
                return (IName)functionFactory.GetFunctionName(Name);
            }
        }

        // This is used to set a custom renderer for the output of the Function Tool.
        public object GetRenderer(IGPParameter pParam) => null;

        // This is the unique context identifier in a [MAP] file (.h). 
        // ESRI Knowledge Base article #27680 provides more information about creating a [MAP] file. 
        public int HelpContext => 0;

        // This is the path to a .chm file which is used to describe and explain the function and its operation. 
        public string HelpFile => "";

        // This is used to return whether the function tool is licensed to execute.
        public bool IsLicensed()
        {
            IAoInitialize aoi = new AoInitializeClass();
            ILicenseInformation licInfo = (ILicenseInformation)aoi;
            string licName = licInfo.GetLicenseProductName(aoi.InitializedProduct());
            if (licName == "Advanced" || licName == "ArcGIS Server")
                return true;
            else
                return false;
        }

        // This is the name of the (.xml) file containing the default metadata for this function tool. 
        // The metadata file is used to supply the parameter descriptions in the help panel in the dialog. 
        // If no (.chm) file is provided, the help is based on the metadata file. 
        // ESRI Knowledge Base article #27000 provides more information about creating a metadata file.
        public string MetadataFile
        {
            // if you just return the name of an *.xml file as follows:
            // get { return m_metadatafile; }
            // then the metadata file will be created 
            // in the default location - <install directory>\help\gp

            // alternatively, you can send the *.xml file to the location of the DLL.
            // 
            get
            {
                string filePath;
                filePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                filePath = Path.Combine(filePath, m_metadatafile);
                return filePath;
            }
        }

        // By default, the Toolbox will create a dialog based upon the parameters returned 
        // by the ParameterInfo property.
        public UID DialogCLSID => null;
        #endregion
    }
}
