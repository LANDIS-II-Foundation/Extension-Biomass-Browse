//  Authors:  Brian Miranda, Nate De Jager, Patrick Drohan

using System.Collections.Generic;
using System.Linq;
using Landis.Core;

namespace Landis.Extension.Browse
{
    class PreferenceList
    {
        public static List<double> Initialize(ISppParameters[] sppParms)
        {
            //Browse - compile sorted list of species preferences
            List<double> preferenceList = new List<double>();
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                //PlugIn.ModelCore.UI.WriteLine("species = {0}", species.Name); //debug
                //SF TODO try-catch here and throw exception if species is missing; name the species
                double browsePref = sppParms[species.Index].BrowsePref;
                if (!preferenceList.Contains(browsePref))
                    preferenceList.Add(browsePref);
            }
            List<double> sortedPreferenceList = (preferenceList.OrderByDescending(i => i)).ToList();

            return sortedPreferenceList;
        }
    }
}
