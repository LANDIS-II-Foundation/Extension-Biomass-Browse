//  Authors:  Brian Miranda, Robert M. Scheller

using Landis.SpatialModeling;
using Landis.Library.UniversalCohorts;
using System.Collections.Generic;

namespace Landis.Extension.Browse
{
    public static class SiteVars
    {
        private static ISiteVar<double> sitePreference;
        private static ISiteVar<SiteCohorts> cohorts;
        private static ISiteVar<double> neighborhoodForage;
        private static ISiteVar<double> browseIndex;
        private static ISiteVar<double> browseDisturbance;
        private static ISiteVar<int> populationZone;
        private static ISiteVar<double> forageQuantity;
        private static ISiteVar<double> habitatSuitability;
        private static ISiteVar<double> localPopulation;
        private static ISiteVar<double> siteCapacity;
        private static ISiteVar<double> weightedBrowse;
        private static ISiteVar<double> cappedBrowse;  
        private static ISiteVar<double> remainBrowse;
        private static ISiteVar<double> totalBrowse;
        private static ISiteVar<double> biomassRemoved;
        private static ISiteVar<int> cohortsDamaged;
        //private static ISiteVar<int> ecoMaxBiomass;
        //private static ISiteVar<List<Landis.Library.BiomassCohorts.ICohort>> siteCohortList;

        //public static ISiteVar<Dictionary<int, Dictionary<int, double>>> Forage;
        //public static ISiteVar<Dictionary<int, Dictionary<int, double>>> ForageInReach;
        //public static ISiteVar<Dictionary<int, Dictionary<int, double>>> LastBrowseProportion;


        //private static ISiteVar<int> youngCohortCount;

        //---------------------------------------------------------------------

        public static void Initialize()
        {
            sitePreference = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            neighborhoodForage = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            browseIndex = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            browseDisturbance = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            populationZone = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            forageQuantity = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            habitatSuitability = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            localPopulation = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            siteCapacity = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            weightedBrowse = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            cappedBrowse = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            remainBrowse = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            totalBrowse = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            biomassRemoved = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            cohortsDamaged = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            //youngCohortCount = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            //ecoMaxBiomass = PlugIn.ModelCore.GetSiteVar<int>("Succession.MaxBiomass");

            //Forage = PlugIn.ModelCore.Landscape.NewSiteVar<Dictionary<int, Dictionary<int, double>>>();
            //ForageInReach = PlugIn.ModelCore.Landscape.NewSiteVar<Dictionary<int, Dictionary<int, double>>>();
            //LastBrowseProportion = PlugIn.ModelCore.Landscape.NewSiteVar<Dictionary<int, Dictionary<int, double>>>();

            foreach(ActiveSite site in PlugIn.ModelCore.Landscape.ActiveSites)
            {
                //Forage[site] = new Dictionary<int, Dictionary<int, double>>();
                //ForageInReach[site] = new Dictionary<int, Dictionary<int, double>>();
                //LastBrowseProportion[site] = new Dictionary<int, Dictionary<int, double>>();
            }

            cohorts = PlugIn.ModelCore.GetSiteVar<Landis.Library.UniversalCohorts.SiteCohorts>("Succession.UniversalCohorts");

            //if (biomassCohorts == null)
            //{
            //    //SF TODO throw exception if missing?
            //    PlugIn.ModelCore.UI.WriteLine("Problem getting biomassCohorts");
            //}

        }
        ////---------------------------------------------------------------------
        //public static void ReInitialize()
        //{
        //    biomassCohorts = PlugIn.ModelCore.GetSiteVar<Landis.Library.BiomassCohorts.ISiteCohorts>("Succession.BiomassCohorts");
        //}
        //---------------------------------------------------------------------
        //public static void SetForage(ICohort cohort, ActiveSite site, double new_forage)
        //{

        //    int cohortAddYear = GetAddYear(cohort);
        //    Dictionary<int, double> cohortDict;
        //    double oldForage;


        //    // If the dictionary entry exists for the cohort, overwrite it:
        //    if (Forage[site].TryGetValue(cohort.Species.Index, out cohortDict))
        //        if (cohortDict.TryGetValue(cohortAddYear, out oldForage))
        //        {
        //            Forage[site][cohort.Species.Index][cohortAddYear] = new_forage;
        //            return;
        //        }

        //    // If the dictionary does not exist for the cohort, create it:
        //    Dictionary<int, double> newEntry = new Dictionary<int, double>();
        //    newEntry.Add(cohortAddYear, new_forage);

        //    if (Forage[site].ContainsKey(cohort.Species.Index))
        //    {
        //        Forage[site][cohort.Species.Index].Add(cohortAddYear, new_forage);
        //    }
        //    else
        //    {
        //        Forage[site].Add(cohort.Species.Index, newEntry);
        //    }
        //}

        //public static void SetForageInReach(ICohort cohort, ActiveSite site, double forageInReach)
        //{
        //    int cohortAddYear = GetAddYear(cohort);
        //    Dictionary<int, double> cohortDict;
        //    double oldForageInReach;


        //    // If the dictionary entry exists for the cohort, overwrite it:
        //    if (ForageInReach[site].TryGetValue(cohort.Species.Index, out cohortDict))
        //        if (cohortDict.TryGetValue(cohortAddYear, out oldForageInReach))
        //        {
        //            //PlugIn.ModelCore.UI.WriteLine("Overwriting old forageInReach value for cohort");
        //            ForageInReach[site][cohort.Species.Index][cohortAddYear] = forageInReach; 
        //            return;
        //        }

        //    // If the dictionary does not exist for the cohort, create it:
        //    Dictionary<int, double> newEntry = new Dictionary<int, double>();
        //    newEntry.Add(cohortAddYear, forageInReach);

        //    if (ForageInReach[site].ContainsKey(cohort.Species.Index))
        //    {
        //        ForageInReach[site][cohort.Species.Index].Add(cohortAddYear, forageInReach);
        //        //PlugIn.ModelCore.UI.WriteLine("Adding new cohort"); //debug
        //    }
        //    else
        //    {
        //        //PlugIn.ModelCore.UI.WriteLine("Adding new cohort"); //debug
        //        ForageInReach[site].Add(cohort.Species.Index, newEntry);
        //    }
        //}

        //public static void SetLastBrowseProportion(ICohort cohort, ActiveSite site, double lastBrowseProportion)
        //{
        //    int cohortAddYear = GetAddYear(cohort);
        //    Dictionary<int, double> cohortDict;
        //    double oldValue;

        //    // If the dictionary entry exists for the cohort, overwrite it:
        //    if (LastBrowseProportion[site].TryGetValue(cohort.Species.Index, out cohortDict))
        //        if (cohortDict.TryGetValue(cohortAddYear, out oldValue))
        //        {
        //            LastBrowseProportion[site][cohort.Species.Index][cohortAddYear] = lastBrowseProportion;
        //            return;
        //        }

        //    // If the dictionary does not exist for the cohort, create it:
        //    Dictionary<int, double> newEntry = new Dictionary<int, double>();
        //    newEntry.Add(cohortAddYear, lastBrowseProportion);

        //    if (LastBrowseProportion[site].ContainsKey(cohort.Species.Index))
        //    {
        //        LastBrowseProportion[site][cohort.Species.Index].Add(cohortAddYear, lastBrowseProportion);
        //    }
        //    else
        //    {
        //        LastBrowseProportion[site].Add(cohort.Species.Index, newEntry);
        //    }
        //}

        //public static double GetForage(ICohort cohort, ActiveSite site)
        //{
        //    Dictionary<int, double> cohortDict;
        //    int cohortAddYear = GetAddYear(cohort);
        //    double forage = 0.0;

        //    if (Forage[site].TryGetValue(cohort.Species.Index, out cohortDict))
        //        cohortDict.TryGetValue(cohortAddYear, out forage); 
            
        //    return forage;
        //}

        //public static double GetForageInReach(ICohort cohort, ActiveSite site)
        //{
        //    Dictionary<int, double> cohortDict;
        //    int cohortAddYear = GetAddYear(cohort);
        //    double forageInReach = 0.0;

        //    if (ForageInReach[site].TryGetValue(cohort.Species.Index, out cohortDict))
        //        cohortDict.TryGetValue(cohortAddYear, out forageInReach);

        //    return forageInReach;
        //}

        //public static double GetLastBrowseProportion(ICohort cohort, ActiveSite site)
        //{
        //    Dictionary<int, double> cohortDict;
        //    int cohortAddYear = GetAddYear(cohort);
        //    double lastBrowseProportion = 0.0;

        //    if (LastBrowseProportion[site].TryGetValue(cohort.Species.Index, out cohortDict))
        //        cohortDict.TryGetValue(cohortAddYear, out lastBrowseProportion);

        //    return lastBrowseProportion;
        //}


        //---------------------------------------------------------------------
        public static ISiteVar<SiteCohorts> Cohorts
        {
            get
            {
                return Cohorts;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> SitePreference
        {
            get {
                return sitePreference;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> NeighborhoodForage
        {
            get
            {
                return neighborhoodForage;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> BrowseIndex
        {
            get
            {
                return browseIndex;
            }
        }
        //---------------------------------------------------------------------
        //public static ISiteVar<double> BrowseDisturbance
        //{
        //    get
        //    {
        //        return browseDisturbance;
        //    }
        //}
        //---------------------------------------------------------------------
        public static ISiteVar<int> PopulationZone
        {
            get
            {
                return populationZone;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> ForageQuantity
        {
            get
            {
                return forageQuantity;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> HabitatSuitability
        {
            get
            {
                return habitatSuitability;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> LocalPopulation
        {
            get
            {
                return localPopulation;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> SiteCapacity
        {
            get
            {
                return siteCapacity;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> WeightedBrowse
        {
            get
            {
                return weightedBrowse;
            }
        }
        //---------------------------------------------------------------------
        //public static ISiteVar<double> CappedBrowse
        //{
        //    get
        //    {
        //        return cappedBrowse;
        //    }
        //}
        //---------------------------------------------------------------------
        //public static ISiteVar<double> RemainBrowse
        //{
        //    get
        //    {
        //        return remainBrowse;
        //    }
        //}
        //---------------------------------------------------------------------
        /// <summary>
        /// Total Browse To Be Removed
        /// </summary>
        public static ISiteVar<double> TotalBrowse
        {
            get
            {
                return totalBrowse;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> BiomassRemoved
        {
            get
            {
                return biomassRemoved;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<int> CohortsPartiallyDamaged
        {
            get
            {
                return cohortsDamaged;
            }
        }
        //---------------------------------------------------------------------
        //public static ISiteVar<int> EcoregionMaxBiomass
        //TODO figure out how to make this species level instead of ecoregion level
        //{
        //    get
        //    {
        //        return ecoMaxBiomass;
        //    }
        //}
        //---------------------------------------------------------------------
        //public static ISiteVar<List<Landis.Library.BiomassCohorts.ICohort>> SiteCohortList
        //{
        //    get
        //    {
        //        return siteCohortList;
        //    }
        //}
        //---------------------------------------------------------------------
        public static int GetAddYear(ICohort cohort)
        {
            int currentYear = PlugIn.ModelCore.CurrentTime;
            int cohortAddYear = currentYear - cohort.Data.Age;
            return cohortAddYear;
        }

        //---------------------------------------------------------------------
        /*public static ISiteVar<int> YoungCohortCount
        {
            get
            {
                return youngCohortCount;
            }
        }
        */
        //---------------------------------------------------------------------
    }
}
