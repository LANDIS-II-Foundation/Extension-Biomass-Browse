//  Authors:  Robert M. Scheller

using Landis.Core;
using System.Collections.Generic;
using System.IO;

namespace Landis.Extension.Browse
{

    public class DynamicInputs
    {
        //private static Dictionary<int, IDynamicInputRecord[]> allData;
        //private static IDynamicInputRecord[] timestepData;
        public static Dictionary<int, IDynamicInputRecord[]> TemporalData;

        public DynamicInputs()
        {
        }
        //---------------------------------------------------------------------
        public static void Initialize(string filename, bool writeOutput, IInputParameters parameters)
        {
            PlugIn.ModelCore.UI.WriteLine("   Loading dynamic input data from file \"{0}\" ...", filename);
            DynamicInputsParser parser = new DynamicInputsParser();
            try
            {
                bool LoadedCorrectly = Landis.Data.Load<bool>(filename, parser);
            }
            catch (FileNotFoundException)
            {
                string mesg = string.Format("Error: The file {0} does not exist", filename);
                throw new System.ApplicationException(mesg);
            }
        }
    }
}
