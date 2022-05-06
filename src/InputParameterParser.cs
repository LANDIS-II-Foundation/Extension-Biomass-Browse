//  Authors:  Brian Miranda, Nate De Jager, Patrick Drohan

using Landis.Utilities;
using Landis.Core;
using System.Collections.Generic;

namespace Landis.Extension.Browse
{
    /// <summary>
    /// A parser that reads the plug-in's parameters from text input.
    /// </summary>
    public class InputParameterParser
        : TextParser<IInputParameters>
    {
        public static ISpeciesDataset SpeciesDataset = PlugIn.ModelCore.Species;
        //---------------------------------------------------------------------
        public override string LandisDataValue
        {
            get
            {
                return PlugIn.ExtensionName;
            }
        }
        //---------------------------------------------------------------------

        public InputParameterParser()
        {
            RegisterForInputValues();
        }

       //---------------------------------------------------------------------

        protected override IInputParameters Parse()
        {
            //const string MapNames = "SitePrefMapNames";
            
            InputVar<string> landisData = new InputVar<string>("LandisData");
            ReadVar(landisData);
            if (landisData.Value.Actual != PlugIn.ExtensionName)
                throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", PlugIn.ExtensionName);

            InputParameters parameters = new InputParameters(PlugIn.ModelCore.Species.Count);

            InputVar<int> timestep = new InputVar<int>("Timestep");
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;
            
            
            //--------- Read In Species Table ---------------------------------------
            Dictionary<string, int> lineNumbers = new Dictionary<string, int>();
            PlugIn.ModelCore.UI.WriteLine("   Begin parsing SPECIES table.");

            const string SppParms = "SpeciesTable";
            ReadName(SppParms);
            InputVar<string> sppName = new InputVar<string>("Species");
            InputVar<double> browsePref = new InputVar<double>("Browse Preference");
            InputVar<double> growthReductThresh = new InputVar<double>("Growth Reduction Threshold");
            InputVar<double> growthReductMax = new InputVar<double>("Growth Reduction Max");
            InputVar<double> mortThresh = new InputVar<double>("Mortality Threshold");
            InputVar<double> mortMax = new InputVar<double>("Mortality Max");
            InputVar<double> biomassMax = new InputVar<double>("Biomass Max");

            while ((!AtEndOfInput) && (CurrentName != "ZoneMap"))
            {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(sppName, currentLine);
                ISpecies species = SpeciesDataset[sppName.Value.Actual];
                if (species == null)
                    throw new InputValueException(sppName.Value.String,
                                                  "{0} is not a species name.",
                                                  sppName.Value.String);
                int lineNumber;
                if (lineNumbers.TryGetValue(species.Name, out lineNumber))
                    throw new InputValueException(sppName.Value.String,
                                                  "The species {0} was previously used on line {1}",
                                                  sppName.Value.String, lineNumber);
                else
                    lineNumbers[species.Name] = LineNumber;

                ISppParameters sppParms = new SppParameters();
                parameters.SppParameters[species.Index] = sppParms;
                ReadValue(browsePref, currentLine);
                sppParms.BrowsePref = browsePref.Value;

                ReadValue(growthReductThresh, currentLine);
                sppParms.GrowthReductThresh = growthReductThresh.Value;

                ReadValue(growthReductMax, currentLine);
                sppParms.GrowthReductMax = growthReductMax.Value;

                ReadValue(mortThresh, currentLine);
                sppParms.MortThresh = mortThresh.Value;

                ReadValue(mortMax, currentLine);
                sppParms.MortMax = mortMax.Value;

                ReadValue(biomassMax, currentLine);
                sppParms.BiomassMax = biomassMax.Value;

                GetNextLine();
            }

            InputVar<string> zoneMapFile = new InputVar<string>("ZoneMap");
            ReadVar(zoneMapFile);
            parameters.ZoneMapFileName = zoneMapFile.Value;
            //PopulationZones.ReadMap(parameters.ZoneMapFileName);
            ////---------------------------------------------------------------------
            ////Read in INITIAL population data:
            //InputVar<int> zoneCode = new InputVar<int>("Zone code");
            //InputVar<double> population = new InputVar<double>("Population");
            //Dictionary<int, IDynamicInputRecord[]> allData = new Dictionary<int, IDynamicInputRecord[]>();
            ////DynamicInputs.TemporalData = new Dictionary<int, IDynamicInputRecord[]>();
            //IDynamicInputRecord[] inputTable = new IDynamicInputRecord[PopulationZones.Dataset.Count];
            //parameters.TemporalData.Add(0, inputTable);
            ////DynamicInputs.TemporalData.Add(0, inputTable);
            ////PlugIn.ModelCore.UI.WriteLine("  Dynamic Input Parser:  Add new year = {0}.", yr);

            //ReadName("InitialalZonePopulations");
            //while (!AtEndOfInput && (CurrentName != "DefinedPopulationFile"))
            //{
            //    StringReader currentLine = new StringReader(CurrentLine);

            //    ReadValue(zoneCode, currentLine);

            //    IPopulationZone popZone = PopulationZones.FindZone(zoneCode.Value);

            //    IDynamicInputRecord dynamicInputRecord = new DynamicInputRecord();

            //    ReadValue(population, currentLine);
            //    dynamicInputRecord.Population = population.Value;

            //    parameters.TemporalData[0][popZone.Index] = dynamicInputRecord;

            //    CheckNoDataAfter("the " + population.Name + " column",
            //                     currentLine);

            //    GetNextLine();

            //}


            InputVar<string> populationFile = new InputVar<string>("DefinedPopulationFile");
            ReadVar(populationFile);
            parameters.PopulationFileName = populationFile.Value;

            //PlugIn.ModelCore.UI.WriteLine("DynamicPopulation is read as {0}", ReadOptionalName("DynamicPopulation"));

            if (ReadOptionalName("DynamicPopulation"))
            {
                PlugIn.DynamicPopulation = true;

                //ReadName("DynamicPopulation");
                    InputVar<double> rmin = new InputVar<double>("RMin");
                    ReadVar(rmin);
                    PlugIn.PopRMin = (double) rmin.Value;

                    InputVar<double> rmax = new InputVar<double>("RMax");
                    ReadVar(rmax);
                    PlugIn.PopRMax = rmax.Value;

                    InputVar<double> mmin = new InputVar<double>("MortalityMin");
                    ReadVar(mmin);
                    PlugIn.PopMortalityMin = mmin.Value;

                    InputVar<double> mmax = new InputVar<double>("MortalityMax");
                    ReadVar(mmax);
                    PlugIn.PopMortalityMax = mmax.Value;

                    InputVar<double> pmin = new InputVar<double>("PredationMin");
                    ReadVar(pmin);
                    PlugIn.PopPredationMin = pmin.Value;

                    InputVar<double> pmax = new InputVar<double>("PredationMax");
                    ReadVar(pmax);
                    PlugIn.PopPredationMax = pmax.Value;

                    InputVar<double> hmin = new InputVar<double>("HarvestMin");
                    ReadVar(hmin);
                    PlugIn.PopHarvestMin = hmin.Value;

                    InputVar<double> hmax = new InputVar<double>("HarvestMax");
                    ReadVar(hmax);
                    PlugIn.PopHarvestMax = hmax.Value;
                //}
                //InputVar<string> dynamicPopulationFile = new InputVar<string>("DynamicPopulationFile");
                //if (ReadOptionalVar(dynamicPopulationFile))
                //    parameters.DynamicPopulationFileName = dynamicPopulationFile.Value;
                //else
                //    parameters.DynamicPopulationFileName = null;
            }

            InputVar<double> consumptionRate = new InputVar<double>("ConsumptionRate");
            ReadVar(consumptionRate);
            parameters.ConsumptionRate = consumptionRate.Value;

            InputVar<double> anppForageProp = new InputVar<double>("ANPPForageProp");
            ReadVar(anppForageProp);
            parameters.ANPPForageProp = anppForageProp.Value;

            InputVar<double> minBrowsePropinReach = new InputVar<double>("MinBrowsePropinReach");
            ReadVar(minBrowsePropinReach);
            parameters.MinBrowsePropinReach = minBrowsePropinReach.Value;

            InputVar<double> browseBiomassThreshMin = new InputVar<double>("BrowseBiomassThresholdMin");
            ReadVar(browseBiomassThreshMin);
            parameters.BrowseBiomassThreshMin = browseBiomassThreshMin.Value;

            InputVar<double> browseBiomassThreshMax = new InputVar<double>("BrowseBiomassThresholdMax");
            ReadVar(browseBiomassThreshMax);
            parameters.BrowseBiomassThreshMax = browseBiomassThreshMax.Value;

            InputVar<double> escapeBrowsePropLong = new InputVar<double>("EscapeBrowsePropLong");
            ReadVar(escapeBrowsePropLong);
            parameters.EscapeBrowsePropLong= escapeBrowsePropLong.Value;
            
            InputVar<string> growthReduction = new InputVar<string>("GrowthReduction");
            if (ReadOptionalVar(growthReduction))
                if (growthReduction.Value.ToString().ToUpper() == "OFF")
                {
                    parameters.GrowthReduction = false;
                }
                else
                    parameters.GrowthReduction = true;
            else
                parameters.GrowthReduction = true;

            InputVar<string> mortality = new InputVar<string>("Mortality");
            if (ReadOptionalVar(mortality))
                if (mortality.Value.ToString().ToUpper() == "OFF")
                {
                    parameters.Mortality = false;
                }
                else
                    parameters.Mortality = true;
            else
                parameters.Mortality = true;

            InputVar<string> countNonForage = new InputVar<string>("CountNonForageinSitePref");
            if (ReadOptionalVar(countNonForage))
                if (countNonForage.Value.ToString().ToUpper() == "TRUE")
                {
                    parameters.CountNonForage = true;
                }
                else
                    parameters.CountNonForage = false;
            else
                parameters.CountNonForage = false;

            InputVar<string> useInitBiomass = new InputVar<string>("UseInitBiomassAsForage");
            if (ReadOptionalVar(useInitBiomass))
                if (useInitBiomass.Value.ToString().ToUpper() == "TRUE")
                {
                    parameters.UseInitBiomass = true;
                }
                else
                    parameters.UseInitBiomass = false;
            else
                parameters.UseInitBiomass = false;

            InputVar<double> forageQuantityNbrRad = new InputVar<double>("ForageQuantity");
            if (ReadOptionalVar(forageQuantityNbrRad))
                parameters.ForageQuantityNbrRad = forageQuantityNbrRad.Value;
            else
                parameters.ForageQuantityNbrRad = -999;

            InputVar<double> sitePrefNbrRad = new InputVar<double>("SitePreference");
            if (ReadOptionalVar(sitePrefNbrRad))
                parameters.SitePrefNbrRad = sitePrefNbrRad.Value;
            else
                parameters.SitePrefNbrRad = -999;

            if (parameters.SitePrefNbrRad == -999 && parameters.ForageQuantityNbrRad == -999)
                throw new InputValueException("HSI Inputs", "Either ForageQuantity or SitePreference (or both) must be listed for HSI Inputs.  Neighborhood can be 0 for either or both components");

            InputVar<string> sitePrefMapNames = new InputVar<string>("SitePrefMapNames");
            if (ReadOptionalVar(sitePrefMapNames))
                parameters.SitePrefMapNamesTemplate = sitePrefMapNames.Value;
            else
                parameters.SitePrefMapNamesTemplate = null;

            InputVar<string> siteForageMapNames = new InputVar<string>("SiteForageMapNames");
            if (ReadOptionalVar(siteForageMapNames))
                parameters.SiteForageMapNamesTemplate = siteForageMapNames.Value;
            else
                parameters.SiteForageMapNamesTemplate = null;

            InputVar<string> siteHSIMapNames = new InputVar<string>("SiteHSIMapNames");
            if(ReadOptionalVar(siteHSIMapNames))
                parameters.SiteHSIMapNamesTemplate = siteHSIMapNames.Value;
            else
                parameters.SiteHSIMapNamesTemplate = null;

            InputVar<string> sitePopMapNames = new InputVar<string>("SitePopulationMapNames");
            if(ReadOptionalVar(sitePopMapNames))
                parameters.SitePopMapNamesTemplate = sitePopMapNames.Value;
            else
                parameters.SitePopMapNamesTemplate = null;

            InputVar<string> biomassRemovedMapNames = new InputVar<string>("BiomassRemovedMapNames");
            if(ReadOptionalVar(biomassRemovedMapNames))
                parameters.BiomassRemovedMapNamesTemplate = biomassRemovedMapNames.Value;
            else
                parameters.BiomassRemovedMapNamesTemplate = null;

            InputVar<string> logFile = new InputVar<string>("LogFile");
            ReadVar(logFile);
            parameters.LogFileName = logFile.Value;

            CheckNoDataAfter(string.Format("the {0} parameter", logFile.Name));
             
            return parameters; //.GetComplete();
        }

        public static NeighborShape NSParse(string word)
        {
            if (word == "uniform")
                return NeighborShape.uniform;
            else if (word == "linear")
                return NeighborShape.linear;
            else if (word == "gaussian")
                return NeighborShape.gaussian;
            throw new System.FormatException("Valid algorithms: uniform, linear, gaussian");
        }
        public static NeighborSpeed NSpeedParse(string word)
        {
            if (word == "none")
                return NeighborSpeed.none;
            else if (word == "2x")
                return NeighborSpeed.X2;
            else if (word == "3x")
                return NeighborSpeed.X3;
            else if (word == "4x")
                return NeighborSpeed.X4;
            throw new System.FormatException("Valid algorithms:  none, 2x, 3x, 4x");
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Registers the appropriate method for reading input values.
        /// </summary>
        public static void RegisterForInputValues()
        {
            Type.SetDescription<NeighborShape>("Neighbor Shape");
            InputValues.Register<NeighborShape>(NSParse);

            Type.SetDescription<NeighborSpeed>("Neighbor Speed");
            InputValues.Register<NeighborSpeed>(NSpeedParse);
        }
    }
}
