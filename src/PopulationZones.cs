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
                //PlugIn.ModelCore.UI.WriteLine("Population Zone {0} has Map Code {1}.", popZone.Index, popZone.MapCode);//debug
                //PlugIn.ModelCore.UI.WriteLine("Population Zone {0} has {1} cells", popZone.Index, popZone.PopulationZoneSites.Count);//debug
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

            PlugIn.ModelCore.UI.WriteLine("Dynamic Population is {0}", PlugIn.DynamicPopulation);

            // Get population for static population mode          
            if (!PlugIn.DynamicPopulation)
            {
                PlugIn.ModelCore.UI.WriteLine("Using static population");

                //if there is defined population data for the timestep
                if (DynamicInputs.TemporalData.ContainsKey(PlugIn.ModelCore.CurrentTime))
                {
                    PlugIn.ModelCore.UI.WriteLine("     Defined population being loaded for current timestep");
                    foreach (IPopulationZone popZone in Dataset)
                    {
                        if (DynamicInputs.TemporalData[PlugIn.ModelCore.CurrentTime][popZone.Index] != null)
                        {
                            //K and Effective population size are not being calculated for first timesteps,
                            //until the model encounters the next defined pop size (timestep 0 not being used for K and Eff. Pop.)

                            double newPop = DynamicInputs.TemporalData[PlugIn.ModelCore.CurrentTime][popZone.Index].Population;
                            Dataset[popZone.Index].Population = (int)(newPop);
                            Dataset[popZone.Index].K = CalculateK(popZone.Index, parameters);
                            Dataset[popZone.Index].EffectivePop = Math.Min(Dataset[popZone.Index].Population, Dataset[popZone.Index].K);

                            //PlugIn.ModelCore.UI.WriteLine("Using defined population size. PopZone Index {0}.  Population = {1}. K = {2}. EffectivePop = {3}.",
                           // popZone.Index, Dataset[popZone.Index].Population, Dataset[popZone.Index].K, Dataset[popZone.Index].EffectivePop);//debug
                        }
                    }
                }

                else
                {//If no static population is available for the timestep, use previous year's population size
                    foreach (IPopulationZone popZone in Dataset)
                    {
                        Dataset[popZone.Index].K = CalculateK(popZone.Index, parameters);
                        Dataset[popZone.Index].EffectivePop = Math.Min(Dataset[popZone.Index].Population, Dataset[popZone.Index].K);

                        PlugIn.ModelCore.UI.WriteLine("Using previous population size. PopZone Index {0}.  Population = {1}. K = {2}. EffectivePop = {3}.",
                        popZone.Index, Dataset[popZone.Index].Population, Dataset[popZone.Index].K, Dataset[popZone.Index].EffectivePop);
                    }
                }
            }

            // For dynamic population
            else
            {
                PlugIn.ModelCore.UI.WriteLine("Using dynamic population");

                

                //if there is defined population data for the timestep
                if (DynamicInputs.TemporalData.ContainsKey(PlugIn.ModelCore.CurrentTime))
                {
                    PlugIn.ModelCore.UI.WriteLine("     Defined population being loaded for current timestep");

                    foreach (IPopulationZone popZone in Dataset)
                    {
                        if (DynamicInputs.TemporalData[PlugIn.ModelCore.CurrentTime][popZone.Index] != null)
                        {
                            //K and Effective population size are not being calculated for first timesteps,
                            //until the model encounters the next defined pop size (timestep 0 not being used for K and Eff. Pop.)

                            double newPop = DynamicInputs.TemporalData[PlugIn.ModelCore.CurrentTime][popZone.Index].Population;
                            Dataset[popZone.Index].Population = (int)(newPop);
                            Dataset[popZone.Index].K = CalculateK(popZone.Index, parameters);
                            Dataset[popZone.Index].EffectivePop = Math.Min(Dataset[popZone.Index].Population, Dataset[popZone.Index].K);

                            //PlugIn.ModelCore.UI.WriteLine("Using defined population size. PopZone Index {0}.  Population = {1}. K = {2}. EffectivePop = {3}.",
                            // popZone.Index, Dataset[popZone.Index].Population, Dataset[popZone.Index].K, Dataset[popZone.Index].EffectivePop);//debug
                        }
                    }
                } 
                else
                { //If no static population is available for the timestep, calculate dynamic population
                    foreach (IPopulationZone popZone in Dataset)
                    {
                        Dataset[popZone.Index].K = CalculateK(popZone.Index, parameters);
                        Dataset[popZone.Index].Population = CalculateDynamicPop(popZone.Index, parameters);
                        Dataset[popZone.Index].EffectivePop = Math.Min(Dataset[popZone.Index].Population, Dataset[popZone.Index].K);
                        PlugIn.ModelCore.UI.WriteLine("Using dynamic population. PopZone Index {0}.  Population = {1}. K = {2}. EffectivePop = {3}.",
                            popZone.Index, Dataset[popZone.Index].Population, Dataset[popZone.Index].K, Dataset[popZone.Index].EffectivePop);
                    }
                }

                
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Estimate population from forage and Browse Density Index
        /// </summary>
        /// <param name="parameters"></param>
        public static void CalculatePopulationFromBDI(IInputParameters parameters)
        {
            PlugIn.ModelCore.UI.WriteLine("   Estimating effective Zone Population from Browse Density Index.");

            //if there is defined population data for the timestep
            if (DynamicInputs.TemporalData.ContainsKey(PlugIn.ModelCore.CurrentTime))
            {
                PlugIn.ModelCore.UI.WriteLine("     Defined browse density index being loaded for current timestep");

                foreach (IPopulationZone popZone in Dataset)
                {
                    if (DynamicInputs.TemporalData[PlugIn.ModelCore.CurrentTime][popZone.Index] != null)
                    {
                        double newBDI = DynamicInputs.TemporalData[PlugIn.ModelCore.CurrentTime][popZone.Index].Population;//"Population" input is actually BDI
                        if (newBDI > 1 | newBDI < 0)
                        {
                            string mesg = string.Format("Error: Input BDI for year {0} is greater than 1 or less than 0. Value = {0}", 
                                PlugIn.ModelCore.CurrentTime, newBDI);
                            throw new System.ApplicationException(mesg);
                        }
                        Dataset[popZone.Index].BDI = newBDI;
                        Dataset[popZone.Index].K = CalculateK(popZone.Index, parameters);
                        Dataset[popZone.Index].Population = (int)(Dataset[popZone.Index].K * newBDI);
                        Dataset[popZone.Index].EffectivePop = Math.Min(Dataset[popZone.Index].Population, Dataset[popZone.Index].K);
                        
                        //PlugIn.ModelCore.UI.WriteLine("Using defined population size. PopZone Index {0}.  Population = {1}. K = {2}. EffectivePop = {3}.",
                        // popZone.Index, Dataset[popZone.Index].Population, Dataset[popZone.Index].K, Dataset[popZone.Index].EffectivePop);//debug
                    }
                }
            }
            else
            { //If no static population is available for the timestep, calculate dynamic population
                foreach (IPopulationZone popZone in Dataset)
                {
                    Dataset[popZone.Index].K = CalculateK(popZone.Index, parameters);
                    Dataset[popZone.Index].Population = (int)(Dataset[popZone.Index].K * Dataset[popZone.Index].BDI);
                    Dataset[popZone.Index].EffectivePop = Math.Min(Dataset[popZone.Index].Population, Dataset[popZone.Index].K);
                    //PlugIn.ModelCore.UI.WriteLine("Using dynamic population. PopZone Index {0}.  Population = {1}. K = {2}. EffectivePop = {3}.",
                        //popZone.Index, Dataset[popZone.Index].Population, Dataset[popZone.Index].K, Dataset[popZone.Index].EffectivePop);
                }
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
            double zoneK = Dataset[popZoneIndex].K; //K already calculated (Line 169)
            
            PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = 0.0;
            PlugIn.ModelCore.ContinuousUniformDistribution.Beta = 1.0;
            PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = PlugIn.PopRMin;
            PlugIn.ModelCore.ContinuousUniformDistribution.Beta = PlugIn.PopRMax;
            double popR = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();
            popR = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();

            PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = 0.0;
            PlugIn.ModelCore.ContinuousUniformDistribution.Beta = 1.0;
            PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = PlugIn.PopMortalityMin;
            PlugIn.ModelCore.ContinuousUniformDistribution.Beta = PlugIn.PopMortalityMax;
            double popMortality = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();
            popMortality = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();

            PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = 0.0;
            PlugIn.ModelCore.ContinuousUniformDistribution.Beta = 1.0;
            PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = PlugIn.PopPredationMin;
            PlugIn.ModelCore.ContinuousUniformDistribution.Beta = PlugIn.PopPredationMax;
            double popPredation = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();
            popPredation = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();

            PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = 0.0;
            PlugIn.ModelCore.ContinuousUniformDistribution.Beta = 1.0;
            PlugIn.ModelCore.ContinuousUniformDistribution.Alpha = PlugIn.PopHarvestMin;
            PlugIn.ModelCore.ContinuousUniformDistribution.Beta = PlugIn.PopHarvestMax;
            double popHarvest = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();
            popHarvest = PlugIn.ModelCore.ContinuousUniformDistribution.NextDouble();

            double popGrowth = 0;
            popGrowth = popR * oldPop * (1 - (oldPop / zoneK)) - (popMortality * oldPop) - (popPredation * oldPop) - (popHarvest * oldPop);
           
            int newPop = (int)(oldPop + popGrowth); //this always rounds down -- so small populations go extinct quickly

            if (newPop < 0)
            {
                newPop = 0;
                PlugIn.ModelCore.UI.WriteLine("New population was negative, probably because oldPop was much greater than zoneK. Check initial population.");
            } else if (zoneK < 0.0001)
            {
                newPop = 0;
                PlugIn.ModelCore.UI.WriteLine("Carrying capacity = 0; check parameters.");
            }

            PlugIn.ModelCore.UI.WriteLine("oldPop = {0}.  popR = {1}. popMortality = {2}. popPredation = {3}. popHarvest = {4}. popGrowth = {5}. newPop = {6}.",
                                          oldPop, popR, popMortality, popPredation, popHarvest, popGrowth, newPop);
                     

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
            PlugIn.ModelCore.UI.WriteLine("     Calculating carrying capacity");
            double totalForage = 0;
            foreach (Location siteLocation in Dataset[popZoneIndex].PopulationZoneSites)
            {
                Site site = PlugIn.ModelCore.Landscape.GetSite(siteLocation);
                totalForage += SiteVars.ForageQuantity[site];
            }

            //TODO check on the units here -- things seem wonky in the output for forage
            double totalForage_g = totalForage * PlugIn.ModelCore.CellLength * PlugIn.ModelCore.CellLength;  // Convert g/m2 to g
            Dataset[popZoneIndex].TotalForage = (totalForage_g / 1000.0);  //Convert to kg 
            double zoneK = totalForage_g / (parameters.ConsumptionRate * 1000);  // Convert consumption kg to g
            return zoneK;
        }
        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {
            foreach (IPopulationZone popZone in Dataset)
            {
                //Load in initial populations for each zone
                if (!PlugIn.UseBDI)
                {
                    Dataset[popZone.Index].Population = (int)(DynamicInputs.TemporalData[0][popZone.Index].Population); 
                    Dataset[popZone.Index].EffectivePop = Dataset[popZone.Index].Population;
                } else
                {
                    double newBDI = DynamicInputs.TemporalData[0][popZone.Index].Population; //"Population" input is actually BDI
                    if (newBDI > 1 | newBDI <0)
                    {
                        string mesg = string.Format("Error: Input BDI for year 0 is greater than 1 or less than 0. Value = {0}", newBDI);
                        throw new System.ApplicationException(mesg);
                    }
                    Dataset[popZone.Index].BDI = newBDI;
                    Dataset[popZone.Index].K = CalculateK(popZone.Index, parameters);
                    Dataset[popZone.Index].Population = (int)(Dataset[popZone.Index].K * newBDI);
                    Dataset[popZone.Index].EffectivePop = Math.Min(Dataset[popZone.Index].Population, Dataset[popZone.Index].K);
                }
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

            foreach (IPopulationZone popZone in Dataset)
            {
                int zoneSiteCount = 0;
                double cumulativeHSI = 0;
                double cumulativeForage = 0;

                // Calculate weights from proportion of K
                // If weightForage is higher (closer to 1, for dynamic pop), then it has more influence on the local population size. 
                double weightForage = Math.Min(1,popZone.EffectivePop/popZone.K);
                
                double weightHSI = 1 - weightForage;
                // END calculate weights from proportion of K


                double cumulativeCappedSitePop = 0;
                double cumulativeCapacity = 0;
               
                foreach (Location siteLocation in Dataset[popZone.Index].PopulationZoneSites)
                {
                    // within this popZone, sum up the total HSI and Forage for all sites in the zone
                    // TODO SF I think this can be calculated elsewhere rather than looping through all sites again
                    Site site = PlugIn.ModelCore.Landscape.GetSite(siteLocation);
                    cumulativeHSI += SiteVars.HabitatSuitability[site];
                    cumulativeForage += SiteVars.ForageQuantity[site];
                }

                double sumSiteK = 0;
                double sumScaledHSI = 0;

                foreach (Location siteLocation in Dataset[popZone.Index].PopulationZoneSites)
                {
                    //We need to divide up the total population for the zone among all the sites
                    // in that zone. This is done by using the K and HSI of the sites.

                    Site site = PlugIn.ModelCore.Landscape.GetSite(siteLocation);
                    zoneSiteCount++;
                    totalSiteCount++;

                    PlugIn.ModelCore.UI.WriteLine("     Site number {0}", totalSiteCount);//debug

                    // Calculate local K

                    //siteTotalForage is total g of forage in the site
                    double siteTotalForage = SiteVars.ForageQuantity[site] * PlugIn.ModelCore.CellLength * PlugIn.ModelCore.CellLength;
                    PlugIn.ModelCore.UI.WriteLine("         siteTotalForage = {0}", siteTotalForage);//debug
                    //siteK is how many browsers could be supported by that much forage (divide by grams needed for each browser)
                    double siteK = siteTotalForage / (parameters.ConsumptionRate * 1000); // Convert consumption kg to g
                    PlugIn.ModelCore.UI.WriteLine("         siteK = {0}", siteK);//debug
                    //Check that siteK sums to popZone.K
                    sumSiteK += siteK;
                    //PlugIn.ModelCore.UI.WriteLine("         sumSiteK = {0}", sumSiteK);//debug

                    //Redistribute population according to site proportion of total zone K
                    double sitePropOptimal = 0;
                    if (popZone.K > 0)
                        sitePropOptimal = siteK / popZone.K;
                    double sitePopOptimal = popZone.EffectivePop * sitePropOptimal;
                    //PlugIn.ModelCore.UI.WriteLine("         sitePopOptimal = {0}", sitePopOptimal);//debug

                    //Browse - redistribute population according to site proportion of total zone HSI
                    double scaledHSI = 0;
                    if (cumulativeHSI > 0)
                        scaledHSI = SiteVars.HabitatSuitability[site] / cumulativeHSI;
                    double sitePopHSI = popZone.EffectivePop * scaledHSI;
                    //PlugIn.ModelCore.UI.WriteLine("         sitePopHSI = {0}", sitePopHSI);//debug
                    //Check that scaledHSI sums to 1
                    sumScaledHSI += scaledHSI;
                    //PlugIn.ModelCore.UI.WriteLine("         sumScaledHSI = {0}", sumScaledHSI); //debug
                    //Browse - END redistribute HSI population

                    //Browse - calculate weighted average population using both forage and HSI
                    // "avgSitePop" is a confusing term, because it will differ for each site
                    // depending on forage and HSI -- it's not an average across all the sites.
                    double avgSitePop = (sitePopOptimal * weightForage) + (sitePopHSI * weightHSI);
                    PlugIn.ModelCore.UI.WriteLine("         avgSitePop = {0}. {1} * {2} + {3} * {4}", avgSitePop,
                        sitePopOptimal, weightForage, sitePopHSI, weightHSI);//debug

                    //Browse - re-assign excess population on local sites
                    // In the case that the site population exceeds the carrying capacity, 
                    // we need to cap the site population to equal carrying capacity, and 
                    // move the excess population to other sites that have capacity.

                    // Initially, try to set site pop based on its Forage and HSI
                    double cappedSitePop = avgSitePop;
                   // PlugIn.ModelCore.UI.WriteLine("         cappedSitePop = {0}", cappedSitePop);//debug

                    double remainSitePopCapacity = 0;
                                        
                    if (avgSitePop > siteK)
                    {
                        remainSitePopCapacity = 0;
                        cappedSitePop = siteK;
                        //PlugIn.ModelCore.UI.WriteLine("         avgSitePop was larger than K for the site. Setting cappedSitePop = siteK = {0}", cappedSitePop);//debug
                    }
                    else
                    {
                        remainSitePopCapacity = siteK - avgSitePop;
                        //PlugIn.ModelCore.UI.WriteLine("         avgSitePop was less than siteK. remainSitePopCapacity = {0}", remainSitePopCapacity);//debug
                    }
                    
                    
                    cumulativeCappedSitePop += cappedSitePop;
                    cumulativeCapacity += remainSitePopCapacity;

                    SiteVars.SiteCapacity[site] = remainSitePopCapacity;
                    SiteVars.LocalPopulation[site] = cappedSitePop;

                    //PlugIn.ModelCore.UI.WriteLine("Excess SiteCapacity = {0}. LocalPop = {1}.", remainSitePopCapacity, cappedSitePop);
                    
                }

                //bool checkK = (sumSiteK == popZone.K);
                if (Math.Round(cumulativeCappedSitePop) < popZone.EffectivePop)
                    //This happens if at least one of the sites had a population size > K 
                    // (so the CappedSitePop for a site was less than the avgSitePop).
                {
                    foreach (Location siteLocation in Dataset[popZone.Index].PopulationZoneSites)
                    {
                        Site site = PlugIn.ModelCore.Landscape.GetSite(siteLocation);
                        double redistribPop = 0;
                        if (SiteVars.SiteCapacity[site] > 0)
                        {
                            redistribPop = (popZone.EffectivePop - cumulativeCappedSitePop) * (SiteVars.SiteCapacity[site] / cumulativeCapacity);
                            //PlugIn.ModelCore.UI.WriteLine("redistribPop = {0}", redistribPop); //debug

                            SiteVars.LocalPopulation[site] += redistribPop;                            

                            //PlugIn.ModelCore.UI.WriteLine("Final pop = {0}", SiteVars.LocalPopulation[site]);//debug
                        }
                        //Browse - END re-assign excess population on local sites

                    }
                }
            }
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
        /// <summary>
        /// Calculate browse to be removed at each site based on local population
        /// </summary>
        /// <param name="parameters"></param>
        public static void CalculateBrowseToRemove(IInputParameters parameters)
        {
            foreach (IPopulationZone popZone in Dataset)
            {
               
                foreach (Location siteLocation in Dataset[popZone.Index].PopulationZoneSites)
                {
                    Site site = PlugIn.ModelCore.Landscape.GetSite(siteLocation);
                    
                    //Browse - calculate local browse to remove
                    //convert number of individuals to grams of forage consumed per meter squared
                    double siteBrowseToBeRemoved = (SiteVars.LocalPopulation[site] * (parameters.ConsumptionRate * 1000)) / (PlugIn.ModelCore.CellLength * PlugIn.ModelCore.CellLength);

                    SiteVars.TotalBrowse[site] = siteBrowseToBeRemoved;
                    
                    //Browse - END calculate local browse to remove
                }
            }
        }
        //---------------------------------------------------------------------
    }
}
