using System;
using System.Xml;
using Umbraco.Cms.Core.Packaging;

namespace uPersonalize.Migrations
{
	public class uPersonalizeMigrationPlan : PackageMigrationPlan
    {
        public uPersonalizeMigrationPlan() : base("uPersonalize")
        {
        }

        protected override void DefinePlan()
        {
            To<uPersonalizeMigration>(CreateRandomState());
        }
    }
}
