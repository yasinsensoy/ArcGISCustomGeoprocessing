using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;
using System.IO;
using System.Runtime.InteropServices;

namespace MdbToGdb
{
    public class ConvertMdbToGDBWorkspace : IGPFunction2
    {
        private readonly string m_metadatafile = "OrbisMdbtoGdb.xml";
        private readonly IGPUtilities m_GPUtilities;
        private IArray m_Parameters;

        public ConvertMdbToGDBWorkspace()
        {
            m_GPUtilities = new GPUtilitiesClass();
        }

        ~ConvertMdbToGDBWorkspace()
        {
            if (m_GPUtilities != null)
                Marshal.ReleaseComObject(m_GPUtilities);
        }

        #region IGPFunction Members
        public string Name => "OrbisMdbtoGdb";

        public string DisplayName => "MDB to GDB";

        public int HelpContext => 0;

        public string HelpFile => "";

        public UID DialogCLSID => null;

        public IArray ParameterInfo
        {
            get
            {
                IGPParameterEdit3 inputParameter = new GPParameterClass();
                inputParameter.DataType = new GPStringTypeClass();
                inputParameter.Direction = esriGPParameterDirection.esriGPParameterDirectionInput;
                inputParameter.DisplayName = "MDB File Full Path";
                inputParameter.Name = "in_mdb_full_path";
                inputParameter.ParameterType = esriGPParameterType.esriGPParameterTypeRequired;
                IGPString value = new GPStringClass();
                value.Value = @"C:\Users\izambakci\Desktop\Yasin\Rapor\YENI_YOL\KURUCUOVA_TADILAT_YENI_YOL.mdb";
                inputParameter.Value = (IGPValue)value;
                IGPParameterEdit3 outputParameter = new GPParameterClass();
                outputParameter.DataType = new GPStringTypeClass();
                outputParameter.Direction = esriGPParameterDirection.esriGPParameterDirectionOutput;
                outputParameter.DisplayName = "GDB File Path";
                outputParameter.Name = "out_gdb_full_path";
                outputParameter.ParameterType = esriGPParameterType.esriGPParameterTypeDerived;
                IArray parameters = new ArrayClass();
                parameters.Add(inputParameter);
                parameters.Add(outputParameter);
                return parameters;
            }
        }

        public IName FullName
        {
            get
            {
                IGPFunctionFactory functionFactory = new ConvertMdbToGDBWorkspaceFactory();
                return (IName)functionFactory.GetFunctionName(Name);
            }
        }

        public string MetadataFile
        {
            get
            {
                string filePath;
                filePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                filePath = Path.Combine(filePath, m_metadatafile);
                return filePath;
            }
        }

        public void UpdateParameters(IArray paramvalues, IGPEnvironmentManager pEnvMgr)
        {
            m_Parameters = paramvalues;
        }

        public void UpdateMessages(IArray paramvalues, IGPEnvironmentManager pEnvMgr, IGPMessages Messages)
        {
            IGPMessage msg = (IGPMessage)Messages;
            if (msg.IsError())
                return;
            IGPParameter parameter = (IGPParameter)paramvalues.get_Element(0);
            IGPValue parameterValue = m_GPUtilities.UnpackGPValue(parameter);
            string mdbFullPath = parameterValue.GetAsText();
            if (!File.Exists(mdbFullPath) || !mdbFullPath.ToLower().EndsWith(".mdb"))
                Messages.ReplaceError(0, 0, "Lütfen geçerli bir ACCESS(mdb) workspace dosyası seçiniz.");
        }

        public void Execute(IArray paramvalues, ITrackCancel trackcancel, IGPEnvironmentManager envMgr, IGPMessages message)
        {
            IGPParameter parameter = (IGPParameter)paramvalues.get_Element(0);
            string mdbFullPath = parameter.Value.GetAsText();
            MdbToGdb toGdb = new MdbToGdb();
            string gdbPath = toGdb.Convert(mdbFullPath, message);
            IGPParameter3 outGdbName = (IGPParameter3)m_Parameters.get_Element(1);
            outGdbName.Value.SetAsText(gdbPath);
        }

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

        public IGPMessages Validate(IArray paramvalues, bool updateValues, IGPEnvironmentManager envMgr) => null;

        public object GetRenderer(IGPParameter pParam) => null;
        #endregion
    }
}
