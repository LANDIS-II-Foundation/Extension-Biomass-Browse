//  Authors:  Jane Foster, Robert M. Scheller, Brian Miranda

using System;
using Landis.SpatialModeling;
using Landis.Library.UniversalCohorts;

namespace Landis.Extension.Browse
{
    
    public class Defoliate
    {
        public static IInputParameters browseParameters;
        //---------------------------------------------------------------------

        public static void Initialize(IInputParameters parameters)
        {
            // Assign the method below to the CohortDefoliation delegate in BiomassCohorts
            IInputParameters browseParameters = parameters;
            Landis.Library.UniversalCohorts.CohortDefoliation.Compute = DefoliateCohort;

        }

        public static double DefoliateCohort(ActiveSite site, ICohort cohort, int cohortBiomass, int siteBiomass)
        {

            PlugIn.ModelCore.UI.WriteLine("   Calculating defoliation proportion...");

            double forage = SiteVars.GetForage(cohort, site);

            double propBrowse = SiteVars.GetLastBrowseProportion(cohort, site);

            double amountForaged = forage * propBrowse;

            // Estimate leaf biomass from total cohort biomass
            // see Poorter, H., A. M. Jagodzinski, R. Ruiz-Peinado, S. Kuyah, Y. Luo, J. Oleksyn, V. A. Usoltsev, T. N. Buckley, P. B. Reich, and L. Sack. 2015. How does biomass distribution change with size and differ among species? An analysis for 1200 plant species from five continents. New Phytologist 208:736–749.
            // SF TODO check how NECN and PnET are doing this calculation
            // Could allow to use leaf biomass directly from NECN or PnET?
            double leafBiomass = Math.Pow(10, 0.113 + 0.74 * Math.Log10(cohort.Data.Biomass));

            double defoliation = amountForaged / (double)leafBiomass;

            //PlugIn.ModelCore.UI.WriteLine("   Defoliation proportion is {0}. Estimated leafBiomass is {1}; total biomass is {2}", defoliation, leafBiomass, cohort.Biomass); //debug

            return defoliation;  // Cohort total defoliation proportion

        }

        }

}
