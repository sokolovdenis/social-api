using System;

namespace WebApi.DataSources
{
	public abstract class DataSource
	{
		public DataSource(Database databaseEngine)
		{
			Database = databaseEngine ?? throw new ArgumentNullException(nameof(databaseEngine));
		}

		protected Database Database { get; }
	}
}
