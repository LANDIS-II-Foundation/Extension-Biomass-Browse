//  Authors:  Brian Miranda, Nate De Jager, Patrick Drohan

using Landis.Core;
using System.Collections.Generic;
using Landis.Utilities;

namespace Landis.Extension.DeerBrowse
{
    /// <summary>
    /// A parser that reads the tool parameters from text input.
    /// </summary>
    public class DynamicInputsParser
        : TextParser<Dictionary<int, IDynamicInputRecord[]>>
    {

        private string FileName = "Defined Ungulate Population";
        //---------------------------------------------------------------------
        public override string LandisDataValue
        {
            get
            {
                return FileName;
            }
        }
        //---------------------------------------------------------------------
        public DynamicInputsParser()
        {
        }

        //---------------------------------------------------------------------        

        protected override Dictionary<int, IDynamicInputRecord[]> Parse()
        {

            InputVar<string> landisData = new InputVar<string>("LandisData");
            ReadVar(landisData);
            if (landisData.Value.Actual != FileName)
                throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", FileName);
            
            Dictionary<int, IDynamicInputRecord[]> allData = new Dictionary<int, IDynamicInputRecord[]>();

            //---------------------------------------------------------------------
            //Read in population data:
            InputVar<int>    year       = new InputVar<int>("Time step for updating values");
            InputVar<int> zoneCode = new InputVar<int>("Zone code");
            InputVar<double> population = new InputVar<double>("Population");

            while (! AtEndOfInput)
            {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(year, currentLine);
                int yr = year.Value.Actual;

                if(!allData.ContainsKey(yr))
                {
                    IDynamicInputRecord[] inputTable = new IDynamicInputRecord[PopulationZones.Dataset.Count];
                    allData.Add(yr, inputTable);
                    PlugIn.ModelCore.UI.WriteLine("  Dynamic Input Parser:  Add new year = {0}.", yr);
                }

                ReadValue(zoneCode, currentLine);

                IPopulationZone popZone = PopulationZones.FindZone(zoneCode.Value);

                IDynamicInputRecord dynamicInputRecord = new DynamicInputRecord();

                ReadValue(population, currentLine);
                dynamicInputRecord.Population = population.Value;

                allData[yr][popZone.Index] = dynamicInputRecord;

                CheckNoDataAfter("the " + population.Name + " column",
                                 currentLine);

                GetNextLine();

            }

            return allData;
        }

        //---------------------------------------------------------------------


        private ISpecies GetSpecies(InputValue<string> speciesName)
        {
            ISpecies species = PlugIn.ModelCore.Species[speciesName.Actual];
            if (species == null)
                throw new InputValueException(speciesName.String,
                                              "{0} is not a recognized species name.",
                                              speciesName.String);

            return species;
        }


    }
}
