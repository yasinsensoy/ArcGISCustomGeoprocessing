using ESRI.ArcGIS;
using System;

namespace MdbToGdb
{
    internal partial class LicenseInitializer
    {
        public LicenseInitializer()
        {
            ResolveBindingEvent += new EventHandler(BindingArcGISRuntime);
        }

        void BindingArcGISRuntime(object sender, EventArgs e)
        {
            //
            // TODO: Modify ArcGIS runtime binding code as needed
            //
            if (!RuntimeManager.Bind(ProductCode.Desktop))
            {
                // Failed to bind, announce and force exit
                Console.WriteLine("Invalid ArcGIS runtime binding. Application will shut down.");
                System.Environment.Exit(0);
            }
        }
    }
}