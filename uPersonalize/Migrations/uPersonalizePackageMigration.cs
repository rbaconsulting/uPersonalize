using System.Reflection;
using Umbraco.Cms.Core.Packaging;
using uPersonalize.Constants;
using uPersonalize.Migrations.Plans;

namespace uPersonalize.Migrations
{
	public class uPersonalizePackageMigration : PackageMigrationPlan
	{
		public uPersonalizePackageMigration() : base(Plugin.Name)
		{
		}

		protected override void DefinePlan()
		{
			var newVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

			To<uPersonalizePackage>(newVersion);
		}
	}
}