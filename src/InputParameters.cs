//  Authors:  Brian Miranda, Nate De Jager, Patrick Drohan

using Landis.Utilities;
using System.Collections.Generic;

namespace Landis.Extension.Browse
{
    public enum NeighborShape { uniform, linear, gaussian };
    public enum NeighborSpeed { none, X2, X3, X4 };
	/// <summary>
	/// Parameters for the plug-in.
	/// </summary>
    public class InputParameters
        : IInputParameters
    {
        private int timestep;
        private ISppParameters[] sppParameters;
        private string zoneMapFileName;
        private string populationFileName;
        private string dynamicPopulationFileName;
        private double consumptionRate;
        private static double anppForageProp;
        private double minBrowseinReach;
        private double browseBiomassThresh;
        private double escapeBrowsePropLong;
        private bool growthReduction;
        private bool mortality;
        private bool countNonForage;
        private bool useInitBiomass;
        private double forageQuantityNbrRad;
        private double sitePrefNbrRad;
        private string sitePrefMapNamesTemplate;
        private string siteForageMapNamesTemplate;
        private string siteHSIMapNamesTemplate;
        private string sitePopMapNamesTemplate;
        private string biomassRemovedMapNamesTemplate;
        private string logFileName;
        private string successionMethod;
        //private double neighborRadius;
        //private NeighborShape shapeOfNeighbor;
        //private NeighborSpeed neighborSpeedUp;
        private IEnumerable<RelativeLocationWeighted> forageNeighbors;
        private IEnumerable<RelativeLocationWeighted> sitePrefNeighbors;
        private List<double> preferenceList;

        private Dictionary<int, IDynamicInputRecord[]> temporalData;




        //---------------------------------------------------------------------

        /// <summary>
        /// Timestep (years)
        /// </summary>
        public int Timestep
        {
            get
            {
                return timestep;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                                                      "Value must be = or > 0.");
                timestep = value;
            }
        }

        //---------------------------------------------------------------------
        public ISppParameters[] SppParameters
        {
            get
            {
                return sppParameters;
            }
            set
            {
                sppParameters = value;
            }
        }
        //---------------------------------------------------------------------
        public Dictionary<int, IDynamicInputRecord[]> TemporalData
        {
            get
            {
                return temporalData;
            }
            set
            {
                temporalData = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Name of zone map file.
        /// </summary>
        public string ZoneMapFileName
        {
            get
            {
                return zoneMapFileName;
            }
            set
            {
                if (value == null)
                    throw new InputValueException(value.ToString(), "Value must be a file path.");
                zoneMapFileName = value;
            }
        }
        //---------------------------------------------------------------------
        public string PopulationFileName
        {
            get
            {
                return populationFileName;
            }
            set
            {
                if (value == null)
                    throw new InputValueException(value.ToString(), "Value must be a file path.");
                populationFileName = value;
            }
        }
        //---------------------------------------------------------------------
        public string DynamicPopulationFileName
            //TODO SF do we need to fix this?
        {
            get
            {
                return dynamicPopulationFileName;
            }
            set
            {
                if (value == null)
                    PlugIn.ModelCore.UI.WriteLine("   DynamicPopulationFile could not be found.  Population will be based on PopulationFile");
                dynamicPopulationFileName = value;
            }
        }
        //---------------------------------------------------------------------
        public double ConsumptionRate
        {
            get
            {
                return consumptionRate;
            }
            set
            {
                consumptionRate = value;
            }
        }
        //---------------------------------------------------------------------
        public double ANPPForageProp
        {
            get
            {
                return anppForageProp;
            }
            set
            {
                anppForageProp = value;
            }
        }
        //---------------------------------------------------------------------
        public double MinBrowsePropinReach
        {
            get
            {
                return minBrowseinReach;
            }
            set
            {
                minBrowseinReach = value;
            }
        }
        //---------------------------------------------------------------------
        public double BrowseBiomassThresh
        {
            get
            {
                return browseBiomassThresh;
            }
            set
            {
                browseBiomassThresh = value;
            }
        }
        //---------------------------------------------------------------------
        public double EscapeBrowsePropLong
        {
            get
            {
                return escapeBrowsePropLong;
            }
            set
            {
                escapeBrowsePropLong = value;
            }
        }
        //---------------------------------------------------------------------
        public bool GrowthReduction
        {
            get
            {
                return growthReduction;
            }
            set
            {
                growthReduction = value;
            }
        }
        //---------------------------------------------------------------------
        public bool Mortality
        {
            get
            {
                return mortality;
            }
            set
            {
                mortality = value;
            }
        }
        //---------------------------------------------------------------------
        public bool CountNonForage
        {
            get
            {
                return countNonForage;
            }
            set
            {
                countNonForage = value;
            }
        }
        //---------------------------------------------------------------------
        public bool UseInitBiomass
        {
            get
            {
                return useInitBiomass;
            }
            set
            {
                useInitBiomass = value;
            }
        }
        //---------------------------------------------------------------------
        public double ForageQuantityNbrRad
        {
            get
            {
                return forageQuantityNbrRad;
            }
            set
            {
                forageQuantityNbrRad = value;
            }
        }
        //---------------------------------------------------------------------
        public double SitePrefNbrRad
        {
            get
            {
                return sitePrefNbrRad;
            }
            set
            {
                sitePrefNbrRad = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for output maps.
        /// </summary>
        public string SitePrefMapNamesTemplate
        {
            get
            {
                return sitePrefMapNamesTemplate;
            }
            set
            {
                MapNames.CheckTemplateVars(value);
                sitePrefMapNamesTemplate = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for output maps.
        /// </summary>
        public string SiteForageMapNamesTemplate
        {
            get
            {
                return siteForageMapNamesTemplate;
            }
            set
            {
                MapNames.CheckTemplateVars(value);
                siteForageMapNamesTemplate = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for output maps.
        /// </summary>
        public string SiteHSIMapNamesTemplate
        {
            get
            {
                return siteHSIMapNamesTemplate;
            }
            set
            {
                MapNames.CheckTemplateVars(value);
                siteHSIMapNamesTemplate = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for output maps.
        /// </summary>
        public string SitePopMapNamesTemplate
        {
            get
            {
                return sitePopMapNamesTemplate;
            }
            set
            {
                MapNames.CheckTemplateVars(value);
                sitePopMapNamesTemplate = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for output maps.
        /// </summary>
        public string BiomassRemovedMapNamesTemplate
        {
            get
            {
                return biomassRemovedMapNamesTemplate;
            }
            set
            {
                MapNames.CheckTemplateVars(value);
                biomassRemovedMapNamesTemplate = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Name of log file.
        /// </summary>
        public string LogFileName
        {
            get
            {
                return logFileName;
            }
            set
            {
                if (value == null)
                    throw new InputValueException(value.ToString(), "Value must be a file path.");
                logFileName = value;
            }
        }

        //---------------------------------------------------------------------

        public InputParameters(int sppCount)
        {
            SppParameters = new ISppParameters[sppCount];
        }

        //---------------------------------------------------------------------

        public string SuccessionMethod
        {
            get
            {
                return successionMethod;
            }
            set
            {
                successionMethod = value;
            }
        }

        //---------------------------------------------------------------------
        public IEnumerable<RelativeLocationWeighted> ForageNeighbors
        {
            get
            {
                return forageNeighbors;
            }
            set
            {
                forageNeighbors = value;
            }
        }
        //---------------------------------------------------------------------
        public IEnumerable<RelativeLocationWeighted> SitePrefNeighbors
        {
            get
            {
                return sitePrefNeighbors;
            }
            set
            {
                sitePrefNeighbors = value;
            }
        }
        //---------------------------------------------------------------------
        public List<double> PreferenceList
        {
            get
            {
                return preferenceList;
            }
            set
            {
                preferenceList = value;
            }
        }
        //---------------------------------------------------------------------
        /*
        public double NeighborRadius
        {
            get
            {
                return neighborRadius;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                                                      "Value must be = or > 0.");
                neighborRadius = value;
            }
        }
        //---------------------------------------------------------------------
        public NeighborShape ShapeOfNeighbor
        {
            get
            {
                return shapeOfNeighbor;
            }
            set
            {
                shapeOfNeighbor = value;
            }
        }
        //---------------------------------------------------------------------
        public NeighborSpeed NeighborSpeedUp
        {
            get
            {
                return neighborSpeedUp;
            }
            set
            {
                neighborSpeedUp = value;
            }
        }
        //---------------------------------------------------------------------
        */

    }
        
    //---------------------------------------------------------------------
    /// <summary>
    /// Interface to the Parameters for the extension
    /// </summary>
    public interface IInputParameters
    {
        Dictionary<int, IDynamicInputRecord[]> TemporalData { get; set; }
        int Timestep { get; set; }
        ISppParameters[] SppParameters { get; set; }
        string ZoneMapFileName { get; set; }
        string PopulationFileName { get; set; }
        string DynamicPopulationFileName { get; set; }
        double ConsumptionRate { get; set; }
        double ANPPForageProp { get; set; }
        double MinBrowsePropinReach { get; set; }
        double BrowseBiomassThresh { get; set; }
        double EscapeBrowsePropLong { get; set; }
        bool GrowthReduction { get; set; }
        bool Mortality { get; set; }
        bool CountNonForage { get; set; }
        bool UseInitBiomass { get; set; }
        double ForageQuantityNbrRad { get; set; }
        double SitePrefNbrRad { get; set; }
        string SitePrefMapNamesTemplate { get; set; }
        string SiteForageMapNamesTemplate { get; set; }
        string SiteHSIMapNamesTemplate { get; set; }
        string SitePopMapNamesTemplate { get; set; }
        string BiomassRemovedMapNamesTemplate { get; set; }
        string LogFileName { get; set; }
        string SuccessionMethod { get; set; }
        //double NeighborRadius { get; set; }
        //NeighborShape ShapeOfNeighbor { get; set; }
        //NeighborSpeed NeighborSpeedUp { get; set; }
        IEnumerable<RelativeLocationWeighted> ForageNeighbors { get; set; }
        IEnumerable<RelativeLocationWeighted> SitePrefNeighbors { get; set; }
        List<double> PreferenceList { get; set; }

        
    }
}
