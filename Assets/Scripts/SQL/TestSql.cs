using System;
using System.Data;
using MySql.Data.MySqlClient;
using UnityEngine;
using System.Text;

public class TestSql : MonoBehaviour
{
    private string host = "127.0.0.1";
    private string port = "3306";
    private string username = "root";
    private string pwd = "32570472";
    private string database = "unity_user";
    public string UserName = "";
    public string Password = "";
    public string Email = "";
    public string Phone = "";
    public int Value = 0;
    public bool isLogin = false;
    public bool isRegister = false;
    public bool isUpdate = false;
    private bool LoginState = false;
    public PlayerData Player = new PlayerData();
    SqlAccess sql;
    void Start()
    {
        sql = new SqlAccess(host, port, username, pwd, database);
        if( isLogin) Login();
        if (isRegister) Register();
        if (isUpdate) UpdateData(Player,Value,"",1);
        sql.Close();
    }
    void Login()
    {
        DataSet ds = sql.Select("test", new string[] { "UserName","Password","Value" }, new string[] { "`" + "UserName" + "`", "`" + "Password" + "`" }, new string[] { "=", "=" }, new string[] { UserName, Password });
        if (ds != null)
        {
            DataTable table = ds.Tables[0];
            if (table.Rows.Count > 0)
            {
                Debug.Log("登入成功!");
                LoginState = true;
                RefreshData(table);
            }
            else
            {
                Debug.Log("帳號或密碼錯誤");
            }
        }
    }
    void Register()
    {
        DataSet ds = sql.Select("test", new string[] { "Value" }, new string[] { "`" + "UserName" + "`"}, new string[] {"="}, new string[] { UserName });
        if (ds != null)
        {
            DataTable table = ds.Tables[0];
            if (table.Rows.Count > 0)
            {
                Debug.Log("此用戶已存在!");
            }
            else
            {
                PlayerData data = new PlayerData();
                data.UserName = UserName;
                data.Password = Password;
                sql.Insert("test", data);
                Debug.Log("註冊成功!");
            }
        }
    }
    void RefreshData(DataTable table)
    {
        Player.UserName = table.Rows[0][0].ToString();
        Player.Password = table.Rows[0][1].ToString();
        Player.Email = table.Rows[0][2].ToString();
        Player.Phone = table.Rows[0][3].ToString();
        Player.Song1 = (int)table.Rows[0][4];
        Player.Song2 = (int)table.Rows[0][5];
        Player.Song3 = (int)table.Rows[0][6];
        Debug.Log("玩家帳號：" + Player.UserName + " 玩家密碼:" +  Player.Password);
    }
    void UpdateData(PlayerData data,int value,string pswd,int num)
    {
        switch (num)
        {
            case 1:
                data.Password = pswd;
                break;
            case 2:
                data.Song1 = value;
                break;
            case 3:
                data.Song2 = value;
                break;
            case 4:
                data.Song3 = value;
                break;
        }
            sql.UpdateValue("test", data);
    }
    
}