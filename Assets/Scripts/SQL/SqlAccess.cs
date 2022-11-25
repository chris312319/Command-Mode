using System;
using System.Data;
using MySql.Data.MySqlClient;
using UnityEngine;
using System.Text;

public class SqlAccess
{
	public static MySqlConnection dbConnection;
	public SqlAccess(string host, string port, string username, string pwd, string database)
	{
		//连接数据库
		try
		{
			string connectionString = string.Format("server = {0};port={1};database = {2};user = {3};password = {4};", host, port, database, username, pwd);
			dbConnection = new MySqlConnection(connectionString);
			dbConnection.Open();
			Debug.Log("連接成功！");
		}
		catch (Exception e)
		{
			throw new Exception("連接失敗！" + e.Message.ToString());
		}
	}

	//关闭连接
	public void Close()
	{
		if (dbConnection != null)
		{
			dbConnection.Close();
			dbConnection.Dispose();
			dbConnection = null;
		}
	}

	//查询
	public DataSet Select(string tableName, string[] items, string[] whereColumnName, string[] operation, string[] value)
	{

		if (whereColumnName.Length != operation.Length || operation.Length != value.Length)
		{
			throw new Exception("输入不正确：" + "要查询的条件、条件操作符、条件值 的数量不一致！");
		}
		string query = "Select " + items[0];
		for (int i = 1; i < items.Length; i++)
		{
			query += "," + items[i];
		}

		query += " FROM " + tableName + " WHERE " + whereColumnName[0] + " " + operation[0] + " '" + value[0] + "'";
		for (int i = 1; i < whereColumnName.Length; i++)
		{
			query += " and " + whereColumnName[i] + " " + operation[i] + " '" + value[i] + "'";
		}
		return Execute(query);

	}

	public void Insert(string tableName,PlayerData data)
	{
		string query = "INSERT INTO " + tableName + " VALUES ('" + data.UserName + "', '" + data.Password + "', '" + data.Email + "', '" + data.Phone + "', '" + data.Song1 + "', '" + data.Song2 + "', '" + data.Song3 + "')";
		Execute(query);
	}

	public void UpdateValue(string tableName,PlayerData data)
	{
		string query = "UPDATE " + tableName + " SET Password = " + data.Password + " WHERE UserName = '" + data.UserName + "'"; 
		Execute(query);
		query = "UPDATE " + tableName + " SET Song1 = " + data.Song1 + " WHERE UserName = '" + data.UserName + "'";
		Execute(query);
		query = "UPDATE " + tableName + " SET Song2 = " + data.Song2 + " WHERE UserName = '" + data.UserName + "'";
		Execute(query);
		query = "UPDATE " + tableName + " SET Song3 = " + data.Song3 + " WHERE UserName = '" + data.UserName + "'";
		Execute(query);
	}

	//执行sql语句
	public static DataSet Execute(string sqlString)
	{
		if (dbConnection.State == ConnectionState.Open)
		{
			DataSet ds = new DataSet();
			try
			{
				MySqlDataAdapter da = new MySqlDataAdapter(sqlString, dbConnection);
				da.Fill(ds);
			}
			catch (Exception ee)
			{
				throw new Exception("SQL:" + sqlString + "/n" + ee.Message.ToString());
			}
			finally
			{
			}
			return ds;
		}
		return null;
	}
}
public class PlayerData
{
	public string UserName;
	public string Password;
	public string Email;
	public string Phone;
	public int Song1;
	public int Song2;
	public int Song3;
}