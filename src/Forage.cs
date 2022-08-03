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
            //minBrowseProp is a threshold below which all cohorts are browsed. That browse is subtracted 
            // from the threshold until the threshold has been depleted.
            // 
            // TODO rename and fix in user guide
            double minBrowseProp = parameters.MinBrowsePropinReach;
            double minBrowseEscape = parameters.BrowseBiomassThreshMin;
            double maxBrowseEscape = parameters.BrowseBiomassThreshMax;

            //Cohorts above maxBrowseAge (calculated from maxBrowseAgeProp) are immune from forage (due to height and crown lift)
            double maxBrowseAgeProp = parameters.EscapeBrowsePropLong;

            List<double> propInReachList = new List<double>(cohortList.Count);

            double[] sortedProportion = new double[cohortList.Count];

            // Sort cohorts and loop through them from smallest to largest until 
            // maxThreshold has been reached. 
            var sortedCohortList = cohortList.OrderBy(cohort => cohort.Biomass).ToList();

            int sortedBioIndex = 0;
            double minThreshold = siteBiomassMax * minBrowseEscape;
            double maxThreshold = siteBiomassMax * maxBrowseEscape;

            // Total forage is tracked to toggle the forage calculation between different "modes."
            // While it is below minThreshold, then small cohorts can be entirely browsed. Once the
            // threshold is met, then browse is considered in reach as a proportion of maxThreshold.
            double biomassThreshold = maxThreshold;

            // Biomass that can be considered as forage, for tracking browsable biomass.
            // This is a different number than NewForage that is calculated in Event.cs.
            double forageBiomassPool = 0;

            foreach (Landis.Library.BiomassCohorts.ICohort cohort in sortedCohortList)
            {
                double propInReach = 0;

                if(forageBiomassPool >= maxThreshold)
                {
                    sortedProportion[sortedBioIndex] = propInReach;
                    sortedBioIndex++;
                    //PlugIn.ModelCore.UI.WriteLine("Threshold exceeded, skipping cohort");//debug
                    continue;                    
                }

                double maxBrowseAge = cohort.Species.Longevity * maxBrowseAgeProp;

                biomassThreshold = maxThreshold - forageBiomassPool;
                //PlugIn.ModelCore.UI.WriteLine("biomassThreshold = {0}", biomassThreshold);//debug

                if (cohort.Age < (maxBrowseAge))
                {
                    if (cohort.Biomass <= minThreshold)
                    {
                            //if a cohort is smaller than minThreshold, all of it can be foraged
                            propInReach = 1.0;
                            forageBiomassPool += cohort.Biomass;
                    }
                    else if (cohort.Biomass > maxThreshold)
                    {
                            // if a cohort is larger than maxThreshold, none of it can be foraged
                            propInReach = 0.0;
                            //PlugIn.ModelCore.UI.WriteLine("Cohort escaped; biomass = {0}", cohort.Biomass);//debug
                    }
                    else
                    {
                            //if the cohort is larger than minThreshold but smaller than maxThreshold,
                            // then it is foraged as a proportion of the maxThreshold. 
                            // The closer to the threshold, the less of the biomass should be available.
                            // E.g., if the maxThreshold was 3,000, and a cohort has a biomass
                            // of 2,000, then 1/3 of the cohort should be available for browsing. 
                            
                            double tempPropInReach = Math.Min(1, (1 - (cohort.Biomass / maxThreshold)));
                            if (tempPropInReach < minBrowseProp)
                            {
                                      propInReach = 0;
                                      //PlugIn.ModelCore.UI.WriteLine("Cohort escaped by minBrowseProp; tempPropInReach = {0}", tempPropInReach);//debug
                            }
                            else
                            {
                                    propInReach = tempPropInReach;

                                    //if adding this biomass to the pool would put the forageBiomassPool over the maxThreshold
                                    if((forageBiomassPool + (tempPropInReach * cohort.Biomass)) > maxThreshold)
                                    {
                                        double tempBiomass = maxThreshold - forageBiomassPool; //how much biomass until we meet the threshold?
                                        propInReach = tempBiomass/cohort.Biomass; //if all that tempBiomass was foraged, what proportion of the cohort biomass is that?
                                        //PlugIn.ModelCore.UI.WriteLine("Cohort escaped by reaching site biomass threshold; cohortBiomass = {0}, propInReach = {1}", cohort.Biomass, propInReach);//debug
                                    }

                                    forageBiomassPool += propInReach * cohort.Biomass;
                            }
                    }
                }
               // PlugIn.ModelCore.UI.WriteLine("end Cohort biomass = {0}; proportion in reach = {1}.", cohort.Biomass, propInReach); //debug
                sortedProportion[sortedBioIndex] = propInReach;
                sortedBioIndex++;
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
