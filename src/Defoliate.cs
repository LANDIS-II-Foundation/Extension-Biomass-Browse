//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

using System;
using System.Collections.Generic;
using Landis.Core;
using Landis.Library.Biomass;
using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;


namespace Landis.Extension.DeerBrowse
{
    
    public class Defoliate
    {
        public static IInputParameters browseParameters;
        //---------------------------------------------------------------------

        public static void Initialize(IInputParameters parameters)
        {
            // Assign the method below to the CohortDefoliation delegate in
            // Landis.Library.Biomass/Biomass.CohortDefoliation.cs
            //Landis.Library.Biomass.CohortDefoliation.Compute = Defoliate.DefoliateCohort;
            IInputParameters browseParameters = parameters;
        }

        //---------------------------------------------------------------------
        // This method replaces the delegate method.  It is called every year when
        // ACT_ANPP is calculated, for each cohort.  Therefore, this method is operating at
        // an ANNUAL time step and separate from the normal extension time step.

        public static double DefoliateCohort(ActiveSite site, ISpecies species, int cohortBiomass, int siteBiomass, int ANPP = 0)
        {
            //PlugIn.ModelCore.UI.WriteLine("   Calculating browse...");
            double defoliation = 0.0;
            double currentForage = ANPP * browseParameters.ANPPForageProp;

            //double propBrowse = finalRemoval / currentForage;
            return defoliation;  // Cohort total defoliation proportion (summed across insects)

        }



    }

}
