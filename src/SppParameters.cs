//  Authors:  Brian Miranda, Nate De Jager, Patrick Drohan

using Landis.Utilities;

namespace Landis.Extension.Browse
{
    /// <summary>
    /// Extra Spp Paramaters
    /// </summary>
    public interface ISppParameters
    {
        double BrowsePref{ get; set; }
        double GrowthReductThresh { get; set; }
        double GrowthReductMax { get; set; }
        double MortThresh { get; set; }
        double MortMax { get; set; }   
        double BiomassMax { get; set; }
    }
}


namespace Landis.Extension.Browse
{
    public class SppParameters
        : ISppParameters
    {
        private double browsePref;
        private double growthReductThresh;
        private double growthReductMax;
        private double mortThresh;
        private double mortMax;
        private double biomassMax;


        //---------------------------------------------------------------------
        public double BrowsePref
        {
            get
            {
                return browsePref;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or > 0.");
                if (value > 1)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or < 1.");
                browsePref = value;
            }
        }
        //---------------------------------------------------------------------
        public double GrowthReductThresh
        {
            get
            {
                return growthReductThresh;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or > 0.");
                if (value > 1)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or < 1.");
                growthReductThresh = value;
            }
        }
        //---------------------------------------------------------------------
        public double GrowthReductMax
        {
            get
            {
                return growthReductMax;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or > 0.");
                if (value > 1)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or < 1.");
                growthReductMax = value;
            }
        }
        //---------------------------------------------------------------------
        public double MortThresh
        {
            get
            {
                return mortThresh;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or > 0.");
                if (value > 1)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or < 1.");
                mortThresh = value;
            }
        }
        //---------------------------------------------------------------------
        public double MortMax
        {
            get
            {
                return mortMax;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or > 0.");
                if (value > 1)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or < 1.");
                mortMax = value;
            }
        }
        //---------------------------------------------------------------------
        public double BiomassMax
        {
            get
            {
                return biomassMax;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or > 0.");
                if (value > 100000)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or < 100000.");
                biomassMax = value;
            }
        }
        //---------------------------------------------------------------------
        public SppParameters()
        {
            this.browsePref = 0.0;
            this.mortThresh = 0.0;
            this.mortMax = 0.0;
            this.growthReductThresh = 0.0;
            this.growthReductMax = 0.0;
            this.biomassMax = 0.0;
        }
 
    }
}
