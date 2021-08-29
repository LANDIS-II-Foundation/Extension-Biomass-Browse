//  Authors:  Robert M. Scheller, Brian Miranda


namespace Landis.Extension.Browse
{
    /// <summary>
    /// Values for each ecoregion x species combination.
    /// </summary>
    public interface IDynamicInputRecord
    {

        double Population{get;set;}
    }

    public class DynamicInputRecord
    : IDynamicInputRecord
    {

        private double population;

        public double Population
        {
            get {
                return population;
            }
            set {
                population = value;
            }
        }

        public DynamicInputRecord()
        {
        }

    }
}
