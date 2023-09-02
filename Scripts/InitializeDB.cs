using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//References
using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Net;
using UnityEngine.SceneManagement;
using System.Threading;

public class InitializeDB : MonoBehaviour
{
    public static InitializeDB Instance { get; private set; }

    private string conn;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    private IDataReader reader;

    private string limaColor = "#76ff03";

    public string DeviceType;
    public string DatabaseName;
    public string CurrentDatabasePath;

    private void Awake()
    {
        // Assign 'this' when Instance method or variable is needed in another script
        if (Instance == null)
        {
            Instance = this;
        }

        // Add device type to database path (SWITCH BEFORE BUILDING GAME)
        DeviceType = "Windows";

        // Add the database name
        DatabaseName = "InternshipDB.s3db";

        // Filepath to database
        string filePathWindows = Application.dataPath + "/Plugins/" + DatabaseName;
        string filePathAndroid = Application.persistentDataPath + "/" + DatabaseName;

        // Validate device type to set database path
        switch (DeviceType)
        {
            case "Windows":
                CurrentDatabasePath = filePathWindows;
                break;
            case "Android":
                CurrentDatabasePath = filePathAndroid;
                break;
            default:
                Debug.LogError("Device type not found");
                break;
        }

        // Initialize database with device type. Now only works for: Windows/Android
        InitializeSqlite(DeviceType, CurrentDatabasePath);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Emprty...
    }

    private void InitializeSqlite(string deviceType, string filePath)
    {
        // If not exist database, create it with name DatabaseName
        if (!File.Exists(filePath))
        {
            // If not found will create database
            Debug.LogWarning("<color=yellow>File \"" + DatabaseName + "\" doesn't exist. " +
                "Creating new from \"" + filePath + "</color>");

            string url = Path.Combine(Application.streamingAssetsPath, DatabaseName);
            UnityWebRequest loadDB = UnityWebRequest.Get(url);
            loadDB.SendWebRequest();
            Debug.Log("<color=cyan>Database created successfully!</color>");
        }

        CreateTables(filePath, deviceType);
    }

    private void CreateTables(string filePath, string deviceType)
    {
        // Open db connection
        conn = "URI=file:" + filePath;
        Debug.Log("<color=#00FF00>Stablishing " + deviceType + " connection to: " + conn + "</color>");
        dbconn = new SqliteConnection(conn);
        dbconn.Open();

        // Create table if not exist
        string expensesQuery = "CREATE TABLE IF NOT EXISTS Expenses (id INTEGER PRIMARY KEY AUTOINCREMENT, day INT, month_letter VARCHAR(10), year INT, outward_price VARCHAR(5), launch_price VARCHAR(5), return_price VARCHAR(5), total_expenses VARCHAR(5))";

        try
        {
            // Variable to control if insert default data or not when creating tables first time
            bool insertData = true;

            // Table name extracted from command query
            string tableName = expensesQuery.Split(' ')[5];

            // Validate if table already exists in the database
            dbcmd = dbconn.CreateCommand();
            dbcmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Expenses'";
            reader = dbcmd.ExecuteReader();

            if (reader.Read())
            {
                // If table already exists, do nothing
                Debug.Log("<color=yellow>[INFO] Table Expenses already exists</color>");
                insertData = false;
            }
            else
            {
                // If table doesn't exist, create it
                reader.Close();
                dbcmd.CommandText = expensesQuery;
                reader = dbcmd.ExecuteReader();
                Debug.Log("<color=cyan>Table Expenses created successfully!</color>");

                // If tableName is the last item in commands list, proceed to insert default data
                if (tableName == "Expenses")
                {
                    if (insertData)
                    {
                        // Call method to insert default data
                        InsertDefaultData();
                        insertData = false;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error when creating table: " + e.Message);
        }

        // Close db connection
        dbconn.Close();
        Debug.Log("<color=" + limaColor + ">Closed connection to database!</color>");
    }

    private void InsertDefaultData()
    {
        try
        {
            string insertExpenseQuery =
                "INSERT INTO Expenses (day, month_letter, year, outward_price, launch_price, return_price, total_expenses) " +
                "VALUES " +
                "(10, 'Julio', 2023, '0.00', '3.00', '1.00', '4.00'), " +
                "(11, 'Julio', 2023, '0.75', '3.00', '0.75', '4.50'), " +
                "(12, 'Julio', 2023, '0.75', '3.00', '0.75', '4.50'), " +
                "(13, 'Julio', 2023, '0.75', '2.50', '0.75', '4.00'), " +
                "(14, 'Julio', 2023, '0.75', '3.00', '0.75', '4.50'), " +
                "(17, 'Julio', 2023, '0.75', '2.50', '0.75', '4.00'), " +
                "(18, 'Julio', 2023, '0.75', '3.00', '0.75', '4.50'), " +
                "(19, 'Julio', 2023, '0.75', '2.50', '0.75', '4.00'), " +
                "(20, 'Julio', 2023, '0.75', '2.50', '0.75', '4.00'), " +
                "(21, 'Julio', 2023, '0.75', '2.75', '0.75', '4.25'), " +
                "(24, 'Julio', 2023, '0.75', '0.00', '0.75', '1.50'), " +
                "(25, 'Julio', 2023, '0.75', '0.00', '0.75', '1.50'), " +
                "(26, 'Julio', 2023, '0.75', '0.25', '0.75', '1.75'), " +
                "(27, 'Julio', 2023, '0.75', '1.25', '0.75', '2.75'), " +
                "(28, 'Julio', 2023, '0.75', '5.50', '0.75', '7.00'), " +
                "(7, 'Agosto', 2023, '0.75', '0.00', '0.75', '1.50'), " +
                "(8, 'Agosto', 2023, '0.75', '0.00', '0.75', '1.50'), " +
                "(9, 'Agosto', 2023, '0.75', '3.00', '0.75', '4.50'), " +
                "(10, 'Agosto', 2023, '0.75', '4.00', '0.75', '5.50'), " +
                "(11, 'Agosto', 2023, '0.75', '1.75', '0.75', '3.25'), " +
                "(21, 'Agosto', 2023, '0.75', '0.25', '0.75', '1.75'), " +
                "(22, 'Agosto', 2023, '0.75', '0.00', '0.75', '1.50'), " +
                "(23, 'Agosto', 2023, '0.75', '0.25', '0.75', '1.75'), " +
                "(24, 'Agosto', 2023, '0.75', '0.25', '0.75', '1.75'), " +
                "(25, 'Agosto', 2023, '1.90', '3.25', '0.75', '5.90'), " +
                "(28, 'Agosto', 2023, '0.75', '0.00', '0.75', '1.50'), " +
                "(29, 'Agosto', 2023, '0.75', '0.50', '1.25', '2.50'), " +
                "(30, 'Agosto', 2023, '0.75', '0.25', '0.75', '1.75'), " +
                "(31, 'Agosto', 2023, '0.75', '5.25', '0.75', '6.75'), " +
                "(1, 'Septiembre', 2023, '0.75', '5.00', '0.75', '6.50')";



            // Create a list of commands to execute
            List<string> commandsToInsert = new List<string>();
            commandsToInsert.Add(insertExpenseQuery);

            foreach (string command in commandsToInsert)
            {
                // Table name extracted from command query
                string tableName = command.Split(' ')[2];
                // Execute command
                dbcmd = dbconn.CreateCommand();
                dbcmd.CommandText = command;
                reader = dbcmd.ExecuteReader();
                Debug.Log("<color=magenta>Default data inserted on table " + tableName + " successfully!</color>");
            }
        }
        catch (Exception)
        {
            Debug.LogError("Error when inserting default data");
        }
    }

}
