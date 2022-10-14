using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Extension.Browse
{
    public class CalibrateLog
    {

        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "...")]
        public int Time { get; set; }

        [DataFieldAttribute(Desc = "Age of Cohort", Unit = "Years")]
        public int CohortAge { get; set; }

        [DataFieldAttribute(Desc = "Species Code")]
        public int CohortName { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Growth Reduction B")]
        public int GrowthReduction { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int NewForageInReach { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int FirstPassRemoval { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int SecondPassRemoval { get; set; }


        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int FinalRemoval { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int NewForage { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int LastBrowseProportion { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2)]
        public int ForageInReach { get; set; }


    }
}
