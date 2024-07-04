//  Authors:  Brian Miranda, Nate De Jager, Patrick Drohan, Robert Scheller

using Landis.SpatialModeling;
using Landis.Core;
using System.Collections.Generic;
using Landis.Library.UniversalCohorts;


namespace Landis.Extension.Browse
{
    /// <summary>
    /// A biomass disturbance that handles partial thinning of cohorts.
    /// </summary>
    public class BrowseDisturbance 
        : IDisturbance
    {
        private static BrowseDisturbance singleton;
        //private static IDictionary<ushort, int>[] reductions;
        private static IDictionary<ushort, double>[] reductions;
        //private static IDictionary<ushort, int>[] forageDictionary;
        //private static IDictionary<ushort, int>[] forageInReachDictionary;
        //private static IDictionary<ushort, double>[] lastBrowsePropDictionary;
        private static ActiveSite currentSite;

        //---------------------------------------------------------------------
        ActiveSite IDisturbance.CurrentSite
        {
            get
            {
                return currentSite;
            }
        }

        //---------------------------------------------------------------------
        ExtensionType IDisturbance.Type
        {
            get
            {
                return PlugIn.ExtType;
            }
        }

        //---------------------------------------------------------------------
        static BrowseDisturbance()
        {
            singleton = new BrowseDisturbance();
        }

        //---------------------------------------------------------------------
        public BrowseDisturbance()
        {
        }

        //---------------------------------------------------------------------
        int IDisturbance.ReduceOrKillMarkedCohort(ICohort cohort)
        {
            double reduction;
            
            if (reductions[cohort.Species.Index].TryGetValue(cohort.Data.Age, out reduction))
            {
                //PlugIn.ModelCore.UI.WriteLine("Reduction = {0}", reduction); //debug
                SiteVars.BiomassRemoved[currentSite] += reduction;
                SiteVars.CohortsPartiallyDamaged[currentSite]++;

                //TODO SF does using an int here cause problems?
                return (int) reduction;
            }
            else
                return 0;
        }

        public static void Initialize()
        {
            reductions = new IDictionary<ushort, double>[PlugIn.ModelCore.Species.Count];
            for (int i = 0; i < reductions.Length; i++)
                reductions[i] = new Dictionary<ushort, double>();
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Reduces the biomass of cohorts that have been marked for partial
        /// reduction.
        /// </summary>
        public static void ReduceCohortBiomass(ActiveSite site)
        {

            currentSite = site;

            SiteVars.Cohorts[site].ReduceOrKillCohorts(singleton); // Original

        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Records the biomass reduction for a particular cohort.
        /// Reductions are stored here, and then later called by ReduceCohortBiomass to translate to the 
        /// succession extension. 
        /// </summary>
        public static void RecordBiomassReduction(ICohort cohort,
                                                  double reduction)
        {
            //PlugIn.ModelCore.UI.WriteLine("Recording reduction:  {0:0.0}/{1:0.0}/{2}.", cohort.Species.Name, cohort.Age, reduction);//debug
            reductions[cohort.Species.Index][cohort.Data.Age] = reduction;
        }
    }
}

