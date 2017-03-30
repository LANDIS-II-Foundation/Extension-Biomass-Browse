using System.Collections.Generic;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.DeerBrowse
{
    /// <summary>
    /// A parser that reads the plug-in's parameters from text input.
    /// </summary>
    class DynamicPopulationParser: TextParser<List<double>>
    {
        private string FileName = "Dynamic Ungulate Population";
        //---------------------------------------------------------------------
        public override string LandisDataValue
        {
            get
            {
                return FileName;
            }
        }
        //---------------------------------------------------------------------
        protected override List<double> Parse()
        {
            InputVar<string> landisData = new InputVar<string>("LandisData");
            ReadVar(landisData);
            if (landisData.Value.Actual != FileName)
                throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", FileName);

            List<double> dynamicPopParameters = new List<double>();

            InputVar<double> popRMin = new InputVar<double>("RMin");
            InputVar<double> popRMax = new InputVar<double>("RMax");
            InputVar<double> popMortalityMin = new InputVar<double>("MortalityMin");
            InputVar<double> popMortalityMax = new InputVar<double>("MortalityMax");
            InputVar<double> popPredationMin = new InputVar<double>("PredationMin");
            InputVar<double> popPredationMax = new InputVar<double>("PredationMax");
            InputVar<double> popHarvestMin = new InputVar<double>("HarvestMin");
            InputVar<double> popHarvestMax = new InputVar<double>("HarvestMax");

            

            ReadName("R");
            StringReader currentLine = new StringReader(CurrentLine);
            ReadValue(popRMin,currentLine);
            ReadValue(popRMax, currentLine);
            dynamicPopParameters.Add(popRMin.Value);
            dynamicPopParameters.Add(popRMax.Value);
            GetNextLine();

            ReadName("Mortality");
            currentLine = new StringReader(CurrentLine);
            ReadValue(popMortalityMin,currentLine);
            ReadValue(popMortalityMax, currentLine);
            dynamicPopParameters.Add(popMortalityMin.Value);
            dynamicPopParameters.Add(popMortalityMax.Value);
            GetNextLine();

            ReadName("Predation");
            currentLine = new StringReader(CurrentLine);
            ReadValue(popPredationMin,currentLine);
            ReadValue(popPredationMax, currentLine);
            dynamicPopParameters.Add(popPredationMin.Value);
            dynamicPopParameters.Add(popPredationMax.Value);
            GetNextLine();
            
            ReadName("Harvest");
            currentLine = new StringReader(CurrentLine);
            ReadValue(popHarvestMin,currentLine);
            ReadValue(popHarvestMax, currentLine);
            dynamicPopParameters.Add(popHarvestMin.Value);
            dynamicPopParameters.Add(popHarvestMax.Value);
            GetNextLine();
      
            return dynamicPopParameters;
        }
    }
}
