using System;
using GDataDB;

namespace Sample {
	internal class Program {
		private static void Main(string[] args) {
			Console.WriteLine("Connecting");
			var client = new DatabaseClient("you@gmail.com", "yourpassword");
			const string dbName = "testing";
			Console.WriteLine("Opening or creating database");
			var db = client.GetDatabase(dbName) ?? client.CreateDatabase(dbName);
			const string tableName = "testtable";
			Console.WriteLine("Opening or creating table");
			var t = db.GetTable<Entity>(tableName) ?? db.CreateTable<Entity>(tableName);
			var all = t.FindAll();
			Console.WriteLine("{0} elements", all.Count);
			var r = all.Count > 0 ? all[0] : t.Add(new Entity {Conteudo = "some content", Amount = 5});
			Console.WriteLine("conteudo: {0}", r.Element.Conteudo);
			r.Element.Conteudo = "nothing at all " + DateTime.Now;
			Console.WriteLine("updating row");
			r.Update();
			Console.WriteLine("Now there are {0} elements", t.FindAll().Count);
			Console.WriteLine("executing a few queries");
			foreach (var q in new[] { "amount=5", "amount<6", "amount>0", "amount!=6", "amount<>6" }) {
				Console.Write("querying '{0}': ", q);
				var rows = t.FindStructured(q);
				Console.WriteLine("{0} elements found", rows.Count);
			}
			Console.WriteLine("deleting row");
			r.Delete();
			//t.DeleteAll();
			//Console.WriteLine("deleting table"); // FIXME doesn't work!
			//t.Delete();
			//Console.WriteLine("deleting database"); // FIXME doesn't work!
			//db.Delete();
			Console.WriteLine("Press any key...");
			Console.ReadKey();
		}
	}
}