//  Authors:  Brian Miranda, Nate De Jager, Patrick Drohan, Robert Scheller

using System;
using System.Collections.Generic;
using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;


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


        //---------------------------------------------------------------------

        static Event()
        {
           
        }

        //---------------------------------------------------------------------
        public int SitesDamaged
        {
            get {
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
            get {
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
            get {
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
                PopulationZones.CalculatePopulation(parameters);

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
            //this.zoneBiomassRemoved = new int[PopulationZones.Dataset.Count];
            this.zoneBiomassRemoved = new double[PopulationZones.Dataset.Count];
            //this.zoneBiomassKilled = new int[PopulationZones.Dataset.Count];
            this.zoneBiomassKilled = new double[PopulationZones.Dataset.Count];
            this.zoneCohortsKilledSpp = new int[PopulationZones.Dataset.Count][];
            //this.zoneBiomassRemovedSpp = new int[PopulationZones.Dataset.Count][];
            this.zoneBiomassRemovedSpp = new double[PopulationZones.Dataset.Count][];
            
            foreach (IPopulationZone popZone in PopulationZones.Dataset)

                {
                this.zoneSitesDamaged[popZone.Index] = 0;
                this.zonePopulation[popZone.Index] = 0;
                this.zoneCohortsKilled[popZone.Index] = 0;
                this.zoneBiomassRemoved[popZone.Index] = 0;
                this.zoneBiomassKilled[popZone.Index] = 0;
                this.zoneCohortsKilledSpp[popZone.Index] = new int[PlugIn.ModelCore.Species.Count];
                this.zoneBiomassRemovedSpp[popZone.Index] = new double[PlugIn.ModelCore.Species.Count];
                foreach (ISpecies species in PlugIn.ModelCore.Species)
                {
                    this.zoneCohortsKilledSpp[popZone.Index][species.Index] = 0;
                    this.zoneBiomassRemovedSpp[popZone.Index][species.Index] = 0;
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

                    //Browse - allocate browse to cohorts
                    if (siteTotalToBrowse > 0)
                    {
                        List<ICohort> siteCohortList = new List<ICohort>();
                        foreach (ISpecies species in PlugIn.ModelCore.Species)
                        {
                            ISpeciesCohorts cohortList = SiteVars.BiomassCohorts[site][species];
                            if (cohortList != null)
                            {
                                foreach (ICohort cohort in cohortList)
                                {
                                    siteCohortList.Add(cohort);
                                }
                            }
                        }
                        double forageRemoved = 0.0;
                        
                        //Browse - compile browse by preference class
                        double[] forageByPrefClass = new double[parameters.PreferenceList.Count];
                        double[] propBrowseList = new double[siteCohortList.Count];

                        //Browse - calculate first pass removal
                        // first pass removes forage at rate equal to preference
                        double[] firstPassRemovalList = new double[siteCohortList.Count];
                        double firstPassRemoval = 0;
                        //double firstPassRemovalInt = 0;
                        int cohortLoop = 0;
                        foreach (ICohort cohort in siteCohortList)
                        {
                            ISppParameters sppParms = parameters.SppParameters[cohort.Species.Index];
                            double browsePref = sppParms.BrowsePref;

                            double availForage = SiteVars.GetForageInReach(cohort, site);
                            firstPassRemoval += availForage * browsePref;                           
                            //PlugIn.ModelCore.UI.WriteLine("{0:0.0}/{1:0.0}. availForage = {2}, browsePref = {3}, firstPassRemoval = {4}",
                            //    cohort.Species.Name, cohort.Age, availForage, browsePref, firstPassRemoval);
                            firstPassRemovalList[cohortLoop] = availForage * browsePref;

                            int prefIndex = 0;
                            foreach (double prefValue in parameters.PreferenceList)
                            {
                                if (browsePref == prefValue)
                                {
                                    forageByPrefClass[prefIndex] += (availForage - (availForage * browsePref));
                                    break;
                                }
                                prefIndex++;
                            }

                            cohortLoop++;
                        }

                        // First pass adjustment
                        // if first pass exceeds removal then adjust downward for each cohort
                        double[] adjFirstPassRemovalList = new double[siteCohortList.Count];
                        double[] remainingBrowseList = new double[siteCohortList.Count];
                        double adjFirstPassRemoval = firstPassRemoval;
                        //double adjFirstPassRemovalInt = firstPassRemovalInt;
                        if (firstPassRemoval > siteTotalToBrowse)
                        {
                            adjFirstPassRemoval = 0;
                            //adjFirstPassRemovalInt = 0;
                            int removalIndex = 0;
                            foreach (var i in firstPassRemovalList)
                            {
                                double firstRemoval = firstPassRemovalList[removalIndex];
                                double adjFirstRemoval = firstRemoval * siteTotalToBrowse / firstPassRemoval;
                                adjFirstPassRemovalList[removalIndex] = adjFirstRemoval;
                                adjFirstPassRemoval += adjFirstRemoval;
                                //adjFirstPassRemovalInt += adjFirstRemoval; // Math.Round(adjFirstRemoval);
                                removalIndex++;

                            }
                        }
                        else
                        {
                            adjFirstPassRemovalList = firstPassRemovalList;
                        }
                        int adjIndex = 0;
                        foreach (var i in adjFirstPassRemovalList)
                        {
                            //SF TODO is it inefficient to call GetForageInReach so many times?
                            remainingBrowseList[adjIndex] = SiteVars.GetForageInReach(siteCohortList[adjIndex], site) - adjFirstPassRemovalList[adjIndex];
                            adjIndex++;
                        }
                        double unallocatedBrowse = siteTotalToBrowse - adjFirstPassRemoval;
                        

                        //Browse - allocate second pass of browse removal to cohorts
                        //second pass removes all of most preferred before moving down in preference
                        double[] secondPassRemovalList = new double[siteCohortList.Count];
                        double[] finalRemovalList = adjFirstPassRemovalList;
                        forageRemoved = adjFirstPassRemoval;
                        int prefLoop = 0;
                        foreach (double prefValue in parameters.PreferenceList)
                        {
                            cohortLoop = 0;
                            double prefClassRemoved = 0;
                            foreach (Landis.Library.BiomassCohorts.Cohort cohort in siteCohortList)
                            {
                                ISppParameters sppParms = parameters.SppParameters[cohort.Species.Index];
                                double browsePref = sppParms.BrowsePref;

                                if (browsePref == prefValue)
                                {
                                    double finalRemoval = adjFirstPassRemovalList[cohortLoop];
                                    if (forageRemoved < siteTotalToBrowse)
                                    {
                                        double availForage = SiteVars.GetForageInReach(cohort, site); 
                                        double prefClassForage = forageByPrefClass[prefLoop];
                                        double secondPassRemoval = 0;
                                        if (prefClassForage > 0)
                                        {
                                            secondPassRemoval = unallocatedBrowse * ((availForage - adjFirstPassRemovalList[cohortLoop]) / prefClassForage);
                                            secondPassRemoval = Math.Min(secondPassRemoval, (availForage - adjFirstPassRemovalList[cohortLoop]));
                                        }
                                        secondPassRemovalList[cohortLoop] = secondPassRemoval;
                                        finalRemoval += secondPassRemoval;
                                        forageRemoved -= adjFirstPassRemovalList[cohortLoop];
                                        forageRemoved += finalRemoval;
                                        prefClassRemoved += (finalRemoval - adjFirstPassRemovalList[cohortLoop]);
                                        //PlugIn.ModelCore.UI.WriteLine("{0:0.0}/{1:0.0}. secondPassRemoval = {2}, finalRemoval = {3}", cohort.Species.Name, cohort.Age, secondPassRemoval, finalRemoval); //debug
                                    }
                                    
                                    finalRemovalList[cohortLoop] = finalRemoval;

                                    this.biomassRemoved += (double)finalRemoval;
                                    this.zoneBiomassRemoved[popZone.Index] += (double)finalRemoval;
                                    siteTotalRemoval += finalRemoval;

                                    double propBrowse = 0.0;
                                    if (SiteVars.GetForage(siteCohortList[cohortLoop], site) > 0)
                                        propBrowse = finalRemoval / SiteVars.GetForage(siteCohortList[cohortLoop], site);
                                    //PlugIn.ModelCore.UI.WriteLine("propBrowse = {0}", propBrowse);//debug
                                    propBrowseList[cohortLoop] = propBrowse;
                                    if (propBrowse < 0.0 || propBrowse > 1.0001)
                                        //SF TODO this error still comes up frequently -- track this down
                                        PlugIn.ModelCore.UI.WriteLine("   Browse Proportion not between 0 and 1: {0}", propBrowse);
                                    if (propBrowse > 1.0001)
                                        propBrowse = 1;

                                    //LastBrowseProportion is only used for GrowthReduction
                                    if (propBrowse > 0.0)
                                        //PlugIn.ModelCore.UI.WriteLine("Setting LastBrowseProportion :  {0:0.0}/{1:0.0}/{2}.", cohort.Species.Name, cohort.Age, propBrowse);
                                        SiteVars.SetLastBrowseProportion(cohort, site, propBrowse);
                                        
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
                                            double biomassKilled = (double)cohort.Biomass - (double)finalRemoval;
                                            finalRemoval = (double)cohort.Biomass;
                                            this.biomassKilled += biomassKilled;
                                            this.zoneBiomassKilled[popZone.Index] += biomassKilled;
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
                                        PartialDisturbance.RecordBiomassReduction(cohort, finalRemoval);
                                        this.biomassRemovedSpp[cohort.Species.Index] += finalRemoval;
                                        this.zoneBiomassKilled[popZone.Index] += finalRemoval;
                                        this.zoneBiomassRemovedSpp[popZone.Index][cohort.Species.Index] += finalRemoval;
                                    }
                                }
                                cohortLoop++;

                            }
                            unallocatedBrowse -= prefClassRemoved;
                            prefLoop++;
                        }
                        this.sitesDamaged += 1;
                        this.zoneSitesDamaged[popZone.Index] += 1;
                    }

                    // Send biomass reduction to the succession extensions
                    PartialDisturbance.ReduceCohortBiomass(site);
                    
                }
            }
        }

        //---------------------------------------------------------------------
        public static double CalculateReduction(double threshold, double max, double propBrowse)
        {
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
            //   Forage.UpdateCohortForage(parameters);
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
                                       
                    ISpeciesCohorts cohortList = SiteVars.BiomassCohorts[site][species];

                    if (cohortList != null)
                    {
                        foreach (ICohort cohort in cohortList)
                            //for each cohort, get the new forage (Biomass*0.04*proportion of ANPP that is forage)
                            //and assign newForage to cohort in SiteVars
                            //total forage will later be reduced to represent forage in reach of browsers
                        {
                            //int newForage = 0;
                            double newForage = 0;
                            //PlugIn.ModelCore.UI.WriteLine("     browsePref = {0}", browsePref); //debug
                            if ((browsePref > 0) || (parameters.CountNonForage))
                            {
                                //newForage = (int)Math.Round(cohort.ANPP * parameters.ANPPForageProp);
                                //newForage = (int)Math.Round((cohort.Biomass * 0.1) * parameters.ANPPForageProp);  // RMS:  Using 10% approximation for now; will update from Keeling curve later.
                                //newForage = (cohort.Biomass * 0.1) * parameters.ANPPForageProp;  // RMS:  Using 10% approximation for now; will update from Keeling curve later.

                                // SF: using smaller value of 4%, from Hubbard Brook:  https://hubbardbrook.org/online-book/forest-biomass-and-primary-productivity
                                // This value also matches more closely what Biomass Succession was generating for the previous version of the model
                                // This gets us site ANPP within 10% of what NECN produces, at least for the handful of "typical" sites I tested -- SF
                                newForage = (cohort.Biomass * 0.04) * parameters.ANPPForageProp;
                                //PlugIn.ModelCore.UI.WriteLine("ANPP estimated as {0}", newForage);//debug

                                //Use estimates from Keeling quadratic all-data model, inverted to represent ANPP ~ biomass
                                // Work in progress -- these curves do not work well for small cohorts
                                //newForage = (32.61 - Math.Sqrt(1083 - 3.056 * cohort.Biomass/100)) / 1.528;
                                //newForage *= 100;
                                //PlugIn.ModelCore.UI.WriteLine("ANPP estimated as {0}", newForage);//debug
                                //newForage = 19.9539 - 0.0035461 * Math.Sqrt(2.55099e7 - 56400 * cohort.Biomass/100);
                                //newForage *= 100;
                                //PlugIn.ModelCore.UI.WriteLine("ANPP estimated as {0}", newForage);//debug
                                //newForage *= parameters.ANPPForageProp;

                                //PlugIn.ModelCore.UI.WriteLine("     Calculating original newForage = {0}", newForage); //debug

                                if (cohort.Age == 1)
                                {
                                    if (parameters.UseInitBiomass)
                                    {
                                        newForage = cohort.Biomass * parameters.ANPPForageProp; //(int)Math.Round(cohort.Biomass * parameters.ANPPForageProp);
                                    }
                                    else
                                    {
                                        newForage = 0;
                                        //PlugIn.ModelCore.UI.WriteLine("     Baby cohort = no forage"); //debug
                                    }
                                }
                            }

                            //we also need to update the site max biomass if a species with a high value is present
                            //TODO SF why?
                            siteBiomassMax = Math.Max(siteBiomassMax, sppParms.BiomassMax);
                            //PlugIn.ModelCore.UI.WriteLine("     adjusting siteBiomassMax = {0} g m-2", siteBiomassMax); //debug  

                            //record original forage
                            SiteVars.SetForage(cohort, site, newForage);
                        }
                    }
                }

                List<ICohort> siteCohortList = new List<ICohort>();


                foreach (ISpecies species in PlugIn.ModelCore.Species)
                    //Build a list of cohorts at the site
                {
                    ISpeciesCohorts cohortList = SiteVars.BiomassCohorts[site][species];

                    if (cohortList != null)
                    {
                        foreach (ICohort cohort in cohortList)
                        {
                            siteCohortList.Add(cohort);

                        }
                    }
                       
                }


                // This next chunk calculates how much of the forage is "in reach" or available to be 
                // foraged. It's highly dependent upon the biomassThreshold.
                List<double> propInReachList = new List<double>(siteCohortList.Count);
                propInReachList = Forage.CalculateCohortPropInReach(siteCohortList, parameters, siteBiomassMax);

                int listCount = 0;
                foreach (ICohort cohort in siteCohortList)
                {
                    //int newForageinReach = (int)Math.Round(SiteVars.GetForage(cohort, site) * propInReachList[listCount]);
                    double newForageinReach = SiteVars.GetForage(cohort, site) * propInReachList[listCount];
                    //PlugIn.ModelCore.UI.WriteLine("forage in reach = {0} g m-2", newForageinReach); //debug

                    if (newForageinReach > 0)
                        SiteVars.SetForageInReach(cohort, site, newForageinReach);

                    listCount++;
                }

                //  Calculate Site BrowsePreference and ForageQuantity
                SitePreference.CalcSiteForage(parameters, site);
            }
        }

    }
}
