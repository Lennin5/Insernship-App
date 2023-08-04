using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//References
using Mono.Data.Sqlite;
using TMPro;
using System.Data;
using System.Net;
using System.Xml.Linq;
using System;

public class InsertData : MonoBehaviour
{
    private string conn, sqlQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;

    public TMP_InputField txtBoxOutward, txtBoxLaunch, txtBoxReturn;

    // Start is called before the first frame update
    void Start()
    {
        // Empty
        txtBoxOutward.text = "0.75";
        txtBoxLaunch.text = "3.00";
        txtBoxReturn.text = "0.75";
    }

    public void InsertExpenseButton()
    {
        // Get current date
        DateTime currentDate = DateTime.Now;

        // Set values to insert
        int day = currentDate.Day;
        string monthLetter = currentDate.ToString("MMMM");
        int year = currentDate.Year;
        string outwardPrice = txtBoxOutward.text;
        string launchPrice = txtBoxLaunch.text;
        string returnPrice = txtBoxReturn.text;
        string totalExpenses = (float.Parse(outwardPrice) + float.Parse(launchPrice) + float.Parse(returnPrice)).ToString("F2");

        // Do monthLetter with only the first letter in uppercase
        monthLetter = monthLetter.Substring(0, 1).ToUpper() + monthLetter.Substring(1);


        // Call insert function and pass values
        InsertExpense(day, monthLetter, year, outwardPrice, launchPrice, returnPrice, totalExpenses);
    }

    private void InsertExpense(int day, string monthLetter, int year, string outwardPrice, string launchPrice, string returnPrice, string totalExpenses)
    {
        // Get database path       
        string filePath = InitializeDB.Instance.CurrentDatabasePath;

        // Open db connection
        conn = "URI=file:" + filePath;
        dbconn = new SqliteConnection(conn);
        dbconn.Open();

        try
        {
            // Insert data
            dbcmd = dbconn.CreateCommand();
            sqlQuery = string.Format("INSERT INTO Expenses (day, month_letter, year, outward_price, launch_price, return_price, total_expenses) VALUES ({0}, '{1}', {2}, '{3}', '{4}', '{5}', '{6}')", day, monthLetter, year, outwardPrice, launchPrice, returnPrice, totalExpenses);
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            // Close db connection
            dbcmd.Dispose();
            dbconn.Close();
            Debug.Log("Insert Done");

            // Call read expenses function and pass filepath
            ReadData.Instance.LoadReadExpenses(filePath);
        }
        catch (Exception e)
        {
            Debug.Log("Error when inserting data: " + e.Message);
        }
    }
}
