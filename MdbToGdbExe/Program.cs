using ESRI.ArcGIS.esriSystem;
using System;

namespace MdbToGdb
{
    class Program
    {
        static readonly LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();

        static void Main(string[] args)
        {
            if (!m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced }, new esriLicenseExtensionCode[] { }))
                m_AOLicenseInitializer.ShutdownApplication();
            if (args.Length > 0)
            {
                MdbToGdb mdbToGdb = new MdbToGdb();
                mdbToGdb.Convert(args[0]);
            }
            else
                throw new Exception("Eksik parametre mdb dosyasının yolunu göndermelisiniz.");
        }
    }
}