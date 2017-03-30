using System.Collections.Generic;
using System.IO;

namespace Landis.Extension.DeerBrowse
{
    class DynamicPopulation
    {
        private static double popRMin;
        private static double popRMax;
        private static double popMortalityMin;
        private static double popMortalityMax;
        private static double popHarvestMin;
        private static double popHarvestMax; 
        private static double popPredationMin;
        private static double popPredationMax;

        private static List<double> dynamicPopParameters;

        public DynamicPopulation()
        {
        }

        //---------------------------------------------------------------------
        public static double PopRMin
        {
            get
            {
                return popRMin;
            }
            set
            {
                popRMin = value;
            }
        }
        //---------------------------------------------------------------------
        public static double PopRMax
        {
            get
            {
                return popRMax;
            }
            set
            {
                popRMax = value;
            }
        }
        //---------------------------------------------------------------------
        public static double PopMortalityMin
        {
            get
            {
                return popMortalityMin;
            }
            set
            {
                popMortalityMin = value;
            }
        }
        //---------------------------------------------------------------------
        public static double PopMortalityMax
        {
            get
            {
                return popMortalityMax;
            }
            set
            {
                popMortalityMax = value;
            }
        }
        //---------------------------------------------------------------------
        public static double PopHarvestMin
        {
            get
            {
                return popHarvestMin;
            }
            set
            {
                popHarvestMin = value;
            }
        }
        //---------------------------------------------------------------------
        public static double PopHarvestMax
        {
            get
            {
                return popHarvestMax;
            }
            set
            {
                popHarvestMax = value;
            }
        }
        //---------------------------------------------------------------------
        public static double PopPredationMax
        {
            get
            {
                return popPredationMax;
            }
            set
            {
                popPredationMax = value;
            }
        }

        //---------------------------------------------------------------------
        public static double PopPredationMin
        {
            get
            {
                return popPredationMin;
            }
            set
            {
                popPredationMin = value;
            }
        }

        //---------------------------------------------------------------------
        public static void Initialize(string filename, bool writeOutput)
        {
            if (filename != null)
            {
                PlugIn.ModelCore.UI.WriteLine("   Loading dynamic population data from file \"{0}\" ...", filename);
                DynamicPopulationParser parser = new DynamicPopulationParser();
                try
                {
                    dynamicPopParameters = Landis.Data.Load<List<double>>(filename, parser);
                    DynamicPopulation.PopRMin = dynamicPopParameters[0];
                    DynamicPopulation.PopRMax = dynamicPopParameters[1];
                    DynamicPopulation.PopMortalityMin = dynamicPopParameters[2];
                    DynamicPopulation.PopMortalityMax = dynamicPopParameters[3];
                    DynamicPopulation.PopPredationMin = dynamicPopParameters[4];
                    DynamicPopulation.PopPredationMax = dynamicPopParameters[5];
                    DynamicPopulation.PopHarvestMin = dynamicPopParameters[6];
                    DynamicPopulation.PopHarvestMax = dynamicPopParameters[7];

                }
                catch (FileNotFoundException)
                {
                    string mesg = string.Format("Error: The file {0} does not exist", filename);
                    throw new System.ApplicationException(mesg);
                }
            }
        }

    }
}
