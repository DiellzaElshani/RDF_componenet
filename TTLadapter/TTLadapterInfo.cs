using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace TTLadapter
{
	public class TTLadapterInfo : GH_AssemblyInfo
	{
		public override string Name => "TTLadapter";

		//Return a 24x24 pixel bitmap to represent this GHA library.
		public override Bitmap Icon => null;

		//Return a short string describing the purpose of this GHA library.
		public override string Description => "";

		public override Guid Id => new Guid("d2410be1-9461-4097-9428-0f29c478bb41");

		//Return a string identifying you or your company.
		public override string AuthorName => "";

		//Return a string representing your preferred contact details.
		public override string AuthorContact => "";
	}
}