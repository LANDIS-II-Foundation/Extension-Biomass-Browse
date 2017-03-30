using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;
using Landis.Library.AgeOnlyCohorts;

namespace Landis.Extension.DeerBrowse
{
    class Damage
    {
        //---------------------------------------------------------------------
        //Go through all active sites and damage them according to the
        //Browse Index.
        public static void DisturbSites(IInputParameters parameters)
        { 
            int siteCohortsKilled = 0;
            int[] cohortsKilled = new int[2];

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                siteCohortsKilled = 0;

                double propRemove = SiteVars.BrowseIndex[site];
                if (propRemove > 0)
                {
                    int cohortCount = 0;
                    if (parameters.SuccessionMethod == "AgeOnly")
                    {
                        // Sort young cohorts by decreasing preference
                        List<Landis.Library.AgeOnlyCohorts.ICohort> cohortList = new List<Landis.Library.AgeOnlyCohorts.ICohort>();
                        foreach (Landis.Library.AgeOnlyCohorts.ISpeciesCohorts speciesCohorts in SiteVars.AgeCohorts[site])
                        {
                            foreach (Landis.Library.AgeOnlyCohorts.ICohort cohort in speciesCohorts)
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
                        foreach (Landis.Library.AgeOnlyCohorts.ICohort cohort in cohortList)
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
                    }
                    else // (parameters.SuccessionMethod == "Biomass")
                    {
                        //PartialDisturbance.RecordBiomassReduction(cohort, remainBioRem);
                    }

                }
            }


        }
    }
}
