
using Npgsql;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;



namespace TestSqlKata
{
	internal class Program
	{
		static void Main(string[] args)
		{
			//new SQLiteConnection("Data Source=C:\\Action.NET\\Projects\\Project3.dbAlarm;")
			//new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=AnOTS;User Id=postgres;Password=Sdop@3985;")
			//new OracleConnection("Data Source=ActionNet;Integrated Security=no;User Id=dev;Password=sdop3985;")
			using (DbConnection connection = new SqlConnection("Server=127.0.0.1;Database=ActionNet;User Id=sa;Password=sdop@3985;")) { 
				connection.Open();

				var compiler = new SqlServerCompiler();
				var query = new Query("Alarms")
					.WhereBetween("ActiveTime_Ticks", DateTime.Now.AddDays(-60).ToUniversalTime().Ticks, DateTime.Now.ToUniversalTime().Ticks)
					.Where("Priority", "<", 10)
					.OrderByDesc("ActiveTime_Ticks");

				SqlResult result = compiler.Compile(query);
				string sql = result.Sql;
				List<object> bindings = result.Bindings;

				Console.WriteLine($"Query: {sql}");
				Console.WriteLine($"Bindings: {string.Join(", ", bindings)}");

				Console.WriteLine("Buscando dados...");

				var db = new QueryFactory(connection, compiler);
			
				var alarms = db.Get<AlarmItem>(query).ToList();

				Console.WriteLine($"Retornados {alarms.Count} registros");

				Console.WriteLine("Pressione alguma tecla para encerrar");
				Console.ReadLine();
			}
		}
	}
}
