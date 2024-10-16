﻿using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace TTLAdapter
{
	public class TTLAdapterInfo : GH_AssemblyInfo
	{
		public override string Name => "TTLAdapter";

		//Return a 24x24 pixel bitmap to represent this GHA library.
		public override Bitmap Icon => null;

		//Return a short string describing the purpose of this GHA library.
		public override string Description => "Send RDF data from grasshopper to the web application.";

		public override Guid Id => new Guid("d2410be1-9461-4097-9428-0f29c478bb41");

		//Return a string identifying you or your company.
		public override string AuthorName => "University of Stuttgart";

		//Return a string representing your preferred contact details.
		public override string AuthorContact => "";

		// public override string Version => "0.1.0";
	}
}