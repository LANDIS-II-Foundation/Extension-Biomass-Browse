//  Authors:  Brian Miranda, Nate De Jager, Patrick Drohan

namespace Landis.Extension.Browse
{
	/// <summary>
	/// Editable parameters (size and frequency) for wind events for a
	/// collection of ecoregions.
	/// </summary>
	public class EventParameterDataset
	{
		private IEventParameters[] parameters;

		//---------------------------------------------------------------------

		/// <summary>
		/// The number of ecoregions in the dataset.
		/// </summary>
		public int Count
		{
			get {
				return parameters.Length;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The event parameters for an ecoregion.
		/// </summary>
		public IEventParameters this[int ecoregionIndex]
		{
			get {
				return parameters[ecoregionIndex];
			}

			set {
				parameters[ecoregionIndex] = value;
			}
		}

		//---------------------------------------------------------------------

		public EventParameterDataset(int ecoregionCount)
		{
			parameters = new IEventParameters[ecoregionCount];
		}

	}
}
