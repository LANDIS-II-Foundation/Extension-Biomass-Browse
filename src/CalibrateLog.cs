using Landis.Library.Metadata;
using Landis.Library.UniversalCohorts;
using Landis.Core;
using Landis.SpatialModeling;



namespace Landis.Extension.Browse
{
    public class CalibrateLog
    {

        public static void WriteCalibrateFile(int year)
        {
            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {

                foreach (ISpecies species in PlugIn.ModelCore.Species)
                {
                    ISpeciesCohorts cohortList = SiteVars.Cohorts[site][species];
                    if (cohortList != null)
                    {
                        foreach (ICohort cohort in cohortList)
                        {

                            PlugIn.calibrateLog.Clear();
                            CalibrateLog clog = new CalibrateLog();

                            clog.Year = year;
                            clog.CohortAge = cohort.Data.Age;
                            clog.CohortName = cohort.Species.Name;
                            clog.ForageInReach = (int)cohort.Data.AdditionalParameters.ForageInReach;
                            clog.BrowseRemoval = (int)cohort.Data.AdditionalParameters.BiomassRemoval;
                            clog.Forage = (int)cohort.Data.AdditionalParameters.ProportionBrowse;
                            clog.ProportionBrowse = (int)cohort.Data.AdditionalParameters.ProportionBrowse;

                            PlugIn.calibrateLog.AddObject(clog);
                            PlugIn.calibrateLog.WriteToFile();


                        }
                    }
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
        public short GrowthReduction { get; set; } 

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int ForageInReach { get; set; } 

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int BrowseRemoval { get; set; } // index 4

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int Forage { get; set; } // index 5

        [DataFieldAttribute(Unit = "Proportion")]
        public int ProportionBrowse { get; set; } 


        //public static void SetCalibrateData(ICohort cohort, int index, double newValue)
        //{
        //    int cohortAddYear = SiteVars.GetAddYear(cohort);
        //    Dictionary<int, double[]> cohortDict;
        //    double[] oldValue;

        //    //PlugIn.ModelCore.UI.WriteLine("cohort species = {0}, species index = {1}, cohort add year = {2}, calibrate index = {3}", 
        //     //   cohort.Species.Name, cohort.Species.Index, cohortAddYear, index);

        //    // If the dictionary entry exists for the cohort, overwrite it:
        //    if (CohortCalibrationData.TryGetValue(cohort.Species.Index, out cohortDict))
        //        if (cohortDict.TryGetValue(cohortAddYear, out oldValue))
        //        {
        //         //   PlugIn.ModelCore.UI.WriteLine("Replacing values for cohort in calibrate log");
        //            CohortCalibrationData[cohort.Species.Index][cohortAddYear][index] = newValue;
        //            return;
        //        }

        //    // If the dictionary does not exist for the cohort, create it:
        //    Dictionary<int, double[]> newEntry = new Dictionary<int, double[]>();
        //    double[] newArray = new double[9]; //SF update this number when adding new calibration variable -- n+1
        //    newArray[index] = newValue;
        //    newEntry.Add(cohortAddYear, newArray);

        //    if (CohortCalibrationData.ContainsKey(cohort.Species.Index))
        //    {
        //      //  PlugIn.ModelCore.UI.WriteLine("Adding species to calibrate log");
        //        CohortCalibrationData[cohort.Species.Index].Add(cohortAddYear, newArray);
        //    }
        //    else
        //    {
        //      //  PlugIn.ModelCore.UI.WriteLine("Adding species to calibrate log");
        //        CohortCalibrationData.Add(cohort.Species.Index, newEntry);
        //    }
        //}


    }
}
