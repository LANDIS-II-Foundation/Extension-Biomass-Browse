//  Authors:  Brian Miranda, Nate De Jager, Patrick Drohan, Robert Scheller

using Landis.Library.BiomassCohorts;
using Landis.SpatialModeling;

namespace Landis.Extension.Browse
{
    class GrowthReduction
    {
        private static IInputParameters inputParameters;

        public static void Initialize(IInputParameters parameters)
        {
            // Assign the method below to the CohortGrowthReduction delegate in
            // biomass-cohorts/Biomass.CohortGrowthReduction.cs
            CohortGrowthReduction.Compute = ReduceCohortGrowth;
            inputParameters = parameters;

        }
         //---------------------------------------------------------------------
        // This method replaces the delegate method.  It is called every year when
        // ACT_ANPP is calculated, for each cohort.  Therefore, this method is operating at
        // an ANNUAL time step and separate from the normal extension time step.

        public static double ReduceCohortGrowth(ICohort cohort, ActiveSite site)
        {
            double reduction = 0;
            double propBrowse = SiteVars.GetLastBrowseProportion(cohort, site); //cohort.Data.LastBrowseProp;
            double threshold = inputParameters.SppParameters[cohort.Species.Index].GrowthReductThresh;

            double max = inputParameters.SppParameters[cohort.Species.Index].GrowthReductMax;
            if (propBrowse > threshold)
            {
                reduction = (max / (1.0 - threshold)) * propBrowse - threshold * (max / (1 - threshold));
            }
            return reduction;

        }

    }
}
