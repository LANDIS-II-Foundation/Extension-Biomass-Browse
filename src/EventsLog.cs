using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Extension.Browse
{
    public class EventsLog
    {

        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "...")]
        public int Time { get; set; }

        [DataFieldAttribute(Desc = "Population Zone")]
        public int PopulationZone { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Total Sites")]
        public int TotalSites { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Total Population of Browsers")]
        public int TotalPopulation { get; set; }

        [DataFieldAttribute(Desc = "Population Density (animals per ha)")]
        public double PopulationDensity { get; set; }

        [DataFieldAttribute(Desc = "Total Forage (kg)")]
        public long TotalForage { get; set; }

        [DataFieldAttribute(Desc = "Mean Forage (g m^-2)")]
        public int MeanForage { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Total Capacity (K)")]
        public int TotalK { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Total Effective Population")]
        public int TotalEffectivePopulation { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Total Sites Damaged")]
        public int TotalSitesDamaged { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Average Biomass Removed")]
        public int AverageBiomassRemoved { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Average Biomass Killed")]
        public int AverageBiomassKilled { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Total Cohorts Killed")]
        public int TotalCohortsKilled { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Browse Density Index")]
        public double BDI { get; set; }

    }
}
