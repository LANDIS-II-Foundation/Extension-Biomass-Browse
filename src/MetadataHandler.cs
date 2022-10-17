using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;
using Landis.Utilities;
using Landis.Core;

namespace Landis.Extension.Browse
{
    public static class MetadataHandler
    {
        
        public static ExtensionMetadata Extension {get; set;}

        public static void InitializeMetadata(int Timestep)
        {
            ScenarioReplicationMetadata scenRep = new ScenarioReplicationMetadata() {
                RasterOutCellArea = PlugIn.ModelCore.CellArea,
                TimeMin = PlugIn.ModelCore.StartTime,
                TimeMax = PlugIn.ModelCore.EndTime//,
                //ProjectionFilePath = "Projection.?" 
            };

            Extension = new ExtensionMetadata(PlugIn.ModelCore){
                Name = PlugIn.ExtensionName,
                TimeInterval = Timestep, 
                ScenarioReplicationMetadata = scenRep
            };

            //---------------------------------------
            //          table outputs:   
            //---------------------------------------

            PlugIn.eventLog = new MetadataTable<EventsLog>("browse-events-log.csv");
            PlugIn.eventSpeciesLog = new MetadataTable<EventsSpeciesLog>("browse-event-species-log.csv");
            PlugIn.summaryLog = new MetadataTable<SummaryLog>("browse-summary-log.csv");
            PlugIn.calibrateLog = new MetadataTable<CalibrateLog>("browse-calibrate-log.csv");

            OutputMetadata tblOut_events = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "EventLog",
                FilePath = PlugIn.eventLog.FilePath,
                Visualize = false,
            };
            tblOut_events.RetriveFields(typeof(EventsLog));
            Extension.OutputMetadatas.Add(tblOut_events);

            PlugIn.ModelCore.UI.WriteLine("   Generating summary table...");
            OutputMetadata tblOut_summary = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "BrowseSummaryLog",
                FilePath = PlugIn.summaryLog.FilePath,
                Visualize = true,
            };
            tblOut_summary.RetriveFields(typeof(SummaryLog));
            Extension.OutputMetadatas.Add(tblOut_summary);

            PlugIn.ModelCore.UI.WriteLine("   Generating summary table...");
            OutputMetadata tblOut_species = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "BrowseSpeciesLog",
                FilePath = PlugIn.eventSpeciesLog.FilePath,
                Visualize = true,
            };
            tblOut_species.RetriveFields(typeof(EventsSpeciesLog));
            Extension.OutputMetadatas.Add(tblOut_species);

            PlugIn.ModelCore.UI.WriteLine("   Generating calibration table...");
            OutputMetadata tblOut_calibration = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "CalibrationLog",
                FilePath = PlugIn.calibrateLog.FilePath,
                Visualize = true,
            };
            tblOut_calibration.RetriveFields(typeof(CalibrateLog));
            Extension.OutputMetadatas.Add(tblOut_calibration);


            //---------------------------------------            
            //          map outputs:         
            //---------------------------------------

            //OutputMetadata mapOut_Severity = new OutputMetadata()
            //{
            //    Type = OutputType.Map,
            //    Name = "severity",
            //    FilePath = @MapFileName,
            //    Map_DataType = MapDataType.Ordinal,
            //    Map_Unit = FieldUnits.Severity_Rank,
            //    Visualize = true,
            //};
            //Extension.OutputMetadatas.Add(mapOut_Severity);

            //---------------------------------------
            MetadataProvider mp = new MetadataProvider(Extension);
            mp.WriteMetadataToXMLFile("Metadata", Extension.Name, Extension.Name);




        }
    }
}
