//  Authors:  Brian Miranda, Nate De Jager, Patrick Drohan

using Landis.SpatialModeling;
using System.Collections.Generic;

namespace Landis.Extension.Browse
{
    /// <summary>
    /// The parameters for an zone.
    /// </summary>
    public interface IPopulationZone
    {
        int MapCode {get;set;}
        int Index {get; set;}
        double Population  {get;set;}
        double EffectivePop { get; set; }
        double K { get; set; }
        List<Location> PopulationZoneSites {get;}
        double WeightedBrowse { get; set; }
        double TotalForage { get; set; }
        double BDI { get; set; }
    }
}

namespace Landis.Extension.Browse
{
    public class PopulationZone
        : IPopulationZone
    {
        private int mapCode;
        private int index;
        
        private double population;
        private double effectivePop;
        private double k;
        private List<Location> populationZoneSites;
        private double weightedBrowse;
        private double totalForage;
        private double bdi;

        //---------------------------------------------------------------------
        public int Index
        {
            get {
                return index;
            }
            set {
                index = value;
            }
        }
        
        //---------------------------------------------------------------------
        public int MapCode
        {
            get {
                return mapCode;
            }
            set {
                mapCode = value;
            }
        }
        //---------------------------------------------------------------------
        public double Population
        {
            get
            {
                return population;
            }
            set
            {
                population = value;
            }
        }
        //---------------------------------------------------------------------
        public double EffectivePop
        {
            get
            {
                return effectivePop;
            }
            set
            {
                effectivePop = value;
            }
        }
        //---------------------------------------------------------------------
        public double K
        {
            get
            {
                return k;
            }
            set
            {
                k = value;
            }
        }
        //---------------------------------------------------------------------
        public List<Location> PopulationZoneSites
        {
            get
            {
                return populationZoneSites;
            }
        }
        //---------------------------------------------------------------------
        public double WeightedBrowse
        {
            get
            {
                return weightedBrowse;
            }
            set
            {
                weightedBrowse = value;
            }
        }
        //---------------------------------------------------------------------
        public double TotalForage
        {
            get
            {
                return totalForage;
            }
            set
            {
                totalForage = value;
            }
        }
        //---------------------------------------------------------------------
        public double BDI
        {
            get
            {
                return bdi;
            }
            set
            {
                bdi = value;
            }
        }
        //---------------------------------------------------------------------
        public PopulationZone(int index, int mapCode)
        {
            populationZoneSites =   new List<Location>();
            this.index = index;
            this.mapCode = mapCode;
        }
        //---------------------------------------------------------------------
        
    }
}
