//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;
//using Landis.Library.AgeOnlyCohorts;
using System.Collections.Generic;

namespace Landis.Extension.DeerBrowse
{
    public static class SiteVars
    {
        private static ISiteVar<double> sitePreference;
        //private static ISiteVar<Landis.Library.AgeOnlyCohorts.ISiteCohorts> ageCohorts;
        private static ISiteVar<Landis.Library.BiomassCohorts.ISiteCohorts> biomassCohorts;
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
        private static ISiteVar<int> biomassRemoved;
        private static ISiteVar<int> cohortsDamaged;
        private static ISiteVar<int> ecoMaxBiomass;
        private static ISiteVar<List<Landis.Library.BiomassCohorts.ICohort>> siteCohortList;


        //private static ISiteVar<int> youngCohortCount;
        
        //---------------------------------------------------------------------

        public static string Initialize()
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
            biomassRemoved = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            cohortsDamaged = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            //youngCohortCount = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            ecoMaxBiomass = PlugIn.ModelCore.GetSiteVar<int>("Succession.EcoregionMaxBiomass");
            siteCohortList = PlugIn.ModelCore.Landscape.NewSiteVar<List<Landis.Library.BiomassCohorts.ICohort>>();

            biomassCohorts = PlugIn.ModelCore.GetSiteVar<Landis.Library.BiomassCohorts.ISiteCohorts>("Succession.BiomassCohorts");

            if (biomassCohorts == null)
            {
                //ageCohorts = PlugIn.ModelCore.GetSiteVar<Landis.Library.AgeOnlyCohorts.ISiteCohorts>("Succession.AgeCohorts");
                return "AgeOnly";
            }
            else
            {
                return "Biomass";
            }
          

        }
        //---------------------------------------------------------------------
        public static void ReInitialize()
        {
            biomassCohorts = PlugIn.ModelCore.GetSiteVar<Landis.Library.BiomassCohorts.ISiteCohorts>("Succession.BiomassCohorts");
            //ageCohorts = PlugIn.ModelCore.GetSiteVar<Landis.Library.AgeOnlyCohorts.ISiteCohorts>("Succession.AgeCohorts");
        }
        //---------------------------------------------------------------------
        /*public static ISiteVar<Landis.Library.AgeOnlyCohorts.ISiteCohorts> AgeCohorts
        {
            get
            {
                return ageCohorts;
            }
        }
         * */
        //---------------------------------------------------------------------
        public static ISiteVar<Landis.Library.BiomassCohorts.ISiteCohorts> BiomassCohorts
        {
            get
            {
                return biomassCohorts;
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
        public static ISiteVar<double> BrowseDisturbance
        {
            get
            {
                return browseDisturbance;
            }
        }
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
        public static ISiteVar<double> CappedBrowse
        {
            get
            {
                return cappedBrowse;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<double> RemainBrowse
        {
            get
            {
                return remainBrowse;
            }
        }
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
        public static ISiteVar<int> BiomassRemoved
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
        public static ISiteVar<int> EcoregionMaxBiomass
        {
            get
            {
                return ecoMaxBiomass;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<List<Landis.Library.BiomassCohorts.ICohort>> SiteCohortList
        {
            get
            {
                return siteCohortList;
            }
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
