using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;
//using Landis.Library.AgeOnlyCohorts;

namespace Landis.Extension.Browse
{
    class Damage
    {
        //---------------------------------------------------------------------
        //Go through all active sites and damage them according to the
        //Browse Index.
        public static void DisturbSites(IInputParameters parameters)
        { 
            
            int[] cohortsKilled = new int[2];

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                
                double propRemove = SiteVars.BrowseIndex[site];
                if (propRemove > 0)
                {
                    int cohortCount = 0;
                    //if (parameters.SuccessionMethod == "AgeOnly")
                    //{
                        // Sort young cohorts by decreasing preference
                        List<ICohort> cohortList = new List<ICohort>();
                        foreach (ISpeciesCohorts speciesCohorts in SiteVars.BiomassCohorts[site])
                        {
                            foreach (ICohort cohort in speciesCohorts)
                            {
                                if (cohort.Age <= 10)
                                {
                                    cohortList.Add(cohort);
                                }
                            }
                        }
                        cohortList = cohortList.OrderByDescending(x => parameters.SppParameters[x.Species.Index].BrowsePref).ToList();
                        cohortCount = cohortList.Count;
                        int cohortsToRemove = (int)Math.Round((double)cohortCount * propRemove);

                        int cohortsRemoved = 0;
                        foreach (ICohort cohort in cohortList)
                        {
                            if (cohortsRemoved < cohortsToRemove)
                            {
                                cohortsRemoved += 1;
                            }
                            else
                            {
                                break;
                            }

                        }
                    //}
                    //else // (parameters.SuccessionMethod == "Biomass")
                    //{
                    //    //PartialDisturbance.RecordBiomassReduction(cohort, remainBioRem);
                    //}

                }
            }


        }
    }
}
