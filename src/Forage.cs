//  Authors:  Brian Miranda, Nate De Jager, Patrick Drohan, Robert Scheller

using System;
using System.Collections.Generic;
using System.Linq;
using Landis.Library.BiomassCohorts;

namespace Landis.Extension.Browse
{
    class Forage
    {
        private static IDictionary<ushort, int>[] forageDictionary;

        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {
            forageDictionary = new IDictionary<ushort, int>[PlugIn.ModelCore.Species.Count];
            for (int i = 0; i < forageDictionary.Length; i++)
                forageDictionary[i] = new Dictionary<ushort, int>();
            
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Records the forage for a particular cohort.
        /// </summary>
        public static void RecordForage(ICohort cohort,
                                                  int forage)
        {
            forageDictionary[cohort.Species.Index][cohort.Age] = forage;
        }
        //---------------------------------------------------------------------
        public static List<double> CalculateCohortPropInReach(List<Landis.Library.BiomassCohorts.ICohort> cohortList, IInputParameters parameters, double siteBiomassMax)
        {
            //PlugIn.ModelCore.UI.WriteLine("   Calculating proportion of browse in reach using method {0}", PlugIn.PropInReachMethod); //debug

            //Cohorts above maxBrowseAge (calculated from maxBrowseAgeProp) are immune from forage (due to height and crown lift)
            double maxBrowseAgeProp = parameters.EscapeBrowsePropLong;
            double minBrowseProp = parameters.MinBrowsePropinReach;

            List<double> propInReachList = new List<double>(cohortList.Count);

            double[] sortedProportion = new double[cohortList.Count];

            // Sort cohorts and loop through them from smallest to largest until 
            // maxThreshold has been reached. 
            var sortedCohortList = cohortList.OrderBy(cohort => cohort.Biomass).ToList();

            int sortedBioIndex = 0;

            if (PlugIn.PropInReachMethod == "LinearEachCohort")
            {
                foreach (Landis.Library.BiomassCohorts.ICohort cohort in sortedCohortList)
                {
                    ISppParameters sppParms = parameters.SppParameters[cohort.Species.Index];
                    double minThreshold = sppParms.BiomassMax * parameters.BrowseBiomassThreshMin; //below this value, cohort entirely foraged
                    double maxThreshold = sppParms.BiomassMax * parameters.BrowseBiomassThreshMax; //above this threshold, cohort entirely escapes forage
                    double maxBrowseAge = cohort.Species.Longevity * maxBrowseAgeProp; //above this value, cohort escapes forage

                    //PlugIn.ModelCore.UI.WriteLine("Min biomass threshold = {0}, Max biomass threshold {1}, maxBrowseAge = {2}", 
                    //    minThreshold, maxThreshold, maxBrowseAge);//debug

                    double propInReach = 0;

                    if (cohort.Age < maxBrowseAge)
                    {
                        if (cohort.Biomass <= minThreshold)
                        {
                            //if a cohort is smaller than minThreshold, all of it can be foraged
                            propInReach = 1.0;
                            //PlugIn.ModelCore.UI.WriteLine("Cohort entirely in reach; biomass = {0}, threshold = {1}", cohort.Biomass, minThreshold);//debug
                        }
                        else if (cohort.Biomass > maxThreshold)
                        {
                            // if a cohort is larger than maxThreshold, none of it can be foraged
                            propInReach = 0.0;
                            //PlugIn.ModelCore.UI.WriteLine("Cohort escaped; biomass = {0}, maxThresold = {1}", cohort.Biomass, maxThreshold);//debug
                        }
                        else
                        {
                            //if the cohort is larger than minThreshold but smaller than maxThreshold,
                            // then it is foraged as a proportion of the maxThreshold. 
                            // The closer to the threshold, the less of the biomass should be available.
                            // E.g., if the maxThreshold was 3,000, and a cohort has a biomass
                            // of 2,000, then 1/3 of the cohort's ANPP should be available for browsing. 

                            double tempPropInReach = Math.Min(1, (1 - (cohort.Biomass / maxThreshold)));
                            //PlugIn.ModelCore.UI.WriteLine("tempPropInReach = {0}", tempPropInReach);//debug
                            propInReach = tempPropInReach;

                            if (tempPropInReach < minBrowseProp)
                            {
                                propInReach = 0;
                                //PlugIn.ModelCore.UI.WriteLine("Cohort escaped by minBrowseProp; tempPropInReach = {0}", tempPropInReach);//debug
                            }
                                                        
                        }
                    }

                    //PlugIn.ModelCore.UI.WriteLine("end Cohort biomass = {0}; proportion in reach = {1}.", cohort.Biomass, propInReach); //debug
                    sortedProportion[sortedBioIndex] = propInReach;
                    sortedBioIndex++;
                }
            }

            else //"ordered cohorts" method
            {

                //These parameters have slightly different meanings depending on which PropInReachMethod is used
                double maxBrowseEscape = parameters.BrowseBiomassThreshMax;

                double maxThreshold = siteBiomassMax * maxBrowseEscape;
                double biomassThreshold = maxThreshold;

                double remainingThreshold = biomassThreshold;
                //PlugIn.ModelCore.UI.WriteLine("remainingThreshold = {0}", remainingThreshold); //debug

                foreach (Landis.Library.BiomassCohorts.ICohort cohort in sortedCohortList)
                {
                    double maxBrowseAge = cohort.Species.Longevity * maxBrowseAgeProp;

                    double propInReach = 0;
                    if (cohort.Age < maxBrowseAge)
                    {
                        if (cohort.Biomass <= remainingThreshold)
                        {
                            propInReach = 1.0;
                            remainingThreshold -= cohort.Biomass;
                        }
                        else
                        {
                            double tempPropInReach = (remainingThreshold / cohort.Biomass);
                            //PlugIn.ModelCore.UI.WriteLine("tempPropInReach = {0}.", tempPropInReach); //debug
                            if (tempPropInReach < minBrowseProp)
                            {
                                //PlugIn.ModelCore.UI.WriteLine("Cohort escaped browse. tempPropInReach = {0}; minBrowseProp = {1}.", tempPropInReach, minBrowseProp); //debug
                                propInReach = 0;
                            }
                            else
                            {
                                //PlugIn.ModelCore.UI.WriteLine("Cohort partially in reach"); //debug
                                propInReach = tempPropInReach;
                                remainingThreshold = 0;
                            }
                        }
                    }
                    //PlugIn.ModelCore.UI.WriteLine("end Cohort biomass = {0}; proportion in reach = {1}.", cohort.Biomass, propInReach); //debug
                    sortedProportion[sortedBioIndex] = propInReach;
                    sortedBioIndex++;
                }

            }
            

            //SF TODO this seems like a really inefficient way to do this -- looping over all the cohorts
            // just to set values for the handful of cohorts within a site
            int cohortIndex = 0;
            foreach (Landis.Library.BiomassCohorts.ICohort cohort in cohortList)
            {
                int sortedIndex = 0;
                foreach (Landis.Library.BiomassCohorts.ICohort xcohort in sortedCohortList)
                {
                    if ((xcohort.Species == cohort.Species) && (xcohort.Age == cohort.Age) && (xcohort.Biomass == cohort.Biomass))
                    {
                        propInReachList.Add(sortedProportion[sortedIndex]);
                    }
                    sortedIndex++;
                }
                cohortIndex++;
            }

            return propInReachList;
        }

    }

}
