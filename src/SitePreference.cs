//  Authors:  Brian Miranda, Nate De Jager, Patrick Drohan

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;

namespace Landis.Extension.Browse
{
    class SitePreference
    {
        
        //---------------------------------------------------------------------
        /// <summary>
        /// Calculate total site forage and forage-weighted site preference
        /// Forage calculations use forage in reach
        /// </summary>
        /// <param name="parameters"></param>
        public static void CalcSiteForage(IInputParameters parameters, ActiveSite site)
        {
            PlugIn.ModelCore.UI.WriteLine("   Calculating Site Preference & Forage.");

            double sumPref = 0.0;
            double weightPref = 0.0;
            int countCohorts = 0;
            double sumWeight = 0.0;
            double sumForage = 0.0;
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {

                ISppParameters sppParms = parameters.SppParameters[species.Index];
                ISpeciesCohorts cohortList = SiteVars.BiomassCohorts[site][species];
                if (cohortList != null)
                {
                    foreach (ICohort cohort in cohortList)
                    {
                        double browsePref = sppParms.BrowsePref;
                        if ((browsePref > 0) || (parameters.CountNonForage))
                        {
                            sumPref += browsePref;
                            weightPref += (browsePref * SiteVars.GetForageInReach(cohort, site));
                            countCohorts += 1;
                            sumWeight += SiteVars.GetForageInReach(cohort, site);
                            PlugIn.ModelCore.UI.WriteLine("     Adding to sumWeight. sumWeight = {0}", sumWeight);
                        }
                        if (browsePref > 0)
                            sumForage += SiteVars.GetForageInReach(cohort, site);
                            PlugIn.ModelCore.UI.WriteLine("     Adding to sumForage. sumForage = {0}", sumForage);
                    }
                }
            }
               
            double avgPref = 0.0;
            double avgWeightPref = 0.0;
            if (countCohorts > 0)
            {
                avgPref = sumPref / (double)countCohorts;
                avgWeightPref = weightPref / sumWeight;
            }

            SiteVars.SitePreference[site] = avgWeightPref;
            SiteVars.ForageQuantity[site] = sumForage;

            PlugIn.ModelCore.UI.WriteLine("     Site avgWeightPref = {0}; sumForage = {1}", avgWeightPref, sumForage);

        }  //end SitePreference
        //---------------------------------------------------------------------
    }
}
