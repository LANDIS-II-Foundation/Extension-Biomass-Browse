﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Extension.Browse
{
    public class EventsSpeciesLog
    {

        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "...")]
        public int Time { get; set; }

        [DataFieldAttribute(Desc = "Population Zone")]
        public int PopulationZone { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Total Sites")]
        public int TotalSites { get; set; }

        [DataFieldAttribute(Desc = "Species Name")]
        public string SpeciesName { get; set; }

        [DataFieldAttribute(Desc = "Species Index")]
        public int SpeciesIndex { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Average Forage Density")]
        public double AverageForage { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Average ForageInReach Density")]
        public double AverageForageInReach { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Total Biomass Consumed")]
        public double AverageBiomassBrowsed { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Total Biomass Removed (Consumed + Killed)")]
        public double AverageBiomassRemoved { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Total Cohorts Killed")]
        public int TotalCohortsKilled { get; set; }

    }
}
