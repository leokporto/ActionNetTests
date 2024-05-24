using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Reflection;

namespace TestFuckinOracle
{
	public class SequenceItem
	{
		[Key]
		public long Id { get; set; }

		public int ParentId { get; set; }

		public string TagType { get; set; }

		public string TagName { get; set; }

		public string Description { get; set; }

		public double CurrentValue { get; set; }

		public double InstructorValue { get; set; }
	}

	internal class Program
	{
		private static Dictionary<Type, OracleType> _sqlGlossaryTypes = new Dictionary<Type, OracleType>();
		private static Dictionary<Type, string> _sqlGlossary = new Dictionary<Type, string>();
		private const int STEP_VALUE = 100;
		private const int TOTAL_VALUES = 1347;
		static void Main(string[] args)
		{
			List<SequenceItem> list = new List<SequenceItem>();
			Random random = new Random();
			for (int i = 1; i < (TOTAL_VALUES + 1); i++)
			{
				SequenceItem item = new SequenceItem()
				{
					ParentId = 1,
					TagType = "Integer",
					TagName = $"REGIAO_01\\SE_{random.Next(1, 4)}\\BAY_{i}.STA.DJ",
					Description = $"BAY_{i};STA;POSIÇÃO DISJUNTOR",
					CurrentValue = 0d,
					InstructorValue = 0d
				};
				list.Add(item);
			}



			LoadSqlGlossary();

			System.Threading.Thread.Sleep(1000);

			try
			{
				using (OracleConnection conn = new OracleConnection("Data Source=ActionNet;User Id=dev;Password=sdop3985;"))
				{
					conn.Open();
					//InsertWithOracleClient(item);
					//InsertWithOracleCmdParams(item, conn);
					MiniProfiler miniProfiler = MiniProfiler.StartNew("Test oracle inserts");

					//DeleteAll(conn);

					//using (miniProfiler.Step("InsertMultipleWithTransaction"))
					//{
					//	Console.WriteLine("InsertMultipleWithTransaction");
					//	InsertMultipleWithTransaction(list, conn);
					//}

					DeleteAll(conn);

					using (miniProfiler.Step("InsertMultipleOneByOne"))
					{
						Console.WriteLine("InsertMultipleOneByOne");
						InsertMultipleOneByOne(list, conn);
					}

					DeleteAll(conn);

					using (miniProfiler.Step("BatchesInsertMultipleWithTransaction"))
					{
						Console.WriteLine("BatchesInsertMultipleWithTransaction");
						BatchesInsertWithAction(list,conn, InsertMultipleWithTransaction);
					}

					DeleteAll(conn);

					using (miniProfiler.Step("BatchesInsertMultipleWithOneTransaction"))
					{
						Console.WriteLine("BatchesInsertMultipleWithOneTransaction");
						BatchesInsertMultipleWithOneTransaction(list, conn);
					}

					//DeleteAll(conn);

					//using (miniProfiler.Step("BatchesInsertMultipleOneByOne"))
					//{
					//	Console.WriteLine("BatchesInsertMultipleOneByOne");
					//	BatchesInsertWithAction(list, conn, InsertMultipleOneByOne);
					//}

					DeleteAll(conn);

					//using(miniProfiler.Step("InsertMultipleTableValued"))
					//{
					//	Console.WriteLine("InsertMultipleTableValued");
					//	InsertMultipleTableValued(conn, list);
					//}
					//DeleteAll(conn);


					miniProfiler.Stop();
					Console.WriteLine(miniProfiler.RenderPlainText());

					Console.ReadKey();
					//InsertWithDapper();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static void InsertWithOracleCmdParams(SequenceItem item, OracleConnection conn)
		{

			try
			{

				using (OracleCommand cmd = conn.CreateCommand())
				{
					string sqlQuery = $"INSERT INTO \"ScenarioItems\" (\"ParentId\", \"TagType\", \"TagName\", \"Description\", \"CurrentValue\", \"InstructorValue\") VALUES (:ParentId, :TagType, :TagName, :Description, :CurrentValue, :InstructorValue)";
					cmd.CommandText = sqlQuery;
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add(new OracleParameter("ParentId", item.ParentId));
					cmd.Parameters.Add(new OracleParameter("TagType", item.TagType));
					cmd.Parameters.Add(new OracleParameter("TagName", item.TagName));
					cmd.Parameters.Add(new OracleParameter("Description", item.Description));
					cmd.Parameters.Add(new OracleParameter("CurrentValue", item.CurrentValue));
					cmd.Parameters.Add(new OracleParameter("InstructorValue", item.InstructorValue));

					var result = cmd.ExecuteNonQuery();
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		

		//TODO: Verificar como fazer um insert com inumeras linhas
		private static void InsertMultipleOneByOne(List<SequenceItem> items, OracleConnection conn)
		{
			if (items == null || items.Count == 0)
			{
				return;
			}

			try
			{
				int totalRecords = 0;
				using (OracleCommand cmd = conn.CreateCommand())
				{
					string sqlQuery = $"INSERT INTO \"ScenarioItems\" (\"ParentId\", \"TagType\", \"TagName\", \"Description\", \"CurrentValue\", \"InstructorValue\") VALUES (:ParentId, :TagType, :TagName, :Description, :CurrentValue, :InstructorValue)";
					cmd.CommandText = sqlQuery;
					cmd.CommandType = CommandType.Text;

					var properties = GetProperties(items[0]);

					foreach (var prop in properties)
					{
						if (_sqlGlossaryTypes.ContainsKey(prop.PropertyType))
							cmd.Parameters.Add(new OracleParameter(prop.Name, _sqlGlossaryTypes[prop.PropertyType]));
					}

					foreach (var item in items)
					{
						var valueProperties = GetProperties(item);
						foreach (var prop in valueProperties)
							cmd.Parameters[prop.Name].Value = prop.GetValue(item);

						var result = cmd.ExecuteNonQuery();
						totalRecords += result;
					}

				}
				Console.WriteLine($"Total records: {totalRecords}");

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static void BatchesInsertWithAction(List<SequenceItem> items, OracleConnection conn, Func<List<SequenceItem>,OracleConnection,int> insertAction) {
			int pages = GetPages(items);
			int totalRecords = 0;
			for (int i = 0; i < pages; i++)
			{
				List<SequenceItem> batch = items.Skip(i * STEP_VALUE).Take(STEP_VALUE).ToList();
				totalRecords += insertAction.Invoke(batch, conn);
			}
			Console.WriteLine($"Total records: {totalRecords}");
		}

		private static void BatchesInsertMultipleWithOneTransaction(List<SequenceItem> items, OracleConnection conn) 
		{
			if (items == null || items.Count == 0)
			{
				return;
			}

			try
			{
				int totalRecords = 0;
				OracleTransaction transaction = conn.BeginTransaction();
				int pages = GetPages(items);
				for (int i = 0; i < pages; i++)
				{
					List<SequenceItem> batch = items.Skip(i * STEP_VALUE).Take(STEP_VALUE).ToList();

					foreach (var item in batch)
					{
						using (OracleCommand cmd = conn.CreateCommand())
						{
							string sqlQuery = $"INSERT INTO \"ScenarioItems\" (\"ParentId\", \"TagType\", \"TagName\", \"Description\", \"CurrentValue\", \"InstructorValue\") VALUES (:ParentId, :TagType, :TagName, :Description, :CurrentValue, :InstructorValue)";
							cmd.CommandText = sqlQuery;
							cmd.CommandType = CommandType.Text;
							cmd.Transaction = transaction;

							var properties = GetProperties(items[0]);

							foreach (var prop in properties)
							{
								if (_sqlGlossaryTypes.ContainsKey(prop.PropertyType))
									cmd.Parameters.Add(new OracleParameter(prop.Name, _sqlGlossaryTypes[prop.PropertyType]));
							}

							var valueProperties = GetProperties(item);
							foreach (var prop in valueProperties)
								cmd.Parameters[prop.Name].Value = prop.GetValue(item);

							var result = cmd.ExecuteNonQuery();
							totalRecords += result;
						}
					}
				}
				transaction.Commit();
				Console.WriteLine($"Total records: {totalRecords}");

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static int GetPages(List<SequenceItem> items)
		{
			int pages = items.Count / STEP_VALUE;
			int rest = items.Count % STEP_VALUE;
			if (rest > 0)
				pages++;
			Console.WriteLine($"pages: {pages}");
			return pages;
		}

		private static int InsertMultipleWithTransaction(List<SequenceItem> items, OracleConnection conn)
		{
			if (items == null || items.Count == 0)
			{
				return 0;
			}

			try
			{
				int totalRecords = 0;
				OracleTransaction transaction = conn.BeginTransaction();
				foreach (var item in items)
				{
					using (OracleCommand cmd = conn.CreateCommand())
					{
						string sqlQuery = $"INSERT INTO \"ScenarioItems\" (\"ParentId\", \"TagType\", \"TagName\", \"Description\", \"CurrentValue\", \"InstructorValue\") VALUES (:ParentId, :TagType, :TagName, :Description, :CurrentValue, :InstructorValue)";
						cmd.CommandText = sqlQuery;
						cmd.CommandType = CommandType.Text;
						cmd.Transaction = transaction;

						var properties = GetProperties(items[0]);

						foreach (var prop in properties)
						{
							if (_sqlGlossaryTypes.ContainsKey(prop.PropertyType))
								cmd.Parameters.Add(new OracleParameter(prop.Name, _sqlGlossaryTypes[prop.PropertyType]));
						}
					
						var valueProperties = GetProperties(item);
						foreach (var prop in valueProperties)
							cmd.Parameters[prop.Name].Value = prop.GetValue(item);

						var result = cmd.ExecuteNonQuery();
						totalRecords += result;
					}
				}
				transaction.Commit();
				return totalRecords;

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return 0;
			}
		}

		//private static void InsertMultipleTableValued(OracleConnection conn, List<SequenceItem> items)
		//{
		//	if(conn == null)
		//	{
		//		return;
		//	}

		//	if (items == null || items.Count == 0)
		//	{
		//		return;
		//	}

		//	DataTable table = new DataTable();
		//	var properties = GetProperties(items[0]);
		//	string fields = "";

		//	foreach (var prop in properties)
		//	{
		//		table.Columns.Add(prop.Name, prop.PropertyType);
		//		fields += $"\"{prop.Name}\",";
		//	}
		//	fields = fields.Remove(fields.Length - 1);

		//	string query = $"INSERT INTO \"ScenarioItems\" ({fields}) VALUES SELECT {fields} FROM :SeqTable";

		//	using(OracleCommand cmd = conn.CreateCommand())
		//	{
		//		cmd.CommandText = query;
		//		cmd.CommandType = CommandType.Text;
		//		cmd.Parameters.Add(new SqlParameter() 
		//							{ 
		//								ParameterName = ":SeqTable",
		//								SqlDbType = SqlDbType.Structured,
		//								Value = table
		//							});
				
		//		cmd.ExecuteNonQuery();
		//	}
		//}

		//private static void BulkInsert(OracleConnection conn, DataTable table)
		//{
		//	using (OracleBulkCopy bulkCopy = new OracleBulkCopy(conn))
		//	{
		//		bulkCopy.DestinationTableName = "MyTable";
		//		bulkCopy.ColumnMappings.Add("mytext", "mytext");
		//		bulkCopy.ColumnMappings.Add("num", "num");
		//		bulkCopy.WriteToServer(table);
		//	}
		//}

		private static IEnumerable<PropertyInfo> GetProperties(SequenceItem defaultItem)
		{			
			IEnumerable<PropertyInfo> properties = defaultItem.GetType().GetProperties()
					.Where(element => !(Attribute.IsDefined(element, typeof(NotMappedAttribute)) ||
										Attribute.IsDefined(element, typeof(KeyAttribute)) ||
										Attribute.IsDefined(element, typeof(DatabaseGeneratedAttribute)))).ToList();

			return properties;
		}

		

		//private static void InsertWithOracleClient(SequenceItem item)
		//{
			

		//	try
		//	{
		//		using (OracleConnection conn = new OracleConnection("Data Source=ActionNet;User Id=dev;Password=sdop3985;"))
		//		{
		//			conn.Open();

		//			using (OracleCommand cmd = conn.CreateCommand())
		//			{
		//				string sqlQuery = $"INSERT INTO \"ScenarioItems\" (\"ParentId\", \"TagType\", \"TagName\", \"Description\", \"CurrentValue\", \"InstructorValue\") VALUES ({item.ParentId}, '{item.TagType}', '{item.TagName}', '{item.Description}', {item.CurrentValue}, {item.InstructorValue})";
		//				cmd.CommandText = sqlQuery;
		//				cmd.CommandType = CommandType.Text;
						
		//				var result = cmd.ExecuteNonQuery();
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine(ex.Message);
		//	}
		//}

		public static void DeleteAll(OracleConnection conn)
		{
			try
			{
				OracleTransaction oracleTransaction = conn.BeginTransaction();
				using (OracleCommand cmd = conn.CreateCommand())
				{
					string sqlQuery = "DELETE FROM \"ScenarioItems\"";
					cmd.CommandText = sqlQuery;
					cmd.CommandType = CommandType.Text;
					cmd.Transaction = oracleTransaction;

					var result = cmd.ExecuteNonQuery();
				}
				
				oracleTransaction.Commit();
				//System.Threading.Thread.Sleep(1000);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}


		#region OracleQueryBuilder
		private static void LoadSqlGlossary()
		{
			_sqlGlossary.Add(typeof(long), "INTEGER");
			_sqlGlossary.Add(typeof(long?), "INTEGER");
			_sqlGlossary.Add(typeof(int), "INTEGER");
			_sqlGlossary.Add(typeof(string), "VARCHAR");
			_sqlGlossary.Add(typeof(byte), "NUMBER(3)");
			_sqlGlossary.Add(typeof(bool), "NUMBER(3)");
			_sqlGlossary.Add(typeof(short), "NUMBER(5)");
			_sqlGlossary.Add(typeof(DateTime), "DATE");
			_sqlGlossary.Add(typeof(DateTime?), "DATE");
			_sqlGlossary.Add(typeof(double), "REAL");
			_sqlGlossary.Add(typeof(float), "FLOAT");

			_sqlGlossaryTypes.Add(typeof(long), OracleType.Number);
			_sqlGlossaryTypes.Add(typeof(long?), OracleType.Number);
			_sqlGlossaryTypes.Add(typeof(int), OracleType.Number);		
			_sqlGlossaryTypes.Add(typeof(string), OracleType.NVarChar);
			_sqlGlossaryTypes.Add(typeof(byte), OracleType.Byte);
			_sqlGlossaryTypes.Add(typeof(bool), OracleType.Byte);
			_sqlGlossaryTypes.Add(typeof(short), OracleType.Number);
			_sqlGlossaryTypes.Add(typeof(DateTime), OracleType.DateTime);
			_sqlGlossaryTypes.Add(typeof(DateTime?), OracleType.DateTime);
			_sqlGlossaryTypes.Add(typeof(double), OracleType.Double);
			_sqlGlossaryTypes.Add(typeof(float), OracleType.Float);
		}
		#endregion
	}
}
