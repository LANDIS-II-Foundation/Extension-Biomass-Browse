﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;
using Landis.Library.BiomassCohorts;

namespace Landis.Extension.Browse
{
    public class CalibrateLog
    {

        public static Dictionary<int, Dictionary<int, double[]>> CohortCalibrationData = new Dictionary<int, Dictionary<int, double[]>>();

        public static void WriteLogFile(int year)
        {

            foreach (int sppkey in CohortCalibrationData.Keys)
            {
                //PlugIn.ModelCore.UI.WriteLine("sppkey = {0}", sppkey);
                Dictionary<int, double[]> cohortDict;
                CohortCalibrationData.TryGetValue(sppkey, out cohortDict);
                
                //cohortDict.ToList().ForEach(x => PlugIn.ModelCore.UI.WriteLine("age keys in cohortDict = {0}", x.Key));

                foreach (int agekey in cohortDict.Keys)
                {

                    PlugIn.calibrateLog.Clear();
                    CalibrateLog clog = new CalibrateLog();

                    //PlugIn.ModelCore.UI.WriteLine("agekey = {0}", agekey);

                    double[] cohortData;
                    cohortDict.TryGetValue(agekey, out cohortData);

                    clog.Year = year;
                    clog.CohortAge = agekey;
                    clog.CohortCode = sppkey;
                    clog.CohortName = PlugIn.ModelCore.Species[sppkey].Name;
                    clog.GrowthReduction = (short) cohortData[0];
                    clog.NewForageInReach = (int) cohortData[1]; 
                    clog.FirstPassRemoval = (int)cohortData[2]; 
                    clog.SecondPassRemoval = (int)cohortData[3]; 
                    clog.FinalRemoval = (int)cohortData[4]; 
                    clog.NewForage = (int)cohortData[5]; 
                    clog.LastBrowseProportion = (int)cohortData[6]; 
                    clog.ForageInReach = (int)cohortData[7];
                    clog.ProportionBrowsed = (short)cohortData[8];

                    PlugIn.calibrateLog.AddObject(clog);
                    PlugIn.calibrateLog.WriteToFile();


                }
            }
        }


        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "...")]
        public int Year { get; set; }

        [DataFieldAttribute(Desc = "Age of Cohort", Unit = "Years")]
        public int CohortAge { get; set; }

        [DataFieldAttribute(Desc = "SpeciesCode")]
        public int CohortCode { get; set; }
        [DataFieldAttribute(Desc = "SpeciesIndex")]
        public string CohortName { get; set; }

        [DataFieldAttribute(Unit = "Proportion", Desc = "Growth Reduction B")]
        public short GrowthReduction { get; set; } // index 0

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int NewForageInReach { get; set; } // index 1

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int FirstPassRemoval { get; set; } // index 2

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int SecondPassRemoval { get; set; } // index 3
        
        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int FinalRemoval { get; set; } // index 4

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int NewForage { get; set; } // index 5

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int LastBrowseProportion { get; set; } // index 6

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int ForageInReach { get; set; } // index 7

        [DataFieldAttribute(Unit = "Proportion")]
        public short ProportionBrowsed { get; set; } // index 8


        public static void SetCalibrateData(ICohort cohort, int index, double newValue)
        {
            int cohortAddYear = SiteVars.GetAddYear(cohort);
            Dictionary<int, double[]> cohortDict;
            double[] oldValue;

            //PlugIn.ModelCore.UI.WriteLine("cohort species = {0}, species index = {1}, cohort add year = {2}, calibrate index = {3}", 
             //   cohort.Species.Name, cohort.Species.Index, cohortAddYear, index);

            // If the dictionary entry exists for the cohort, overwrite it:
            if (CohortCalibrationData.TryGetValue(cohort.Species.Index, out cohortDict))
                if (cohortDict.TryGetValue(cohortAddYear, out oldValue))
                {
                 //   PlugIn.ModelCore.UI.WriteLine("Replacing values for cohort in calibrate log");
                    CohortCalibrationData[cohort.Species.Index][cohortAddYear][index] = newValue;
                    return;
                }

            // If the dictionary does not exist for the cohort, create it:
            Dictionary<int, double[]> newEntry = new Dictionary<int, double[]>();
            double[] newArray = new double[9]; //SF update this number when adding new calibration variable -- n+1
            newArray[index] = newValue;
            newEntry.Add(cohortAddYear, newArray);

            if (CohortCalibrationData.ContainsKey(cohort.Species.Index))
            {
              //  PlugIn.ModelCore.UI.WriteLine("Adding species to calibrate log");
                CohortCalibrationData[cohort.Species.Index].Add(cohortAddYear, newArray);
            }
            else
            {
              //  PlugIn.ModelCore.UI.WriteLine("Adding species to calibrate log");
                CohortCalibrationData.Add(cohort.Species.Index, newEntry);
            }
        }


    }
}
