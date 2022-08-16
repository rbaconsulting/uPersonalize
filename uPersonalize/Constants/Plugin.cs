namespace uPersonalize.Constants
{
	public struct Plugin
	{
		public const string Name = "uPersonalize";

		public struct Migrations
		{
			public struct Settings
			{
				public const string MigrationKey = "uPersonalize_Settings";
				public const string TableName = "uPersonalizeSettings";
			}

			public struct Reporting
			{
				public const string MigrationKey = "uPersonalize_Reporting";
				public const string TableName = "uPersonalizeSettings";
			}
		}
	}
}