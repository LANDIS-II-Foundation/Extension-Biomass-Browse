//  Authors:  Robert M. Scheller, Brian R. Miranda 

using Landis.SpatialModeling;
using System.IO;
using System.Collections.Generic;
using System;


namespace Landis.Extension.Browse
{
    public class PopulationZones
    {
        public static List<IPopulationZone> Dataset;

        //---------------------------------------------------------------------

        public static void ReadMap(string path)
        {
            //List<IPopulationZone> dataset = new List<IPopulationZone>(0);
            Dataset = new List<IPopulationZone>(0); // dataset;
            IInputRaster<ShortPixel> map;

            try
            {
                map = PlugIn.ModelCore.OpenRaster<ShortPixel>(path);
            }
            catch (FileNotFoundException)
            {
                string mesg = string.Format("Error: The file {0} does not exist", path);
                throw new System.ApplicationException(mesg);
            }

            if (map.Dimensions != PlugIn.ModelCore.Landscape.Dimensions)
            {
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }

            using (map) {
                ShortPixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    int mapCode = pixel.MapCode.Value;
                    int index = Dataset.Count;
                    if (site.IsActive)
                    {
                        IPopulationZone popZone = new PopulationZone(index, mapCode);
                        //if (Dataset.Count == 0)
                        if (FindZone(mapCode) == null)
                        {
                            Dataset.Add(popZone);
                        }
                        else
                        {
                            popZone = FindZone(mapCode);
                        }
                        
                        SiteVars.PopulationZone[site] = popZone.MapCode;
                        popZone.PopulationZoneSites.Add(site.Location);
                    }
                }
            }

            foreach (IPopulationZone popZone in PopulationZones.Dataset)
            {
                PlugIn.ModelCore.UI.WriteLine("Population Zone {0} has Map Code {1}.", popZone.Index, popZone.MapCode);

            }

        }
        //---------------------------------------------------------------------
        public static IPopulationZone FindZone(int mapCode)
        {
            foreach (IPopulationZone popZone in Dataset)
            {
                if (popZone.MapCode == mapCode)
                {
                    //PlugIn.ModelCore.UI.WriteLine("PopZone mapCode {0}.  Find {1}", popZone.MapCode, mapCode);
                    return popZone;
                }
            }
            return null;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Read population from DynamicInputs or call CalculateDynamicPop (if dynamic population)
        /// </summary>
        /// <param name="parameters"></param>
        public static void CalculatePopulation(IInputParameters parameters)
        {
            PlugIn.ModelCore.UI.WriteLine("   Calculating Zone Population.");
            // Read from defined populations
            if (!PlugIn.DynamicPopulation)
            {
                if (DynamicInputs.TemporalData.ContainsKey(PlugIn.ModelCore.CurrentTime))
                {
                    foreach (IPopulationZone popZone in Dataset)
                    {
                        if (DynamicInputs.TemporalData[PlugIn.ModelCore.CurrentTime][popZone.Index] != null)
                        {
                            double newPop = DynamicInputs.TemporalData[PlugIn.ModelCore.CurrentTime][popZone.Index].Population;
                            Dataset[popZone.Index].Population = (int)(newPop * Dataset[popZone.Index].PopulationZoneSites.Count);
                            Dataset[popZone.Index].K = CalculateK(popZone.Index, parameters);
                            Dataset[popZone.Index].EffectivePop = Math.Min(Dataset[popZone.Index].Population, Dataset[popZone.Index].K);
                        }
                        else
                        {
                            Dataset[popZone.Index].K = CalculateK(popZone.Index, parameters);
                            Dataset[popZone.Index].EffectivePop = Math.Min(Dataset[popZone.Index].Population, Dataset[popZone.Index].K);

                        }
                        //else if (parameters.DynamicPopulationFileName != null)
                        //{
                        //    Dataset[popZone.Index].Population = CalculateDynamicPop(popZone.Index, parameters);
                        //    Dataset[popZone.Index].EffectivePop = Math.Min(Dataset[popZone.Index].Population, Dataset[popZone.Index].K);
                        //}
                    }
                }
            }
            else
            {
                //if (parameters.DynamicPopulationFileName != null)
                //{
                    foreach (IPopulationZone popZone in Dataset)
                    {

                        Dataset[popZone.Index].Population = CalculateDynamicPop(popZone.Index, parameters);
                        Dataset[popZone.Index].EffectivePop = Math.Min(Dataset[popZone.Index].Population, Dataset[popZone.Index].K);
                    }
                //}
                //else  //Non-dynamic population
                //{
                //    foreach (IPopulationZone popZone in Dataset)
                //    {
                //        Dataset[popZone.Index].K = CalculateK(popZone.Index, parameters);
                //    }

                //}
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Calculate dynamic landscape population based on current pop, population parameters
        /// and K calculated from total landscape forage quantity
        /// </summary>
        /// <param name="popZoneIndex"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int CalculateDynamicPop(int popZoneIndex, IInputParameters parameters)
        {
            double oldPop = Dataset[popZoneIndex].EffectivePop;
            double zoneK = CalculateK(popZoneIndex, parameters);
            Dataset[popZoneIndex].K = zoneK;

            PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = PlugIn.PopRMin;
            PlugIn.ModelCore.ContinuousUniformDistribution.Beta = PlugIn.PopRMax;
            double popR = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();
            popR = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();

            PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = PlugIn.PopMortalityMin;
            PlugIn.ModelCore.ContinuousUniformDistribution.Beta = PlugIn.PopMortalityMax;
            double popMortality = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();
            popMortality = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();

            PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = PlugIn.PopPredationMin;
            PlugIn.ModelCore.ContinuousUniformDistribution.Beta = PlugIn.PopPredationMax;
            double popPredation = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();
            popPredation = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();

            PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = PlugIn.PopHarvestMin;
            PlugIn.ModelCore.ContinuousUniformDistribution.Beta = PlugIn.PopHarvestMax;
            double popHarvest = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();
            popHarvest = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();
            
            double popGrowth = popR * oldPop * (1 - (oldPop / zoneK)) - (popMortality * oldPop) - (popPredation * oldPop) - (popHarvest * oldPop);
            
            int newPop = (int)(oldPop + popGrowth);

            return newPop;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Calculate landscape carrying capacity (K) based on total landscape forage quantity
        /// </summary>
        /// <param name="popZoneIndex"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static double CalculateK(int popZoneIndex, IInputParameters parameters)
        {
            double totalForage = 0;
            foreach (Location siteLocation in Dataset[popZoneIndex].PopulationZoneSites)
            {
                Site site = PlugIn.ModelCore.Landscape.GetSite(siteLocation);
                totalForage += SiteVars.ForageQuantity[site];
            }
            double totalForage_g = totalForage * PlugIn.ModelCore.CellLength * PlugIn.ModelCore.CellLength;  // Convert g/m2 to g
            Dataset[popZoneIndex].TotalForage = (totalForage_g / 1000.0);  //Convert to kg
            double zoneK = totalForage_g / (parameters.ConsumptionRate * 1000);  // Convert consumption kg to g
            return zoneK;
        }
        //---------------------------------------------------------------------
        public static void Initialize()// Dictionary<int, IDynamicInputRecord[]> allData, IInputParameters parameters)
        {
            foreach (IPopulationZone popZone in Dataset)
            {
                Dataset[popZone.Index].Population = (int)(DynamicInputs.TemporalData[0][popZone.Index].Population * Dataset[popZone.Index].PopulationZoneSites.Count);
                //if (parameters.DynamicPopulationFileName != null)
                //{
                //    Dataset[popZone.Index].Population = (int)(allData[0][popZone.Index].Population);
                //}
                //else
                //{
                //    Dataset[popZone.Index].Population = (int)(allData[0][popZone.Index].Population * Dataset[popZone.Index].PopulationZoneSites.Count);
                //}
                //Dataset[popZone.Index].K = CalculateK (popZone.Index, parameters);
                Dataset[popZone.Index].EffectivePop = Dataset[popZone.Index].Population;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Calculate local population based on relative HSI within zone
        /// </summary>
        /// <param name="parameters"></param>
        public static void CalculateLocalPopulation(IInputParameters parameters)
        {
            PlugIn.ModelCore.UI.WriteLine("   Calculating Local Population.");
            int totalSiteCount = 0;
            /*//Debugging
            StreamWriter localPopCellLog;
            localPopCellLog = Landis.Data.CreateTextFile("J:/LANDIS_II/code/deer-browse/trunk/tests/biomass/browse/localPopCellLog.csv");
            localPopCellLog.AutoFlush = true;
            localPopCellLog.Write("Cell, Zone, Population");
            localPopCellLog.WriteLine("");
            */

            foreach (IPopulationZone popZone in Dataset)
            {
                int zoneSiteCount = 0;
                double cumulativeHSI = 0;
                double cumulativeForage = 0;
                // Calculate weights from proportion of K
                double weightForage = Math.Min(1,popZone.EffectivePop/popZone.K);
                if (parameters.DynamicPopulationFileName == null)
                    weightForage = ((double)Dataset[popZone.Index].EffectivePop)/((double)popZone.PopulationZoneSites.Count);
                double weightHSI = 1 - weightForage;
                // END calculate weights from proportion of K

                double cumulativeCappedSitePop = 0;
                double cumulativeCapacity = 0;
               
                foreach (Location siteLocation in Dataset[popZone.Index].PopulationZoneSites)
                {
                    Site site = PlugIn.ModelCore.Landscape.GetSite(siteLocation);
                    cumulativeHSI += SiteVars.HabitatSuitability[site];
                    cumulativeForage += SiteVars.ForageQuantity[site];
                }
                //double finalPopSum = 0;
                //double sumSiteK = 0;
                //double sumScaledHSI = 0;
                foreach (Location siteLocation in Dataset[popZone.Index].PopulationZoneSites)
                {
                    Site site = PlugIn.ModelCore.Landscape.GetSite(siteLocation);
                    zoneSiteCount++;
                    totalSiteCount++;
               
                    // Calculate local K
                    // convert forage/m2 to total forage (g)
                    double siteTotalForage = SiteVars.ForageQuantity[site] * PlugIn.ModelCore.CellLength * PlugIn.ModelCore.CellLength;
                    double siteK = siteTotalForage / (parameters.ConsumptionRate * 1000); // Convert consumption kg to g
                    //Check that siteK sums to popZone.K
                    //sumSiteK += siteK;

                    //Redistribute optimal population
                    double sitePropOptimal = 0;
                    if (popZone.K > 0)
                        sitePropOptimal = siteK / popZone.K;
                    double sitePopOptimal = popZone.EffectivePop * sitePropOptimal;

                    //Browse - redistribute HSI population
                    //double sumHSI = siteHSI + (neighborHSI * (landscapeCells - 1));
                    double scaledHSI = 0;
                    if (cumulativeHSI > 0)
                        scaledHSI = SiteVars.HabitatSuitability[site] / cumulativeHSI;
                    double sitePopHSI = popZone.EffectivePop * scaledHSI;
                    //Check that scaledHSI sums to 1
                    //sumScaledHSI += scaledHSI;
                    //Browse - END redistribute HSI population

                    //Browse - calculate weighted average population
                    double avgSitePop = (sitePopOptimal * weightForage) + (sitePopHSI * weightHSI);

                    //Browse - re-assign excess population on local sites
                    double cappedSitePop = avgSitePop;

                    double remainSitePopCapacity = 0;
                    if (parameters.DynamicPopulationFileName != null)
                    {
                        if (avgSitePop > siteK)
                        {
                            remainSitePopCapacity = 0;
                            cappedSitePop = siteK;
                        }
                        else
                        {
                            remainSitePopCapacity = siteK - avgSitePop;
                        }
                    }
                    else
                    {
                        if (avgSitePop > 1.0)
                        {
                            remainSitePopCapacity = 0;
                            cappedSitePop = 1.0;
                        }
                        else if (siteTotalForage > 0)
                        {
                            remainSitePopCapacity = 1.0 - avgSitePop;
                        }
                    }
                    cumulativeCappedSitePop += cappedSitePop;
                    cumulativeCapacity += remainSitePopCapacity;

                    SiteVars.SiteCapacity[site] = remainSitePopCapacity;
                    SiteVars.LocalPopulation[site] = cappedSitePop;
                    
                }
                //bool checkK = (sumSiteK == popZone.K);
                if (Math.Round(cumulativeCappedSitePop) < popZone.EffectivePop)
                {
                    foreach (Location siteLocation in Dataset[popZone.Index].PopulationZoneSites)
                    {
                        Site site = PlugIn.ModelCore.Landscape.GetSite(siteLocation);
                        double redistribPop = 0;
                        if (SiteVars.SiteCapacity[site] > 0)
                        {
                            redistribPop = (popZone.EffectivePop - cumulativeCappedSitePop) * (SiteVars.SiteCapacity[site] / cumulativeCapacity);
                            double finalPop = SiteVars.LocalPopulation[site] + redistribPop;
                            SiteVars.LocalPopulation[site] = finalPop;
                        }
                        //Browse - END re-assign excess population on local sites

                    }
                }
                /*
                // Test that final population sums to popZone.Population
                foreach (Location siteLocation in Dataset[popZone.Index].PopulationZoneSites)
                {
                    Site site = PlugIn.ModelCore.Landscape.GetSite(siteLocation);
                    finalPopSum += SiteVars.LocalPopulation[site];
                    localPopCellLog.Write("{0},{1},{2}",
                                   site.Location.ToString(),
                                   popZone.Index,
                                   SiteVars.LocalPopulation[site]);
                    localPopCellLog.WriteLine("");
                }
                bool testPop = (finalPopSum == popZone.EffectivePop);
                bool testSites = (zoneSiteCount == Dataset[popZone.Index].PopulationZoneSites.Count);
                bool testHSI = (sumScaledHSI == 1);
                */
            }
            //bool testAllSites = (totalSiteCount == PlugIn.ModelCore.Landscape.ActiveSiteCount);
        }
        //---------------------------------------------------------------------
        public static void CalcNeighborhoodForage(IInputParameters parameters)
        {
            PlugIn.ModelCore.UI.WriteLine("   Calculating Neighborhood Forage.");
            foreach (IPopulationZone popZone in Dataset)
            {
                double totalNeighborWeight = 0.0;
                double maxNeighborWeight = 0.0;
                int neighborCnt = 0;
                //int speedUpFraction = (int)parameters.NeighborSpeedUp + 1;
                int speedUpFraction = 1;
                double cumulativeWeightedBrowse = 0;

                foreach (Location siteLocation in Dataset[popZone.Index].PopulationZoneSites)
                {
                    Site site = PlugIn.ModelCore.Landscape.GetSite(siteLocation);
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
                    double weightedBrowse = SiteVars.LocalPopulation[site] * (parameters.ConsumptionRate * 1000) * SiteVars.NeighborhoodForage[site];// Convert consumption kg to g

                    SiteVars.WeightedBrowse[site] = weightedBrowse;
                    cumulativeWeightedBrowse += weightedBrowse;
                    //}
                    //else
                    //    SiteVars.NeighborhoodForage[site] = 0.0;
                }
                popZone.WeightedBrowse = cumulativeWeightedBrowse;

            }
        }
        //---------------------------------------------------------------------
        /*public static void CalculateBrowseToRemove(IInputParameters parameters)
        {
            foreach (IPopulationZone popZone in Dataset)
            {
                double sumCappedBrowse = 0;
                double sumRemainBrowse = 0;

                foreach (Location siteLocation in Dataset[popZone.Index].PopulationZoneSites)
                {
                    Site site = PlugIn.ModelCore.Landscape.GetSite(siteLocation);
                    double rescaleBrowse = SiteVars.WeightedBrowse[site] * popZone.Population * (parameters.ConsumptionRate * 1000) / popZone.WeightedBrowse;// Convert consumption kg to g
                    double cappedBrowse = rescaleBrowse;
                    if (rescaleBrowse > SiteVars.ForageQuantity[site])
                    {
                        cappedBrowse = SiteVars.ForageQuantity[site];
                    }
                    sumCappedBrowse += cappedBrowse;
                    SiteVars.CappedBrowse[site] = cappedBrowse;
                    double remainBrowse = SiteVars.ForageQuantity[site] - cappedBrowse;
                    sumRemainBrowse += remainBrowse;
                    SiteVars.RemainBrowse[site] = remainBrowse;
                }
                foreach (Location siteLocation in Dataset[popZone.Index].PopulationZoneSites)
                {
                    Site site = PlugIn.ModelCore.Landscape.GetSite(siteLocation);
                    double reallocBrowse = (popZone.Population * (parameters.ConsumptionRate * 1000) - sumCappedBrowse) * (SiteVars.RemainBrowse[site] / sumRemainBrowse);// Convert consumption kg to g
                    double totalBrowseRemoved = SiteVars.CappedBrowse[site] + reallocBrowse;
                    SiteVars.TotalBrowse[site] = totalBrowseRemoved;
                }

            }
        }
        */
        //---------------------------------------------------------------------
        /// <summary>
        /// Calculate browse to be removed at each site based on local population
        /// </summary>
        /// <param name="parameters"></param>
        public static void CalculateBrowseToRemove(IInputParameters parameters)
        {
            foreach (IPopulationZone popZone in Dataset)
            {
                //double totalPop = 0;
                //double totalToRemove = 0;
                foreach (Location siteLocation in Dataset[popZone.Index].PopulationZoneSites)
                {
                    Site site = PlugIn.ModelCore.Landscape.GetSite(siteLocation);
                    //Browse - calculate local browse to remove
                    // convert to g/m2
                    double siteBrowseToBeRemoved = (SiteVars.LocalPopulation[site] * (parameters.ConsumptionRate * 1000)) / (PlugIn.ModelCore.CellLength * PlugIn.ModelCore.CellLength);
                    //totalPop += SiteVars.LocalPopulation[site];
                    if (parameters.DynamicPopulationFileName == null)
                        // If non-dynamic pop, then population value = rate of removal
                        siteBrowseToBeRemoved = SiteVars.ForageQuantity[site] * SiteVars.LocalPopulation[site];
                    SiteVars.TotalBrowse[site] = siteBrowseToBeRemoved;
                    //totalToRemove += siteBrowseToBeRemoved;
                    //Browse - END calculate local browse to remove
                }

                //bool testPop = (totalPop == popZone.EffectivePop);
                //bool testRemove = (totalToRemove == (popZone.EffectivePop * parameters.ConsumptionRate));

            }
        }
        //---------------------------------------------------------------------
    }
}
