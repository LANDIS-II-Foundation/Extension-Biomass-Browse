using Landis.Library.BiomassCohorts;
using Landis.SpatialModeling;
using Landis.Core;
using System.Collections.Generic;

namespace Landis.Extension.DeerBrowse
{
    /// <summary>
    /// A biomass disturbance that handles partial thinning of cohorts.
    /// </summary>
    public class PartialDisturbance
        : IDisturbance
    {
        private static PartialDisturbance singleton;
        private static IDictionary<ushort, int>[] reductions;
        private static IDictionary<ushort, int>[] forageDictionary;
        private static IDictionary<ushort, int>[] forageInReachDictionary;
        private static IDictionary<ushort, double>[] lastBrowsePropDictionary;
        private static ActiveSite currentSite;

        //---------------------------------------------------------------------
        ActiveSite Landis.Library.BiomassCohorts.IDisturbance.CurrentSite
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
                return PlugIn.Type;
            }
        }

        //---------------------------------------------------------------------
        static PartialDisturbance()
        {
            singleton = new PartialDisturbance();
        }

        //---------------------------------------------------------------------
        public PartialDisturbance()
        {
        }

        //---------------------------------------------------------------------
        int IDisturbance.ReduceOrKillMarkedCohort(ICohort cohort)
        {
            int reduction;
            if (reductions[cohort.Species.Index].TryGetValue(cohort.Age, out reduction))
            {

                SiteVars.BiomassRemoved[currentSite] += reduction;
                SiteVars.CohortsPartiallyDamaged[currentSite]++;

                return reduction;
            }
            else
            return 0;
        }
        //---------------------------------------------------------------------

        public static void Initialize()
        {
            reductions = new IDictionary<ushort, int>[PlugIn.ModelCore.Species.Count];
            for (int i = 0; i < reductions.Length; i++)
                reductions[i] = new Dictionary<ushort, int>();

            forageDictionary = new IDictionary<ushort, int>[PlugIn.ModelCore.Species.Count];
            for (int i = 0; i < forageDictionary.Length; i++)
                forageDictionary[i] = new Dictionary<ushort, int>();

            forageInReachDictionary = new IDictionary<ushort, int>[PlugIn.ModelCore.Species.Count];
            for (int i = 0; i < forageInReachDictionary.Length; i++)
                forageInReachDictionary[i] = new Dictionary<ushort, int>();

            lastBrowsePropDictionary = new IDictionary<ushort, double>[PlugIn.ModelCore.Species.Count];
            for (int i = 0; i < lastBrowsePropDictionary.Length; i++)
                lastBrowsePropDictionary[i] = new Dictionary<ushort, double>();
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Reduces the biomass of cohorts that have been marked for partial
        /// reduction.
        /// </summary>
        public static void ReduceCohortBiomass(ActiveSite site)
        {
            currentSite = site;

            //PlugIn.ModelCore.Log.WriteLine("ReducingCohortBiomass NOW!");
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                SiteVars.BiomassCohorts[site].ReduceOrKillBiomassCohorts(singleton); // Original

            }

        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Records the biomass reduction for a particular cohort.
        /// </summary>
        public static void RecordBiomassReduction(ICohort cohort,
                                                  int reduction)
        {
            //PlugIn.ModelCore.Log.WriteLine("Recording reduction:  {0:0.0}/{1:0.0}/{2}.", cohort.Species.Name, cohort.Age, reduction);
            reductions[cohort.Species.Index][cohort.Age] = reduction;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Records the forage for a particular cohort.
        /// </summary>
        public static void RecordForage(ICohort cohort,
                                                  int forage)
        {
            //PlugIn.ModelCore.Log.WriteLine("Recording reduction:  {0:0.0}/{1:0.0}/{2}.", cohort.Species.Name, cohort.Age, reduction);
            forageDictionary[cohort.Species.Index][cohort.Age] = forage;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Records the forage in reach for a particular cohort.
        /// </summary>
        public static void RecordForageInReach(ICohort cohort,
                                                  int forageInReach)
        {
            //PlugIn.ModelCore.Log.WriteLine("Recording reduction:  {0:0.0}/{1:0.0}/{2}.", cohort.Species.Name, cohort.Age, reduction);
            forageInReachDictionary[cohort.Species.Index][cohort.Age] = forageInReach;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Records the last browse proportion for a particular cohort.
        /// </summary>
        public static void RecordLastBrowseProp(ICohort cohort,
                                                  double lastBrowseProp)
        {
            //PlugIn.ModelCore.Log.WriteLine("Recording reduction:  {0:0.0}/{1:0.0}/{2}.", cohort.Species.Name, cohort.Age, reduction);
            lastBrowsePropDictionary[cohort.Species.Index][cohort.Age] = lastBrowseProp;
        }
        //---------------------------------------------------------------------
        int IDisturbance.ChangeForage(ICohort cohort)
        {
            int forage;
            if (forageDictionary[cohort.Species.Index].TryGetValue(cohort.Age, out forage))
            {
                return forage;
            }
            else
                return 0;
        }
        //---------------------------------------------------------------------
        public static void UpdateForage(ActiveSite site)
        {
            currentSite = site;

            //PlugIn.ModelCore.Log.WriteLine("ReducingCohortBiomass NOW!");
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                SiteVars.BiomassCohorts[site].UpdateForage(singleton);
            }
        }
        //---------------------------------------------------------------------
        int IDisturbance.ChangeForageInReach(ICohort cohort)
        {
            int forageInReach;
            if (forageInReachDictionary[cohort.Species.Index].TryGetValue(cohort.Age, out forageInReach))
            {
                return forageInReach;
            }
            else
                return 0;
        }
        //---------------------------------------------------------------------

        public static void UpdateForageInReach(ActiveSite site)
        {
            currentSite = site;

            //PlugIn.ModelCore.Log.WriteLine("ReducingCohortBiomass NOW!");
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                SiteVars.BiomassCohorts[site].UpdateForageInReach(singleton);
            }
        }
        //---------------------------------------------------------------------
        double IDisturbance.ChangeLastBrowseProp(ICohort cohort)
        {
            double lastBrowseProp;
            if (lastBrowsePropDictionary[cohort.Species.Index].TryGetValue(cohort.Age, out lastBrowseProp))
            {
                return lastBrowseProp;
            }
            else
                return 0;
        }
        //---------------------------------------------------------------------
        public static void UpdateLastBrowseProp(ActiveSite site)
        {
            currentSite = site;

            //PlugIn.ModelCore.Log.WriteLine("ReducingCohortBiomass NOW!");
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                SiteVars.BiomassCohorts[site].UpdateLastBrowseProp(singleton);
            }
        }
        //---------------------------------------------------------------------
    }
}
