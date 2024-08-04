//  Authors:  Brian Miranda, Nate De Jager, Patrick Drohan, Robert Scheller

using System;
using System.Collections.Generic;
using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.UniversalCohorts;


namespace Landis.Extension.Browse
{
    public class Event
    {

        private int sitesDamaged;
        private int cohortsKilled;
        private double biomassRemoved;
        private double biomassKilled;
        private int population;

        private int[] cohortsKilledSpp;
        private double[] biomassRemovedSpp;

        private int[] zoneSitesDamaged;
        private int[] zoneCohortsKilled;
        private double[] zoneBiomassKilled;
        private double[] zoneBiomassRemoved;
        private int[] zonePopulation;
        private int[][] zoneCohortsKilledSpp;
        private double[][] zoneBiomassRemovedSpp;
        private double[][] zoneBiomassBrowsedSpp;
        private double[][] zoneForageSpp;
        private double[][] zoneForageInReachSpp;


        //---------------------------------------------------------------------

        static Event()
        {

        }

        //---------------------------------------------------------------------
        public int SitesDamaged
        {
            get
            {
                return sitesDamaged;
            }
        }
        //---------------------------------------------------------------------
        public int[] ZoneSitesDamaged
        {
            get
            {
                return zoneSitesDamaged;
            }
        }
        //---------------------------------------------------------------------
        public int CohortsKilled
        {
            get
            {
                return cohortsKilled;
            }
        }
        //---------------------------------------------------------------------
        public int[] ZoneCohortsKilled
        {
            get
            {
                return zoneCohortsKilled;
            }
        }
        //---------------------------------------------------------------------
        public double BiomassKilled
        {
            get
            {
                return biomassKilled;
            }
        }
        //---------------------------------------------------------------------
        public double BiomassRemoved
        {
            get
            {
                return biomassRemoved;
            }
        }
        //---------------------------------------------------------------------
        public double[] ZoneBiomassKilled
        {
            get
            {
                return zoneBiomassKilled;
            }
        }
        //---------------------------------------------------------------------
        public double[] ZoneBiomassRemoved
        {
            get
            {
                return zoneBiomassRemoved;
            }
        }
        //---------------------------------------------------------------------
        public int Population
        {
            get
            {
                return population;
            }
        }
        //---------------------------------------------------------------------
        public int[] ZonePopulation
        {
            get
            {
                return zonePopulation;
            }
        }
        //---------------------------------------------------------------------
        public int[] CohortsKilledSpp
        {
            get
            {
                return cohortsKilledSpp;
            }
        }
        //---------------------------------------------------------------------
        public int[][] ZoneCohortsKilledSpp
        {
            get
            {
                return zoneCohortsKilledSpp;
            }
        }
        //---------------------------------------------------------------------
        public double[] BiomassRemovedSpp
        {
            get
            {
                return biomassRemovedSpp;
            }
        }
        //---------------------------------------------------------------------
        public double[][] ZoneBiomassRemovedSpp
        {
            get
            {
                return zoneBiomassRemovedSpp;
            }
        }
        //---------------------------------------------------------------------
        public double[][] ZoneBiomassBrowsedSpp
        {
            get
            {
                return zoneBiomassBrowsedSpp;
            }
        }
        //---------------------------------------------------------------------
        public double[][] ZoneForageSpp
        {
            get
            {
                return zoneForageSpp;
            }
        }
        //---------------------------------------------------------------------
        public double[][] ZoneForageInReachSpp
        {
            get
            {
                return zoneForageInReachSpp;
            }
        }
        //---------------------------------------------------------------------
        public static void Initialize()
        {

        }

        //---------------------------------------------------------------------

        public static Event Initiate(IInputParameters parameters)
        {
            Event browseEvent = new Event();

            //Calculate forage for each site
            browseEvent.CalculateForage(parameters);

            // Calculate population
            if (PlugIn.UseBDI)
            {
                PopulationZones.CalculatePopulationFromBDI(parameters);
            }
            else
            {
                PopulationZones.CalculatePopulation(parameters);
            }

            // Calculate HSI
            HabitatSuitability.CalculateHSI(parameters);

            // Calculate site-level population
            PopulationZones.CalculateLocalPopulation(parameters);

            //Calculate Browse to remove
            PopulationZones.CalculateBrowseToRemove(parameters);

            browseEvent.DisturbSites(parameters);

            return browseEvent;
        }


        //---------------------------------------------------------------------

        private Event()
        {
            this.sitesDamaged = 0;
            this.population = 0;
            this.cohortsKilled = 0;
            this.biomassRemoved = 0;
            this.biomassKilled = 0;
            this.cohortsKilledSpp = new int[PlugIn.ModelCore.Species.Count];
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                this.cohortsKilledSpp[species.Index] = 0;
            }
            //this.biomassRemovedSpp = new int[PlugIn.ModelCore.Species.Count];
            this.biomassRemovedSpp = new double[PlugIn.ModelCore.Species.Count];
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                this.biomassRemovedSpp[species.Index] = 0;
            }
            this.zoneSitesDamaged = new int[PopulationZones.Dataset.Count];
            this.zonePopulation = new int[PopulationZones.Dataset.Count];
            this.zoneCohortsKilled = new int[PopulationZones.Dataset.Count];
            this.zoneBiomassRemoved = new double[PopulationZones.Dataset.Count];
            this.zoneBiomassKilled = new double[PopulationZones.Dataset.Count];
            this.zoneCohortsKilledSpp = new int[PopulationZones.Dataset.Count][];
            this.zoneBiomassRemovedSpp = new double[PopulationZones.Dataset.Count][];
            this.zoneBiomassBrowsedSpp = new double[PopulationZones.Dataset.Count][];
            this.zoneForageSpp = new double[PopulationZones.Dataset.Count][];
            this.zoneForageInReachSpp = new double[PopulationZones.Dataset.Count][];

            foreach (IPopulationZone popZone in PopulationZones.Dataset)

            {
                this.zoneSitesDamaged[popZone.Index] = 0;
                this.zonePopulation[popZone.Index] = 0;
                this.zoneCohortsKilled[popZone.Index] = 0;
                this.zoneBiomassRemoved[popZone.Index] = 0;
                this.zoneBiomassKilled[popZone.Index] = 0;
                this.zoneCohortsKilledSpp[popZone.Index] = new int[PlugIn.ModelCore.Species.Count];
                this.zoneBiomassRemovedSpp[popZone.Index] = new double[PlugIn.ModelCore.Species.Count];
                this.zoneBiomassBrowsedSpp[popZone.Index] = new double[PlugIn.ModelCore.Species.Count];
                this.zoneForageSpp[popZone.Index] = new double[PlugIn.ModelCore.Species.Count];
                this.zoneForageInReachSpp[popZone.Index] = new double[PlugIn.ModelCore.Species.Count];

                foreach (ISpecies species in PlugIn.ModelCore.Species)
                {
                    this.zoneCohortsKilledSpp[popZone.Index][species.Index] = 0;
                    this.zoneBiomassRemovedSpp[popZone.Index][species.Index] = 0;
                    this.zoneBiomassBrowsedSpp[popZone.Index] = new double[PlugIn.ModelCore.Species.Count];
                    this.zoneForageSpp[popZone.Index][species.Index] = 0;
                    this.zoneForageInReachSpp[popZone.Index][species.Index] = 0;
                }
            }
        }
        //---------------------------------------------------------------------
        //Go through all active sites and damage them according to the
        // site's browse to be removed.
        private void DisturbSites(IInputParameters parameters)
        {
            PlugIn.ModelCore.UI.WriteLine("   Disturbing Sites");
            foreach (IPopulationZone popZone in PopulationZones.Dataset)
            {

                foreach (Location siteLocation in PopulationZones.Dataset[popZone.Index].PopulationZoneSites)
                {
                    double siteTotalRemoval = 0;
                    ActiveSite site = (ActiveSite)PlugIn.ModelCore.Landscape.GetSite(siteLocation);

                    double siteTotalToBrowse = SiteVars.TotalBrowse[site];
                    double priorForageInReach; 

                    //PlugIn.ModelCore.UI.WriteLine(" Allocating browse for site {0}; total browse: {1}", site.DataIndex, siteTotalToBrowse); //debug

                    //Browse - allocate browse to cohorts
                    if (siteTotalToBrowse > 0)
                    {
                        double forageRemoved = 0.0;

                        //Browse - compile browse by preference class
                        double[] forageByPrefClass = new double[parameters.PreferenceList.Count];

                        //Browse - calculate first pass removal
                        // first pass removes forage at rate equal to preference
                        //double[] firstPassRemovalList = new double[cohortCount];
                        double firstPassRemoval = 0;
                        foreach (ISpecies species in PlugIn.ModelCore.Species)
                        {
                            ISpeciesCohorts cohortList = SiteVars.Cohorts[site][species];
                            if (cohortList != null)
                            {
                                foreach (ICohort cohort in cohortList)
                                {

                                    //Calculate how much forage is available using just browse preference and available forage
                                    // (without ranking species)
                                    priorForageInReach = cohort.Data.AdditionalParameters.ForageInReach;

                                    ISppParameters sppParms = parameters.SppParameters[cohort.Species.Index];
                                    double browsePref = sppParms.BrowsePref;

                                    double availForage = cohort.Data.AdditionalParameters.ForageInReach; // SiteVars.GetForageInReach(cohort, site);
                                    firstPassRemoval += availForage * browsePref;
                                    //if (PlugIn.Calibrate)
                                    //    CalibrateLog.SetCalibrateData(cohort, 2, firstPassRemoval);

                                    //PlugIn.ModelCore.UI.WriteLine("{0:0.0}/{1:0.0}. availForage = {2}, browsePref = {3}, " +
                                    //    "firstPass increment = {4}, firstPassRemoval = {5}",
                                    //    cohort.Species.Name, cohort.Age, availForage, browsePref, availForage * browsePref, firstPassRemoval);//debug
                                    //assign first pass removal to each cohort
                                    cohort.Data.AdditionalParameters.BiomassRemoval = availForage * browsePref;

                                    int prefIndex = 0;
                                    foreach (double prefValue in parameters.PreferenceList)
                                    {
                                        //calculate how much forage is leftover for the cohort, and add it to a list
                                        // of leftover forage for each browse preference value (for later ranking)
                                        if (browsePref == prefValue)
                                        {
                                            forageByPrefClass[prefIndex] += (availForage - (availForage * browsePref));
                                            break;
                                        }
                                        prefIndex++;
                                    }

                                }
                            }
                        }

                        // First pass adjustment
                        // if first pass exceeds removal then adjust downward for each cohort
                        //double[] remainingBrowseList = new double[cohortCount];
                        double adjFirstPassRemoval = firstPassRemoval;
                        if (firstPassRemoval > siteTotalToBrowse)
                        {
                            adjFirstPassRemoval = 0;
                            foreach (ISpecies species in PlugIn.ModelCore.Species)
                            {
                                ISpeciesCohorts cohortList = SiteVars.Cohorts[site][species];
                                if (cohortList != null)
                                {
                                    foreach (ICohort cohort in cohortList)
                                    {
                                        cohort.Data.AdditionalParameters.BiomassRemoval *= siteTotalToBrowse / firstPassRemoval;
                                        adjFirstPassRemoval += cohort.Data.AdditionalParameters.BiomassRemoval;
                                    }
                                }

                            }
                        }
                        double unallocatedBrowse = siteTotalToBrowse - adjFirstPassRemoval;

                        //Browse - allocate second pass of browse removal to cohorts
                        //second pass removes all of most preferred before moving down in preference
                        forageRemoved = adjFirstPassRemoval;
                        int prefLoop = 0;
                        foreach (double prefValue in parameters.PreferenceList)
                        { //loop over each preference value
                            double prefClassRemoved = 0;
                            foreach (ISpecies species in PlugIn.ModelCore.Species)
                            {
                                ISpeciesCohorts cohortList = SiteVars.Cohorts[site][species];
                                if (cohortList != null)
                                {
                                    foreach (ICohort cohort in cohortList)
                                    {
                                        ISppParameters sppParms = parameters.SppParameters[cohort.Species.Index];
                                        double browsePref = sppParms.BrowsePref;

                                        if (browsePref == prefValue)
                                        {//only do this if the cohort has matching preference value
                                            double finalRemoval = cohort.Data.AdditionalParameters.BiomassRemoval;
                                            if (forageRemoved < siteTotalToBrowse)
                                            {
                                                double availForage = cohort.Data.AdditionalParameters.ForageInReach; // SiteVars.GetForageInReach(cohort, site);
                                                double prefClassForage = forageByPrefClass[prefLoop];
                                                double secondPassRemoval = 0;
                                                if (prefClassForage > 0)
                                                {
                                                    //site-level unallocated browse multiplied by remaining cohort forage / preference class remaining forage,
                                                    //so it's proportionally assigned to each cohort
                                                    secondPassRemoval = unallocatedBrowse * ((availForage - cohort.Data.AdditionalParameters.BiomassRemoval) / prefClassForage);
                                                    secondPassRemoval = Math.Min(secondPassRemoval, (availForage - cohort.Data.AdditionalParameters.BiomassRemoval)); // adjFirstPassRemovalList[cohortLoop]));
                                                }

                                                cohort.Data.AdditionalParameters.BiomassRemoval = secondPassRemoval;

                                                //if (PlugIn.Calibrate)
                                                //    CalibrateLog.SetCalibrateData(cohort, 3, secondPassRemoval);

                                                finalRemoval += secondPassRemoval; //cohort-level, add first and second pass removal. 
                                                forageRemoved -= cohort.Data.AdditionalParameters.BiomassRemoval;
                                                forageRemoved += finalRemoval;
                                                prefClassRemoved += (finalRemoval - cohort.Data.AdditionalParameters.BiomassRemoval);
                                                //PlugIn.ModelCore.UI.WriteLine("{0:0.0}/{1:0.0}. adjusted firstPassRemoval = {2}, " +
                                                //    "secondPassRemoval = {3}, finalRemoval = {4}", cohort.Species.Name, cohort.Age,
                                                //    adjFirstPassRemovalList[cohortLoop], secondPassRemoval, finalRemoval); //debug
                                            }

                                            //if (PlugIn.Calibrate)
                                            //    CalibrateLog.SetCalibrateData(cohort, 4, finalRemoval);

                                            this.biomassRemoved += (double)finalRemoval;
                                            this.zoneBiomassRemoved[popZone.Index] += (double)finalRemoval;
                                            this.zoneBiomassBrowsedSpp[popZone.Index][cohort.Species.Index] += (double)finalRemoval;
                                            siteTotalRemoval += finalRemoval;

                                            double propBrowse = 0.0;
                                            if (cohort.Data.AdditionalParameters.Forage > 0)
                                                propBrowse = finalRemoval / cohort.Data.AdditionalParameters.Forage;
                                            cohort.Data.AdditionalParameters.ProportionBrowse = propBrowse;

                                            if (propBrowse < -0.0001 || propBrowse > 1.0001)
                                                PlugIn.ModelCore.UI.WriteLine("   Browse Proportion not between 0 and 1: {0}. finalRemoval = {1}," +
                                                    "total forage = {2}. \r\n    Error encountered for site {3} (row {4}, column {5}), cohort {6:0.0}/{7:0.0}.",
                                                    propBrowse, finalRemoval, cohort.Data.AdditionalParameters.Forage, site.DataIndex, site.Location.Row, site.Location.Column,
                                                    cohort.Species.Name, cohort.Data.Age);

                                            if (propBrowse > 1.0001)
                                                propBrowse = 1;

                                            //LastBrowseProportion is only used for GrowthReduction
                                            if (propBrowse > 0.0)
                                                //PlugIn.ModelCore.UI.WriteLine("Setting LastBrowseProportion :  {0:0.0}/{1:0.0}/{2}.", cohort.Species.Name, cohort.Age, propBrowse); //debug
                                                cohort.Data.AdditionalParameters.BrowseProportion = propBrowse;

                                            // Add mortality
                                            // Browse - Mortality caused by browsing
                                            if (parameters.Mortality)
                                            {
                                                double mortThresh = sppParms.MortThresh;
                                                double mortMax = sppParms.MortMax;
                                                double mortProb = CalculateReduction(mortThresh, mortMax, propBrowse);
                                                PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = 0.0;
                                                PlugIn.ModelCore.ContinuousUniformDistribution.Beta = 1.0;
                                                double myRand = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();

                                                if (myRand < mortProb)
                                                {
                                                    //int biomassKilled = cohort.Biomass - (int)finalRemoval;
                                                    double biomassKilled = (double)cohort.Data.Biomass - (double)finalRemoval;
                                                    finalRemoval = (double)cohort.Data.Biomass;
                                                    this.biomassKilled += biomassKilled;
                                                    this.cohortsKilled += 1;
                                                    this.cohortsKilledSpp[cohort.Species.Index] += 1;
                                                    this.zoneCohortsKilled[popZone.Index] += 1;
                                                    this.zoneCohortsKilledSpp[popZone.Index][cohort.Species.Index] += 1;
                                                }
                                            }
                                            if (finalRemoval > 0)
                                            {
                                                //PlugIn.ModelCore.UI.WriteLine("Recording cohort biomass removal :  {0:0.0}/{1:0.0}/{2}.",
                                                //    cohort.Species.Name, cohort.Age, finalRemoval); //debug
                                                BrowseDisturbance.RecordBiomassReduction(cohort, finalRemoval);
                                                this.biomassRemovedSpp[cohort.Species.Index] += finalRemoval;
                                                this.zoneBiomassKilled[popZone.Index] += finalRemoval;
                                                this.zoneBiomassRemovedSpp[popZone.Index][cohort.Species.Index] += finalRemoval;
                                            }
                                        }
                                    }
                                }
                            }
                            unallocatedBrowse -= prefClassRemoved;
                            prefLoop++;
                        }
                        this.sitesDamaged += 1;
                        this.zoneSitesDamaged[popZone.Index] += 1;


                    }

                    // Send biomass reduction to the succession extensions
                    BrowseDisturbance.ReduceCohortBiomass(site);

                    //add species forage per zone for log file
                    foreach (ISpecies species in PlugIn.ModelCore.Species)
                    {
                        ISpeciesCohorts cohortList = SiteVars.Cohorts[site][species];
                        if (cohortList != null)
                        {
                            foreach (ICohort cohort in cohortList)
                            {
                                this.zoneForageSpp[popZone.Index][cohort.Species.Index] += cohort.Data.AdditionalParameters.Forage;//SiteVars.GetForage(cohort, site);
                                this.zoneForageInReachSpp[popZone.Index][cohort.Species.Index] += cohort.Data.AdditionalParameters.ForageInReach; // SiteVars.GetForageInReach(cohort, site);
                            }
                        }
                    }


                }
            }
        }

        //---------------------------------------------------------------------
        public static double CalculateReduction(double threshold, double max, double propBrowse)
        {
            //calculate growth reduction
            double reduction = 0;
            if (propBrowse > threshold)
            {
                reduction = (max / (1.0 - threshold)) * propBrowse - threshold * (max / (1.0 - threshold));
            }
            return reduction;

        }

        //---------------------------------------------------------------------
        private void CalculateForage(IInputParameters parameters)
        {
            // Update available forage by cohort
            PlugIn.ModelCore.UI.WriteLine("   Calculating Site Preference & Forage.");
            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                // SF: removed ecoregion biomassMax, which was originally imported from Biomass Succession.
                // Now using the BiomassMax for each species as given in the Browse input files. 
                // This value is used in two ways: it sets a minBiomassThreshold below which species
                // may be entirely foraged, and it is also used to calculate the maximum forage per site before the 
                // species escapes browse. This is meant to represent the maximum potential biomass at the site,
                // but the current way it's calculated is unsatisfying as it depends on which species are present
                // at the site. For example, if a shrub cohort is at a site, a tree with a higher max biomass arriving
                // at the site would increase the minBiomassThreshold and increase the amount of foraging the shrub
                // is subject to. 
                //TODO: consider replacing with a map?

                double siteBiomassMax = 0;

                foreach (ISpecies species in PlugIn.ModelCore.Species)
                {
                    ISppParameters sppParms = parameters.SppParameters[species.Index];
                    double browsePref = sppParms.BrowsePref;

                    ISpeciesCohorts cohortList = SiteVars.Cohorts[site][species];

                    if (cohortList != null)
                    {
                        foreach (ICohort cohort in cohortList)
                        //for each cohort, get the new forage (Biomass*0.04*proportion of ANPP that is forage)
                        //total forage will later be reduced to represent forage in reach of browsers
                        {
                            double newForage = 0;
                            //PlugIn.ModelCore.UI.WriteLine("     browsePref = {0}", browsePref); //debug
                            if ((browsePref > 0) || (parameters.CountNonForage))
                            {

                                double growthReduction  = GrowthReduction.ReduceCohortGrowth(cohort, site);

                                newForage = (cohort.Data.ANPP) * parameters.ANPPForageProp * (1-growthReduction);
                                //newForage = cohort.Data.Biomass * 0.04;

                                if (cohort.Data.Age == 1)
                                {
                                    if (parameters.UseInitBiomass)
                                    {
                                        newForage = cohort.Data.ANPP * parameters.ANPPForageProp; 
                                    }
                                    else
                                    {
                                        newForage = 0; 
                                    }
                                }

                                //if (PlugIn.Calibrate)
                                //    CalibrateLog.SetCalibrateData(cohort, 5, newForage);

                            }

                            //we also need to update the site max biomass if a species with a high value is present
                            //TODO SF why?
                            siteBiomassMax = Math.Max(siteBiomassMax, sppParms.BiomassMax);
                            //record original forage
                            cohort.Data.AdditionalParameters.Forage = newForage;
                        }
                    }
                }

                List<ICohort> siteCohortList = new List<ICohort>();


                foreach (ISpecies species in PlugIn.ModelCore.Species)
                //Build a list of cohorts at the site
                {
                    ISpeciesCohorts cohortList = SiteVars.Cohorts[site][species];

                    if (cohortList != null)
                    {
                        foreach (ICohort cohort in cohortList)
                        {
                            siteCohortList.Add(cohort);

                        }
                    }

                }

                List<double> propInReachList = new List<double>(siteCohortList.Count);
                propInReachList = Forage.CalculateCohortPropInReach(siteCohortList, parameters, siteBiomassMax);

                int listCount = 0;
                foreach (ICohort cohort in siteCohortList)
                {
                    double newForageinReach = cohort.Data.AdditionalParameters.Forage * propInReachList[listCount]; 
                    //SF need to set this every time for each cohort, so that cohorts that escape browse have forageInReach = 0 
                    //instead of staying set at their previous ForageInReach value
                    cohort.Data.AdditionalParameters.ForageInReach = newForageinReach;

                    listCount++;
                }

                //  Calculate Site BrowsePreference and ForageQuantity
                SitePreference.CalcSiteForage(parameters, site);
            }
        }

    }
}
