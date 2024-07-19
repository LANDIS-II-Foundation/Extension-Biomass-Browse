//  Authors:  Brian Miranda, Nate De Jager, Patrick Drohan, Robert Scheller

using Landis.Library.UniversalCohorts;
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
            //PlugIn.ModelCore.UI.WriteLine("Initializing growth reduction");
            CohortGrowthReduction.Compute = ReduceCohortGrowth;
            inputParameters = parameters;

        }

        //---------------------------------------------------------------------
        // This function is delegated to BiomassCohorts to allow the succession extension to 
        // reduce growth. It might need to be modified to accomodate other succession extensions,
        // or some extensions might need to be modified to call BiomassCohorts library. 

        public static double ReduceCohortGrowth(ICohort cohort, ActiveSite site)
        {
            //PlugIn.ModelCore.UI.WriteLine("   Reducing cohort growth..."); //debug
            double reduction = 0;
            double propBrowse = cohort.Data.AdditionalParameters.LastBrowseProportion;  //SiteVars.GetLastBrowseProportion(cohort, site); 
            double threshold = inputParameters.SppParameters[cohort.Species.Index].GrowthReductThresh;

            double max = inputParameters.SppParameters[cohort.Species.Index].GrowthReductMax;
            if (propBrowse > threshold)
            {
                reduction = (max / (1.0 - threshold)) * propBrowse - threshold * (max / (1 - threshold));
                //PlugIn.ModelCore.UI.WriteLine("Growth reduction from Browse extension is {0}", reduction); //debug
            }
            return reduction;

        }

    }
}
