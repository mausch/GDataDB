using System;
using NUnit.Framework;

namespace GDataDB.Tests {
	[TestFixture]
	public class IntegrationTests {
		private ITable<IntegrationEntity> table;

		[TestFixtureSetUp]
		public void FixtureSetup() {
			Console.WriteLine("Connecting");
			var client = new DatabaseClient("you@gmail.com", "password");
			const string dbName = "IntegrationTests";
			Console.WriteLine("Opening or creating database");
			var db = client.GetDatabase(dbName) ?? client.CreateDatabase(dbName);
			const string tableName = "IntegrationTests";
			Console.WriteLine("Opening or creating table");
			table = db.GetTable<IntegrationEntity>(tableName) ?? db.CreateTable<IntegrationEntity>(tableName);
			table.DeleteAll();
			table.Add(new IntegrationEntity());
		}

		[Test]
		public void tt() {
		}
	}
}