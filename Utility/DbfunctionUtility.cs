using Microsoft.Extensions.Options;
using LaCafelogy.Models;
using System;
using System.Data;
using Npgsql;

namespace LaCafelogy.Utility
{
    public class DbfunctionUtility
    {
        private NpgsqlConnection connection;
        private string connectionString = "";
        private readonly IOptions<Appsettings> _appSettings;

        public DbfunctionUtility(IOptions<Appsettings> appSettings)
        {
            connection = new NpgsqlConnection();
            _appSettings = appSettings;
            connection.ConnectionString = appSettings.Value.DefaultConnection;
        }

        public DataSet GetDataset(string query)
        {
            DataSet ds = new DataSet();
            try
            {
                NpgsqlDataAdapter da = new NpgsqlDataAdapter();
                NpgsqlCommand cmd = new NpgsqlCommand();
                da.SelectCommand = cmd;
                cmd.Connection = connection;
                cmd.CommandText = query;
                da.Fill(ds);
            }
            catch (Exception ex)
            {
            }

            return ds;
        }
    }
}