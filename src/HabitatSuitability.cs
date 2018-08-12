//  Authors:  Brian Miranda, Nate De Jager, Patrick Drohan

using System.Collections.Generic;
using Landis.SpatialModeling;

namespace Landis.Extension.DeerBrowse
{
    class HabitatSuitability
    {
        //---------------------------------------------------------------------
        /// <summary>
        /// Calculate habitat suitability index (HSI) from
        /// neighborhood forage and/or neighborhood site preference
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns> sum of HSI values for landscape</returns>
        public static void CalculateHSI(IInputParameters parameters)
        {
            PlugIn.ModelCore.UI.WriteLine("   Calculating HSI.");
            //double cumulativeHSI = 0;
            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                double forage = 1;
                //bool forageQuantity = false;
                double sitePref = 1;
                //bool sitePreference = false;
                if (parameters.ForageQuantityNbrRad != -999)
                {
                    //forageQuantity = true;
                    forage = CalcNeighborhoodForage(parameters, site);
                }
                if (parameters.SitePrefNbrRad != -999)
                {
                    //sitePreference = true;
                    sitePref = CalcNeighborhoodSitePref(parameters, site);
                }
                double HSI = forage * sitePref;
                SiteVars.HabitatSuitability[site] = HSI;
                //cumulativeHSI += HSI;               
            }
            //return cumulativeHSI;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Calculate average forage quantity within neighborhood
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        public static double CalcNeighborhoodForage(IInputParameters parameters, ActiveSite site)
        {

            //PlugIn.ModelCore.UI.WriteLine("   Calculating Neighborhood Forage.");

            double totalNeighborWeight = 0.0;
            double maxNeighborWeight = 0.0;
            int neighborCnt = 0;
            //int speedUpFraction = (int)parameters.NeighborSpeedUp + 1;
            int speedUpFraction = 1;

            //neighborWeight = 0.0;
            totalNeighborWeight = 0.0;
            maxNeighborWeight = 0.0;
            neighborCnt = 0;

            List<RelativeLocationWeighted> neighborhood = new List<RelativeLocationWeighted>();
            foreach (RelativeLocationWeighted relativeLoc in parameters.ForageNeighbors)
            {

                Site neighbor = site.GetNeighbor(relativeLoc.Location);
                if (neighbor != null
                    && neighbor.IsActive)
                {
                    neighborhood.Add(relativeLoc);
                }
            }

            neighborhood = PlugIn.ModelCore.shuffle(neighborhood);

            totalNeighborWeight += SiteVars.ForageQuantity[site];
            maxNeighborWeight += 1;

            foreach (RelativeLocationWeighted neighbor in neighborhood)
            {
                //Do NOT subsample if there are too few neighbors
                //i.e., <= subsample size.
                if (neighborhood.Count <= speedUpFraction ||
                    neighborCnt % speedUpFraction == 0)
                {
                    Site activeSite = site.GetNeighbor(neighbor.Location);

                    //Note:  SitePreference ranges from 0 - 1.

                    totalNeighborWeight += SiteVars.ForageQuantity[activeSite] * neighbor.Weight;
                    maxNeighborWeight += neighbor.Weight;
                  
                }
                neighborCnt++;
            }

            double neighborhoodForage = 0;
            if (maxNeighborWeight > 0.0)
                neighborhoodForage = totalNeighborWeight / maxNeighborWeight;
            else
                neighborhoodForage = 0.0;

            return neighborhoodForage;

        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Calculate average site preference in neighborhood
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        public static double CalcNeighborhoodSitePref(IInputParameters parameters, ActiveSite site)
        {

            //PlugIn.ModelCore.UI.WriteLine("   Calculating Neighborhood Site Preference.");

            double totalNeighborWeight = 0.0;
            double maxNeighborWeight = 0.0;
            int neighborCnt = 0;
            //int speedUpFraction = (int)parameters.NeighborSpeedUp + 1;
            int speedUpFraction = 1;

            //neighborWeight = 0.0;
            totalNeighborWeight = 0.0;
            maxNeighborWeight = 0.0;
            neighborCnt = 0;


            List<RelativeLocationWeighted> neighborhood = new List<RelativeLocationWeighted>();
            foreach (RelativeLocationWeighted relativeLoc in parameters.SitePrefNeighbors)
            {

                Site neighbor = site.GetNeighbor(relativeLoc.Location);
                if (neighbor != null
                    && neighbor.IsActive)
                {
                    neighborhood.Add(relativeLoc);
                }
            }

            neighborhood = PlugIn.ModelCore.shuffle(neighborhood);

            totalNeighborWeight += SiteVars.SitePreference[site];
            maxNeighborWeight += 1;

            foreach (RelativeLocationWeighted neighbor in neighborhood)
            {
                //Do NOT subsample if there are too few neighbors
                //i.e., <= subsample size.
                if (neighborhood.Count <= speedUpFraction ||
                    neighborCnt % speedUpFraction == 0)
                {
                    Site activeSite = site.GetNeighbor(neighbor.Location);

                    //Note:  SitePreference ranges from 0 - 1.

                    totalNeighborWeight += SiteVars.SitePreference[activeSite] * neighbor.Weight;
                    maxNeighborWeight += neighbor.Weight;

                }
                neighborCnt++;
            }

            double neighborhoodSitePref = 0;
            if (maxNeighborWeight > 0.0)
                neighborhoodSitePref = totalNeighborWeight / maxNeighborWeight;
            else
                neighborhoodSitePref = 0.0;

            return neighborhoodSitePref;

        }
        //---------------------------------------------------------------------
    }
}
