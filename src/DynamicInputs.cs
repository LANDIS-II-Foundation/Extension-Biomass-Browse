//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using System.Collections.Generic;
using System.IO;

namespace Landis.Extension.Browse
{

    public class DynamicInputs
    {
        private static Dictionary<int, IDynamicInputRecord[]> allData;
        private static IDynamicInputRecord[] timestepData;

        public DynamicInputs()
        {
        }

        public static Dictionary<int, IDynamicInputRecord[]> AllData
        {
            get {
                return allData;
            }
        }
        //---------------------------------------------------------------------
        public static IDynamicInputRecord[] TimestepData
        {
            get {
                return timestepData;
            }
            set {
                timestepData = value;
            }
        }

        public static void Write()
        {
            foreach(ISpecies species in PlugIn.ModelCore.Species)
            {
                foreach(IPopulationZone popZone in PopulationZones.Dataset)
                {
                    PlugIn.ModelCore.UI.WriteLine("Zone={1}, Population={2:0.0}.", popZone.MapCode,
                        timestepData[popZone.MapCode].Population);

                }
            }

        }
        //---------------------------------------------------------------------
        public static void Initialize(string filename, bool writeOutput, IInputParameters parameters)
        {
            PlugIn.ModelCore.UI.WriteLine("   Loading dynamic input data from file \"{0}\" ...", filename);
            DynamicInputsParser parser = new DynamicInputsParser();
            try
            {
                allData = Landis.Data.Load<Dictionary<int, IDynamicInputRecord[]>>(filename, parser);
            }
            catch (FileNotFoundException)
            {
                string mesg = string.Format("Error: The file {0} does not exist", filename);
                throw new System.ApplicationException(mesg);
            }
            PopulationZones.Initialize(allData, parameters);

        }
    }

}
