using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace Graph_Website
{
	public class GraphWebsiteInfo : GH_AssemblyInfo
	{
		public override string Name => "GraphWebsite";

		//Return a 24x24 pixel bitmap to represent this GHA library.
		public override Bitmap Icon => null;

		//Return a short string describing the purpose of this GHA library.
		public override string Description => "View RDF graph on localhost website";

		public override Guid Id => new Guid("5f7326de-303a-4c03-8d97-35cfc2126720");

		//Return a string identifying you or your company.
		public override string AuthorName => "University of Stuttgart";

		//Return a string representing your preferred contact details.
		public override string AuthorContact => "";
	}
}