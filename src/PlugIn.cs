//  Authors:  Brian Miranda, Nate De Jager, Patrick Drohan, Robert Scheller

using Landis.SpatialModeling;
using Landis.Core;
using System.Collections.Generic;
using System.IO;
using Landis.Library.BiomassCohorts;

namespace Landis.Extension.Browse
{
    ///<summary>
    /// A disturbance plug-in that simulates browsing by ungulates.
    /// </summary>

    public class PlugIn
        : ExtensionMain
    {
        public static readonly ExtensionType ExtType = new ExtensionType("disturbance:browse");
        public static readonly string ExtensionName = "Biomass Browse";
        
        private string sitePrefMapNameTemplate;
        private string siteForageMapNameTemplate;
        private string siteHSIMapNameTemplate;
        private string sitePopMapNamesTemplate;
        private string biomassRemovedMapNameTemplate;
        private StreamWriter eventLog;
        //private StreamWriter summaryLog;
        private IInputParameters parameters;
        private static ICore modelCore;
        private bool running;

        //Dynamic Population Parameters
        public static double PopRMin;
        public static double PopRMax;
        public static double PopMortalityMin;
        public static double PopMortalityMax;
        public static double PopHarvestMin;
        public static double PopHarvestMax;
        public static double PopPredationMin;
        public static double PopPredationMax;
        public static bool DynamicPopulation = false; //SF changed this so that static population can happen -- otherwise
                                                      //dynamic is always used


        //---------------------------------------------------------------------

        public PlugIn()
            : base(ExtensionName, ExtType)
        {
        }

        //---------------------------------------------------------------------

        public static ICore ModelCore
        {
            get
            {
                return modelCore;
            }
        }
        //---------------------------------------------------------------------

        public override void LoadParameters(string dataFile, ICore mCore)
        {
            modelCore = mCore;
            InputParameterParser parser = new InputParameterParser();
            parameters = Landis.Data.Load<IInputParameters>(dataFile, parser);

            // Add local event handler for cohorts death due to age-only
            // disturbances.
            Cohort.AgeOnlyDeathEvent += CohortKilledByAgeOnlyDisturbance;
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the plug-in with a data file.
        /// </summary>
        /// <param name="dataFile">
        /// Path to the file with initialization data.
        /// </param>
        /// <param name="startTime">
        /// Initial timestep (year): the timestep that will be passed to the
        /// first call to the component's Run method.
        /// </param>
        public override void Initialize()
        {
            Timestep = parameters.Timestep;
            sitePrefMapNameTemplate = parameters.SitePrefMapNamesTemplate;
            siteForageMapNameTemplate = parameters.SiteForageMapNamesTemplate;
            siteHSIMapNameTemplate = parameters.SiteHSIMapNamesTemplate;
            sitePopMapNamesTemplate = parameters.SitePopMapNamesTemplate;
            biomassRemovedMapNameTemplate = parameters.BiomassRemovedMapNamesTemplate;

            SiteVars.Initialize();

            PopulationZones.ReadMap(parameters.ZoneMapFileName);

            DynamicInputs.Initialize(parameters.PopulationFileName, false, parameters);

            //DynamicPopulation.Initialize(parameters.DynamicPopulationFileName, false);

            parameters.PreferenceList =  PreferenceList.Initialize(parameters.SppParameters);

            PartialDisturbance.Initialize();
            GrowthReduction.Initialize(parameters);
            PopulationZones.Initialize();
            

            parameters.ForageNeighbors = GetResourceNeighborhood(parameters.ForageQuantityNbrRad);
            parameters.SitePrefNeighbors = GetResourceNeighborhood(parameters.SitePrefNbrRad);
            
            ModelCore.UI.WriteLine("   Opening browse log files \"{0}\" ...", parameters.LogFileName);
            eventLog = Landis.Data.CreateTextFile(parameters.LogFileName);
            eventLog.AutoFlush = true;
            eventLog.Write("Time, Zone, Population, TotalForage(kg), K, EffectivePop, DamagedSites, BiomassRemoved(g/m2), BiomassMortality(g/m2), CohortsKilled");
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                eventLog.Write(", BiomassRemoved_{0}", species.Name);
            }
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                eventLog.Write(", CohortsKilled_{0}", species.Name);
            }
            eventLog.WriteLine("");
        }

        //---------------------------------------------------------------------

        ///<summary>
        /// Run the plug-in at a particular timestep.
        ///</summary>
        public override void Run()
        {
            running = true;
            ModelCore.UI.WriteLine("Processing landscape for ungulate browse events ...");

            Event browseEvent = Event.Initiate(parameters);

            if (browseEvent != null)
                LogEvent(PlugIn.ModelCore.CurrentTime, browseEvent);
            
            //  Write site preference map 
            string path = MapNames.ReplaceTemplateVars(sitePrefMapNameTemplate, PlugIn.modelCore.CurrentTime);
            Dimensions dimensions = new Dimensions(modelCore.Landscape.Rows, modelCore.Landscape.Columns);
            using (IOutputRaster<ShortPixel> outputRaster = modelCore.CreateRaster<ShortPixel>(path, dimensions))
            {
                ShortPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites) {
                    if (site.IsActive) {
                            pixel.MapCode.Value = (short) (SiteVars.SitePreference[site] * 100);
                    }
                    else {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }

            //  Write site forage map 
            path = MapNames.ReplaceTemplateVars(siteForageMapNameTemplate, PlugIn.modelCore.CurrentTime);
            dimensions = new Dimensions(modelCore.Landscape.Rows, modelCore.Landscape.Columns);
            using (IOutputRaster<ShortPixel> outputRaster = modelCore.CreateRaster<ShortPixel>(path, dimensions))
            {
                ShortPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (short)(SiteVars.ForageQuantity[site]);
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }

            //  Write browse removed map 
            path = MapNames.ReplaceTemplateVars(biomassRemovedMapNameTemplate, PlugIn.modelCore.CurrentTime);
            dimensions = new Dimensions(modelCore.Landscape.Rows, modelCore.Landscape.Columns);
            using (IOutputRaster<ShortPixel> outputRaster = modelCore.CreateRaster<ShortPixel>(path, dimensions))
            {
                ShortPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (short)(SiteVars.TotalBrowse[site]);
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }
            //  Write site population
            path = MapNames.ReplaceTemplateVars(sitePopMapNamesTemplate, PlugIn.modelCore.CurrentTime);
            dimensions = new Dimensions(modelCore.Landscape.Rows, modelCore.Landscape.Columns);
            /*// Debugging
            StreamWriter outputCellLog;
            outputCellLog = Landis.Data.CreateTextFile("J:/LANDIS_II/code/deer-browse/trunk/tests/biomass/browse/outputCellLog.csv");
            outputCellLog.AutoFlush = true;
            outputCellLog.Write("Cell, Zone, Population");
            outputCellLog.WriteLine("");
            double totalPop = 0;
            double totalMapValues = 0;
            int totalSites = 0;
             * */
            using (IOutputRaster<ShortPixel> outputRaster = modelCore.CreateRaster<ShortPixel>(path, dimensions))
            {
                ShortPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        /*double numCells = 1;
                        if (parameters.DynamicPopulationFileName == null)
                        {
                            foreach (PopulationZone popZone in PopulationZones.Dataset)
                            {
                                if (popZone.PopulationZoneSites.Contains(site.Location))
                                {
                                    numCells = popZone.PopulationZoneSites.Count;
                                }
                            }
                        }
                         * */
                        //pixel.MapCode.Value = (short)(SiteVars.LocalPopulation[site]/numCells * 100);
                        //pixel.MapCode.Value = (short)(SiteVars.LocalPopulation[site]  * 100);
                        //totalPop += SiteVars.LocalPopulation[site];
                        // Convert to density (#/km2)
                        double popDens = SiteVars.LocalPopulation[site] / (ModelCore.CellLength * ModelCore.CellLength) * 1000 * 1000;
                        // Mult by 100 and round to integer for mapping
                        pixel.MapCode.Value = (short)(popDens * 100);

                        //totalMapValues += pixel.MapCode.Value;
                        //totalSites++;
                        /*outputCellLog.Write("{0},{1},{2}",
                                   site.Location.ToString(),
                                   SiteVars.PopulationZone[site],
                                   SiteVars.LocalPopulation[site]);
                        outputCellLog.WriteLine("");
                        */
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }
            //  Write site HSI
            path = MapNames.ReplaceTemplateVars(siteHSIMapNameTemplate, PlugIn.modelCore.CurrentTime);
            dimensions = new Dimensions(modelCore.Landscape.Rows, modelCore.Landscape.Columns);
            using (IOutputRaster<ShortPixel> outputRaster = modelCore.CreateRaster<ShortPixel>(path, dimensions))
            {
                ShortPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (short)(SiteVars.HabitatSuitability[site] * 100);
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }
        }

        //---------------------------------------------------------------------

        private void LogEvent(int   currentTime,
                              Event browseEvent)
        {
            //("Time, Zone, Population, DamagedSites,BiomassRemoved,CohortsKilled,BiomassRemoved_spp,CohortsKilled_spp)

            double totalPopulation = 0;
            double totalK = 0;
            double totalEffPop = 0;
            double totalForage = 0;

            //TODO what's going on here? If static population, then totalPoulation gets divided by number of cells (converted to density)
            if (parameters.DynamicPopulationFileName == null)
            {
                int landscapeCells = PlugIn.ModelCore.Landscape.ActiveSiteCount;
                foreach (PopulationZone popZone in PopulationZones.Dataset)
                {
                    totalPopulation += popZone.Population;
                    totalK += popZone.K;
                    totalEffPop += popZone.EffectivePop;
                    totalForage += popZone.TotalForage;
                }
                totalPopulation = totalPopulation / landscapeCells;
                totalEffPop = totalEffPop / landscapeCells;
            }
            else
            {
                foreach (PopulationZone popZone in PopulationZones.Dataset)
                {
                    totalPopulation += popZone.Population;
                    totalK += popZone.K;
                    totalEffPop += popZone.EffectivePop;
                    totalForage += popZone.TotalForage;
                }
            }

            //TODO zone -1 is not being written to log
            foreach (IPopulationZone popZone in PopulationZones.Dataset)
             //   for (int i = -1; i <= (PopulationZones.Dataset.Count-1); i++)
            {
                eventLog.Write("{0},",
                             currentTime);

                //problem happens here, because of change to the index in the loop
                //totals calculated above (Lines 293-315) should be written to zone -1
                if (popZone.Index < 0)
                {
                    eventLog.Write("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                                   "-1",
                                   totalPopulation,
                                   totalForage,
                                   totalK,
                                   totalEffPop,
                                   browseEvent.SitesDamaged,
                                   browseEvent.BiomassRemoved,
                                   browseEvent.BiomassKilled,
                                   browseEvent.CohortsKilled);
                    foreach (ISpecies species in PlugIn.ModelCore.Species)
                    {
                        eventLog.Write(",{0}", browseEvent.BiomassRemovedSpp[species.Index]);
                    }
                    foreach (ISpecies species in PlugIn.ModelCore.Species)
                    {
                        eventLog.Write(",{0}", browseEvent.CohortsKilledSpp[species.Index]);
                    }
                }
                else
                {
                    //TODO make sure this is right
                    //why is static population converted to density?
                    if (parameters.DynamicPopulationFileName == null)
                    {
                        eventLog.Write("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                                       PopulationZones.Dataset[popZone.Index].MapCode,
                                       //why is this scaled to per-cell density?
                                       (double)PopulationZones.Dataset[popZone.Index].Population / (double)PopulationZones.Dataset[popZone.Index].PopulationZoneSites.Count,
                                       //check units for forage
                                       PopulationZones.Dataset[popZone.Index].TotalForage,
                                       PopulationZones.Dataset[popZone.Index].K,
                                       PopulationZones.Dataset[popZone.Index].EffectivePop,
                                       browseEvent.ZoneSitesDamaged[popZone.Index],
                                       browseEvent.ZoneBiomassRemoved[popZone.Index],
                                       browseEvent.ZoneBiomassKilled[popZone.Index],
                                       browseEvent.ZoneCohortsKilled[popZone.Index]);
                    }
                    else
                    {
                        eventLog.Write("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                                       PopulationZones.Dataset[popZone.Index].MapCode,
                                       PopulationZones.Dataset[popZone.Index].Population,
                                       PopulationZones.Dataset[popZone.Index].TotalForage,
                                       PopulationZones.Dataset[popZone.Index].K,
                                       PopulationZones.Dataset[popZone.Index].EffectivePop,
                                       browseEvent.ZoneSitesDamaged[popZone.Index],
                                       browseEvent.ZoneBiomassRemoved[popZone.Index],
                                       browseEvent.ZoneBiomassKilled[popZone.Index],
                                       browseEvent.ZoneCohortsKilled[popZone.Index]);
                    }
                    foreach (ISpecies species in PlugIn.ModelCore.Species)
                    {
                        eventLog.Write(",{0}", browseEvent.ZoneBiomassRemovedSpp[popZone.Index][species.Index]);
                    }
                    foreach (ISpecies species in PlugIn.ModelCore.Species)
                    {
                        eventLog.Write(",{0}", browseEvent.ZoneCohortsKilledSpp[popZone.Index][species.Index]);
                    }
                }
                eventLog.WriteLine("");
            }
        }
        

        //---------------------------------------------------------------------
        //Generate a Relative Location array (with WEIGHTS) of neighbors.
        //Check each cell within a block surrounding the center point.  This will
        //create a set of POTENTIAL neighbors.  These potential neighbors
        //will need to be later checked to ensure that they are within the landscape
        // and active.

        private static IEnumerable<RelativeLocationWeighted> GetResourceNeighborhood(double neighborRadius)
        {
            float CellLength = PlugIn.ModelCore.CellLength;
            //PlugIn.ModelCore.UI.WriteLine("Creating Neighborhood List.");
            int numCellRadius = (int)(neighborRadius / CellLength);
            //PlugIn.ModelCore.UI.WriteLine("NeighborRadius={0}, CellLength={1}, numCellRadius={2}", neighborRadius, CellLength, numCellRadius);

            double centroidDistance = 0;
            double cellLength = CellLength;
            double neighborWeight = 0;

            List<RelativeLocationWeighted> neighborhood = new List<RelativeLocationWeighted>();

            for (int row = (numCellRadius * -1); row <= numCellRadius; row++)
            {
                for (int col = (numCellRadius * -1); col <= numCellRadius; col++)
                {
                    neighborWeight = 0;
                    centroidDistance = DistanceFromCenter(row, col);
                    //PlugIn.ModelCore.Log.WriteLine("Centroid Distance = {0}.", centroidDistance);
                    if (centroidDistance <= neighborRadius && centroidDistance > 0)
                    {

                        //if (parameters.ShapeOfNeighbor == NeighborShape.uniform)
                            neighborWeight = 1.0;
                        /*
                        if (parameters.ShapeOfNeighbor == NeighborShape.linear)
                        {
                            //neighborWeight = (neighborRadius - centroidDistance + (cellLength/2)) / (double) neighborRadius;
                            neighborWeight = 1.0 - (centroidDistance / (double)neighborRadius);
                        }
                        if (parameters.ShapeOfNeighbor == NeighborShape.gaussian)
                        {
                            double halfRadius = neighborRadius / 2;
                            neighborWeight = (float)
                            System.Math.Exp(-1 *
                            System.Math.Pow(centroidDistance, 2) /
                            System.Math.Pow(halfRadius, 2));
                        }
                        */
                        RelativeLocation reloc = new RelativeLocation(row, col);
                        neighborhood.Add(new RelativeLocationWeighted(reloc, neighborWeight));
                    }
                }
            }
            return neighborhood;
        }
        //-------------------------------------------------------
        //Calculate the distance from a location to a center
        //point (row and column = 0).
        private static double DistanceFromCenter(double row, double column)
        {
            double CellLength = PlugIn.ModelCore.CellLength;
            row = System.Math.Abs(row) * CellLength;
            column = System.Math.Abs(column) * CellLength;
            double aSq = System.Math.Pow(column, 2);
            double bSq = System.Math.Pow(row, 2);
            return System.Math.Sqrt(aSq + bSq);
        }
        //---------------------------------------------------------------------
        // Event handler when a cohort is killed by an age-only disturbance.
        public void CohortKilledByAgeOnlyDisturbance(object sender,
                                                     DeathEventArgs eventArgs)
        {
            // If this plug-in is not running, then some base disturbance
            // plug-in killed the cohort.
            if (!running)
                return;

        }
        //---------------------------------------------------------------------
    }

    public class RelativeLocationWeighted
    {
        private RelativeLocation location;
        private double weight;

        //---------------------------------------------------------------------
        public RelativeLocation Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }

        public double Weight
        {
            get
            {
                return weight;
            }
            set
            {
                weight = value;
            }
        }

        public RelativeLocationWeighted(RelativeLocation location, double weight)
        {
            this.location = location;
            this.weight = weight;
        }

    }

    
}
