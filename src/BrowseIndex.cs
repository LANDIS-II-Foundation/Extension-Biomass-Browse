using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.SpatialModeling;

//TODO SF remove this file; never referenced
namespace Landis.Extension.Browse
{
    class BrowseIndex
    {
        public static void CalcBrowseIndex(IInputParameters parameters)
        {
            PlugIn.ModelCore.UI.WriteLine("   Calculating Browse Index.");

            double deerDensityIndex = parameters.BrowseDensityIndex;

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                double browseIndex = deerDensityIndex * (1 - SiteVars.NeighborhoodForage[site]);
                SiteVars.BrowseIndex[site] = browseIndex;
            }
        }
    }
}
