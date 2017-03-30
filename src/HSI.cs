using System;
using Landis.Core;
using Landis.SpatialModeling;
using System.Collections.Generic;

namespace Landis.Extension.DeerBrowse
{
    class HSI
    {
        public static void CalcNeighborhoodForage(IInputParameters parameters)
        {
            if (parameters.ForageQuantityNbrRad != -999)
            {
                PlugIn.ModelCore.UI.WriteLine("   Calculating Neighborhood Forage.");

                double totalNeighborWeight = 0.0;
                double maxNeighborWeight = 0.0;
                int neighborCnt = 0;
                //int speedUpFraction = (int)parameters.NeighborSpeedUp + 1;
                int speedUpFraction = 1;
                double cumulativeWeightedBrowse = 0;

                foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
                {
                    //neighborWeight = 0.0;
                    totalNeighborWeight = 0.0;
                    maxNeighborWeight = 0.0;
                    neighborCnt = 0;

                    //if (SiteVars.SitePreference[site] > 0)
                    //{

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
                    foreach (RelativeLocationWeighted neighbor in neighborhood)
                    {
                        //Do NOT subsample if there are too few neighbors
                        //i.e., <= subsample size.
                        if (neighborhood.Count <= speedUpFraction ||
                            neighborCnt % speedUpFraction == 0)
                        {
                            Site activeSite = site.GetNeighbor(neighbor.Location);

                            //Note:  SitePreference ranges from 0 - 1.
                            //if (SiteVars.SitePreference[activeSite] > 0)
                            //{
                            totalNeighborWeight += SiteVars.SitePreference[activeSite] * neighbor.Weight;
                            maxNeighborWeight += neighbor.Weight;
                            //}
                        }
                        neighborCnt++;
                    }

                    if (maxNeighborWeight > 0.0)
                        SiteVars.NeighborhoodForage[site] = totalNeighborWeight / maxNeighborWeight;
                    else
                        SiteVars.NeighborhoodForage[site] = 0.0;
                    double weightedBrowse = SiteVars.LocalPopulation[site] * parameters.ConsumptionRate * SiteVars.NeighborhoodForage[site];

                    SiteVars.WeightedBrowse[site] = weightedBrowse;
                    cumulativeWeightedBrowse += weightedBrowse;
                    //}
                    //else
                    //    SiteVars.NeighborhoodForage[site] = 0.0;
                }
            }
                        
        }

     
        
    }
}
