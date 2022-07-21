using Umbraco.Cms.Core.Packaging;
using uPersonalize.Constants;
using uPersonalize.Migrations.Plans;

namespace uPersonalize.Migrations
{
	public class uPersonalizePackageMigration : PackageMigrationPlan
	{
		public uPersonalizePackageMigration() : base(AppPlugin.Name)
		{
		}

		protected override void DefinePlan()
		{
			To<uPersonalizePackage>(base.CreateRandomState());
		}
	}
}