using MySqlConnector;
using System.Data;
using System.Runtime.CompilerServices;

namespace TestUtilities
{
    public class DbSetupHelper
    {
        internal string Server { get; private set; }

        internal string Uid { get; private set; }

        internal string Password { get; private set; }

        public DbSetupHelper(string server, string uid, string pwd)
        {
            if (string.IsNullOrEmpty(server))
            {
                throw new ArgumentNullException("server");
            }

            if (string.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid");
            }

            if (string.IsNullOrEmpty(pwd))
            {
                throw new ArgumentNullException("pwd");
            }

            Server = server;
            Uid = uid;
            Password = pwd;
        }

        public void SetupDb(string db, string tables)
        {
            if (string.IsNullOrEmpty(db))
            {
                throw new ArgumentNullException("db");
            }

            if (string.IsNullOrEmpty(tables))
            {
                throw new ArgumentNullException("tables");
            }

            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 3);
            defaultInterpolatedStringHandler.AppendLiteral("Server=");
            defaultInterpolatedStringHandler.AppendFormatted(Server);
            defaultInterpolatedStringHandler.AppendLiteral(";Uid=");
            defaultInterpolatedStringHandler.AppendFormatted(Uid);
            defaultInterpolatedStringHandler.AppendLiteral(";Pwd=");
            defaultInterpolatedStringHandler.AppendFormatted(Password);
            defaultInterpolatedStringHandler.AppendLiteral(";SslMode=None;");
            using (MySqlConnection mySqlConnection = new MySqlConnection(defaultInterpolatedStringHandler.ToStringAndClear()))
            {
                mySqlConnection.Open();
                using MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlCommand.CommandText = "drop database if exists `" + db + "`";
                mySqlCommand.CommandType = CommandType.Text;
                mySqlCommand.ExecuteNonQuery();
                mySqlCommand.CommandText = "create database `" + db + "`";
                mySqlCommand.CommandType = CommandType.Text;
                mySqlCommand.ExecuteNonQuery();
            }

            defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 4);
            defaultInterpolatedStringHandler.AppendLiteral("Server=");
            defaultInterpolatedStringHandler.AppendFormatted(Server);
            defaultInterpolatedStringHandler.AppendLiteral(";Uid=");
            defaultInterpolatedStringHandler.AppendFormatted(Uid);
            defaultInterpolatedStringHandler.AppendLiteral(";Pwd=");
            defaultInterpolatedStringHandler.AppendFormatted(Password);
            defaultInterpolatedStringHandler.AppendLiteral(";Database=");
            defaultInterpolatedStringHandler.AppendFormatted(db);
            defaultInterpolatedStringHandler.AppendLiteral(";SslMode=None;");
            using MySqlConnection mySqlConnection2 = new MySqlConnection(defaultInterpolatedStringHandler.ToStringAndClear());
            mySqlConnection2.Open();
            using MySqlCommand mySqlCommand2 = mySqlConnection2.CreateCommand();
            mySqlCommand2.CommandText = tables;
            mySqlCommand2.CommandType = CommandType.Text;
            mySqlCommand2.ExecuteNonQuery();
        }

        public void TearDownDb(string db)
        {
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 3);
            defaultInterpolatedStringHandler.AppendLiteral("Server=");
            defaultInterpolatedStringHandler.AppendFormatted(Server);
            defaultInterpolatedStringHandler.AppendLiteral(";Uid=");
            defaultInterpolatedStringHandler.AppendFormatted(Uid);
            defaultInterpolatedStringHandler.AppendLiteral(";Pwd=");
            defaultInterpolatedStringHandler.AppendFormatted(Password);
            defaultInterpolatedStringHandler.AppendLiteral(";SslMode=None;");
            using MySqlConnection mySqlConnection = new MySqlConnection(defaultInterpolatedStringHandler.ToStringAndClear());
            mySqlConnection.Open();
            using MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
            mySqlCommand.CommandText = "drop database if exists `" + db + "`";
            mySqlCommand.CommandType = CommandType.Text;
            mySqlCommand.ExecuteNonQuery();
        }
    }
}
