using System.Collections.Generic;
using System.Linq;
using Landis.Library.BiomassCohorts;

namespace Landis.Extension.DeerBrowse
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
        public static List<double> CalculateCohortPropInReach(List<Landis.Library.BiomassCohorts.ICohort> cohortList, IInputParameters parameters, double biomassThreshold)
        {
            double minBrowseProp = parameters.MinBrowsePropinReach;
            double maxBrowseAge = parameters.EscapeBrowsePropLong;

            List<double> propInReachList = new List<double>(cohortList.Count);

            double[] sortedProportion = new double[cohortList.Count];

            //Landis.Library.BiomassCohorts.ISpeciesCohorts sortedCohortList = cohortList;
            //sortedCohortList.
            var sortedCohortList = cohortList.OrderBy(cohort => cohort.Biomass).ToList();

            int sortedBioIndex = 0;
            double remainingThreshold = biomassThreshold;
            foreach (Landis.Library.BiomassCohorts.ICohort cohort in sortedCohortList)
            {
                double propInReach = 0;
                if (cohort.Age < (cohort.Species.Longevity * maxBrowseAge))
                {
                    //sortedBiomass[sortedBioIndex] = cohort.Biomass;
                    if (cohort.Biomass <= remainingThreshold)
                    {
                        propInReach = 1.0;
                        remainingThreshold -= cohort.Biomass;
                    }
                    else
                    {
                        double tempPropInReach = (remainingThreshold / cohort.Biomass);
                        if (tempPropInReach < minBrowseProp)
                            propInReach = 0;
                        else
                            propInReach = tempPropInReach;
                        remainingThreshold = 0;
                    }
                }
                sortedProportion[sortedBioIndex] = propInReach;
                sortedBioIndex++;
            }

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
