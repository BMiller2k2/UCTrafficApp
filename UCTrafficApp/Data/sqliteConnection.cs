using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCTrafficApp.Data;
	public class sqliteConnection
	{
		public ISQLiteAsyncConnection CreateConnection()
		{
		return new SQLiteAsyncConnection(
			Path.Combine(FileSystem.AppDataDirectory, "UCTrafficApp.db3"),
			SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
		}
	}
