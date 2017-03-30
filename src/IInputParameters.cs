//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:    Robert M. Scheller, James B. Domingo

using System.Collections.Generic;

namespace Landis.Extension.DeerBrowse
{
	/// <summary>
	/// Parameters for the plug-in.
	/// </summary>
	public interface IInputParameters
	{

		int Timestep {get;set;}
        ISppParameters[] SppParameters { get; set; }
		string MapNamesTemplate	{get;set;}
		string LogFileName {get;set;}
	}
}
