using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.SpatialModeling;

namespace Landis.Extension.Browse
{
    class BrowseIndex
    {
        public static void CalcBrowseIndex(IInputParameters parameters)
        {
            PlugIn.ModelCore.UI.WriteLine("   Calculating Browse Index.");

            //double deerDensityIndex = parameters.DeerDensityIndex;
            double deerDensityIndex = 0;

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                double browseIndex = deerDensityIndex * (1 - SiteVars.NeighborhoodForage[site]);
                SiteVars.BrowseIndex[site] = browseIndex;
            }
        }
    }
}
