using System;
using System.Data;
using MySql.Data.MySqlClient;
using UnityEngine;
using System.Text;
using TMPro;

public class SQLManager : MonoBehaviour
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

    public GameObject LoginS, RegisterS, ForgetS, ResetS;
    void Start()
    {
        LoginS = GameObject.Find("Login");
        RegisterS = GameObject.Find("Register");
        ForgetS = GameObject.Find("Forget");
        ResetS = GameObject.Find("Reset");
        RegisterS.SetActive(false);
        ForgetS.SetActive(false);
        ResetS.SetActive(false);
        sql = new SqlAccess(host, port, username, pwd, database);
    }
    void Login()
    {
        if (GameObject.Find("L_account").GetComponent<TMP_InputField>().text != "" && GameObject.Find("L_password").GetComponent<TMP_InputField>().text != "")
        {
            DataSet ds = sql.Select("test", new string[] { "UserName", "Password" , "Email" , "Phone" , "Song1" , "Song2" , "Song3" }, new string[] { "`" + "UserName" + "`", "`" + "Password" + "`" }, new string[] { "=", "=" }, new string[] { GameObject.Find("L_account").GetComponent<TMP_InputField>().text, GameObject.Find("L_password").GetComponent<TMP_InputField>().text });
            if (ds != null)
            {
                DataTable table = ds.Tables[0];
                if (table.Rows.Count > 0)
                {
                    Debug.Log("登入成功!");
                    LoginState = true;
                    RefreshData(table);
                    GameObject.Find("MessageBox").GetComponent<TMP_Text>().text = "登入成功";
                }
                else
                {
                    Debug.Log("帳號或密碼錯誤");
                    GameObject.Find("MessageBox").GetComponent<TMP_Text>().text = "帳號或密碼錯誤";
                }
                GameObject.Find("L_account").GetComponent<TMP_InputField>().text = "";
                GameObject.Find("L_password").GetComponent<TMP_InputField>().text = "";
            }
        }
        else
        {
            Debug.Log("不可有欄位為空!");
            GameObject.Find("MessageBox").GetComponent<TMP_Text>().text = "不可有欄位為空";
        }
    }
    void Register()
    {
        if (GameObject.Find("R_account").GetComponent<TMP_InputField>().text != "" && GameObject.Find("R_password").GetComponent<TMP_InputField>().text != "" && GameObject.Find("R_email").GetComponent<TMP_InputField>().text != "" && GameObject.Find("R_phone").GetComponent<TMP_InputField>().text != "")
        {
            DataSet ds = sql.Select("test", new string[] { "Username" }, new string[] { "`" + "UserName" + "`" }, new string[] { "=" }, new string[] { GameObject.Find("R_account").GetComponent<TMP_InputField>().text });
            if (ds != null)
            {
                DataTable table = ds.Tables[0];
                if (table.Rows.Count > 0)
                {
                    Debug.Log("此用戶已存在!");
                    GameObject.Find("MessageBox").GetComponent<TMP_Text>().text = "此用戶已存在";
                    GameObject.Find("R_account").GetComponent<TMP_InputField>().text = "";
                    GameObject.Find("R_password").GetComponent<TMP_InputField>().text = "";
                    GameObject.Find("R_email").GetComponent<TMP_InputField>().text = "";
                    GameObject.Find("R_phone").GetComponent<TMP_InputField>().text = "";
                }
                else
                {
                    GameObject.Find("MessageBox").GetComponent<TMP_Text>().text = "";
                    PlayerData data = new PlayerData();
                    data.UserName = GameObject.Find("R_account").GetComponent<TMP_InputField>().text;
                    data.Password = GameObject.Find("R_password").GetComponent<TMP_InputField>().text;
                    data.Email = GameObject.Find("R_email").GetComponent<TMP_InputField>().text;
                    data.Phone = GameObject.Find("R_phone").GetComponent<TMP_InputField>().text;
                    data.Song1 = 0;
                    data.Song2 = 0;
                    data.Song3 = 0;
                    sql.Insert("test", data);
                    Debug.Log("註冊成功!");
                    GameObject.Find("R_account").GetComponent<TMP_InputField>().text = "";
                    GameObject.Find("R_password").GetComponent<TMP_InputField>().text = "";
                    GameObject.Find("R_email").GetComponent<TMP_InputField>().text = "";
                    GameObject.Find("R_phone").GetComponent<TMP_InputField>().text = "";
                    RegisterS.SetActive(false);
                }
            }
        }
        else
        {
            Debug.Log("不可有欄位為空!");
            GameObject.Find("MessageBox").GetComponent<TMP_Text>().text = "不可有欄位為空";
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
        Debug.Log("玩家帳號：" + Player.UserName + " 玩家密碼:" + Player.Password);
    }
    void UpdateData(PlayerData data, int value, string pswd, int num)
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
    public void Login_click()
    {
        Login();
    }
    public void Register_click()
    {
        GameObject.Find("MessageBox").GetComponent<TMP_Text>().text = "";
        RegisterS.SetActive(true);
    }
    public void Forget_click()
    {
        GameObject.Find("MessageBox").GetComponent<TMP_Text>().text = "";
        ForgetS.SetActive(true);
    }
    public void Create_click()
    {
        Register();
    }
    public void Back_click()
    {
        GameObject.Find("MessageBox").GetComponent<TMP_Text>().text = "";
        GameObject.Find("L_account").GetComponent<TMP_InputField>().text = "";
        GameObject.Find("L_password").GetComponent<TMP_InputField>().text = "";
        RegisterS.SetActive(false);
        ForgetS.SetActive(false);
    }
    public void Check_click()
    {
        if (GameObject.Find("F_email").GetComponent<TMP_InputField>().text != "" && GameObject.Find("F_phone").GetComponent<TMP_InputField>().text != "")
        {
            DataSet ds = sql.Select("test", new string[] { "UserName", "Password", "Email", "Phone", "Song1", "Song2", "Song3" }, new string[] { "`" + "Email" + "`", "`" + "Phone" + "`" }, new string[] { "=", "=" }, new string[] { GameObject.Find("F_email").GetComponent<TMP_InputField>().text, GameObject.Find("F_phone").GetComponent<TMP_InputField>().text });
            if (ds != null)
            {
                DataTable table = ds.Tables[0];
                if (table.Rows.Count > 0)
                {
                    Debug.Log("驗證成功!");
                    GameObject.Find("MessageBox").GetComponent<TMP_Text>().text = "";
                    RefreshData(table);
                    ResetS.SetActive(true);
                }
                else
                {
                    Debug.Log("驗證失敗!");
                    GameObject.Find("MessageBox").GetComponent<TMP_Text>().text = "驗證失敗";
                }
                GameObject.Find("F_email").GetComponent<TMP_InputField>().text = "";
                GameObject.Find("F_phone").GetComponent<TMP_InputField>().text = "";
            }
        }
        else
        {
            Debug.Log("不可有欄位為空!");
            GameObject.Find("MessageBox").GetComponent<TMP_Text>().text = "不可有欄位為空";
        }
    }
    public void Change_click()
    {
        if(GameObject.Find("Re_password").GetComponent<TMP_InputField>().text != "")
        {
            GameObject.Find("MessageBox").GetComponent<TMP_Text>().text = "";
            UpdateData(Player, 0, GameObject.Find("Re_password").GetComponent<TMP_InputField>().text, 1);
            GameObject.Find("Re_password").GetComponent<TMP_InputField>().text = "";
            ResetS.SetActive(false);
            ForgetS.SetActive(false);
        }
        else
        {
            Debug.Log("不可有欄位為空!");
            GameObject.Find("MessageBox").GetComponent<TMP_Text>().text = "不可有欄位為空";
        }
    }
}