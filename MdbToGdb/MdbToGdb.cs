using ESRI.ArcGIS;
using ESRI.ArcGIS.ConversionTools;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.IO;

namespace MdbToGdb
{
    public class MdbToGdb
    {
        public static MdbToGdb instance = new MdbToGdb();

        public void Convert(string parentDirectory, string mdbFileName)
        {
            if (!RuntimeManager.Bind(ProductCode.EngineOrDesktop))
                Console.WriteLine("");
            IAoInitialize licenseInitializer = new AoInitializeClass();
            if (licenseInitializer.Initialize(esriLicenseProductCode.esriLicenseProductCodeStandard) != esriLicenseStatus.esriLicenseCheckedOut)
                Console.WriteLine("");
            ESRI.ArcGIS.Geoprocessor.Geoprocessor gp = new ESRI.ArcGIS.Geoprocessor.Geoprocessor();
            IWorkspaceFactory workspaceFactoryTarget = new FileGDBWorkspaceFactory();
            IWorkspaceName targetWorkspaceName = workspaceFactoryTarget.Create(parentDirectory, mdbFileName, null, 0);
            IName targetWorkspaceNameName = (IName)targetWorkspaceName;
            IWorkspace targetWorkspace = (IWorkspace)targetWorkspaceNameName.Open();
            IWorkspaceFactory sourceWorkspaceFactory = new AccessWorkspaceFactory();
            IPropertySet sourcePropertySet = new PropertySetClass();
            sourcePropertySet.SetProperty("DATABASE", Path.Combine(parentDirectory, mdbFileName));
            IWorkspace sourceWorkspace = sourceWorkspaceFactory.Open(sourcePropertySet, 0);
            IEnumDatasetName sourceEnumDatasetName = sourceWorkspace.get_DatasetNames(esriDatasetType.esriDTAny);
            IDatasetName sourceDatasetName;
            while ((sourceDatasetName = sourceEnumDatasetName.Next()) != null)
            {
                IDatasetName sourceDatasetNameDatasetName = sourceDatasetName;
                switch (sourceDatasetNameDatasetName.Type)
                {
                    case esriDatasetType.esriDTFeatureDataset:
                        {
                            IFeatureDataset featureDataset = (IFeatureDataset)((IName)sourceDatasetName).Open();
                            IGeoDataset geoDataset = (IGeoDataset)featureDataset;
                            CreateFeatureDataset cfd = new CreateFeatureDataset
                            {
                                spatial_reference = geoDataset.SpatialReference,
                                out_dataset_path = targetWorkspace,
                                out_name = sourceDatasetName.Name
                            };
                            gp.Execute(cfd, null);
                            IEnumDatasetName targetEnumDatasetName = targetWorkspace.get_DatasetNames(esriDatasetType.esriDTAny);
                            IDatasetName targetDatasetName;
                            while ((targetDatasetName = targetEnumDatasetName.Next()) != null)
                            {
                                if (targetDatasetName.Name.Equals(sourceDatasetName.Name))
                                    break;
                            }
                            IEnumDatasetName sourceSubEnumDataSetName = sourceDatasetNameDatasetName.SubsetNames;
                            IDatasetName sourceSubDatasetName;
                            while ((sourceSubDatasetName = sourceSubEnumDataSetName.Next()) != null)
                            {
                                switch (sourceSubDatasetName.Type)
                                {
                                    case esriDatasetType.esriDTFeatureClass:
                                        {
                                            IName sourceSubDatasetNameDatasetNameName = (IName)sourceSubDatasetName;
                                            IFeatureClass sourceObjectClass = (IFeatureClass)sourceSubDatasetNameDatasetNameName.Open();
                                            FeatureClassToFeatureClass fcfc = new FeatureClassToFeatureClass
                                            {
                                                in_features = sourceObjectClass,
                                                out_path = targetWorkspace,
                                                out_name = sourceSubDatasetName.Name
                                            };
                                            gp.Execute(fcfc, null);
                                        }
                                        break;
                                    case esriDatasetType.esriDTTable:
                                        {
                                            IName sourceDatasetNameDatasetNameName = (IName)sourceDatasetNameDatasetName;
                                            ITable sourceObjectClass = (ITable)sourceDatasetNameDatasetNameName.Open();
                                            TableToTable ttt = new TableToTable
                                            {
                                                in_rows = sourceObjectClass,
                                                out_path = targetWorkspace,
                                                out_name = sourceSubDatasetName.Name
                                            };
                                            gp.Execute(ttt, null);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        break;
                    case esriDatasetType.esriDTFeatureClass:
                        {
                            IName sourceDatasetNameDatasetNameName = (IName)sourceDatasetNameDatasetName;
                            IFeatureClass sourceObjectClass = (IFeatureClass)sourceDatasetNameDatasetNameName.Open();
                            FeatureClassToFeatureClass fcfc = new FeatureClassToFeatureClass
                            {
                                in_features = sourceObjectClass,
                                out_path = targetWorkspace,
                                out_name = sourceDatasetName.Name
                            };
                            gp.Execute(fcfc, null);
                        }
                        break;
                    case esriDatasetType.esriDTTable:
                        {
                            IName sourceDatasetNameDatasetNameName = (IName)sourceDatasetNameDatasetName;
                            ITable sourceObjectClass = (ITable)sourceDatasetNameDatasetNameName.Open();
                            TableToTable ttt = new TableToTable
                            {
                                in_rows = sourceObjectClass,
                                out_path = targetWorkspace,
                                out_name = sourceDatasetName.Name
                            };
                            gp.Execute(ttt, null);
                        }
                        break;
                    default:
                        break;
                }
                string sourceName = sourceDatasetName.Name;
                Console.WriteLine(sourceName);
            }
        }
    }
}
