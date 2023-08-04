using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

// References
using Mono.Data.Sqlite;
using TMPro;

public class DeleteData : MonoBehaviour
{
    private string conn, sqlQuery;
    IDbConnection dbconn;
    IDbCommand dbcmd;

    public TMP_InputField txtBoxId;

    // Start is called before the first frame update
    void Start()
    {
        // Empty
    }

    public void DeleteExpenseButton()
    {
        // Set value to delete
        int id = int.Parse(txtBoxId.text);

        // Call delete function and pass the ID
        DeleteExpense(id);
    }

    private void DeleteExpense(int id)
    {
        // Get database path
        string filePath = InitializeDB.Instance.CurrentDatabasePath;

        // Open db connection
        conn = "URI=file:" + filePath;
        dbconn = new SqliteConnection(conn);
        dbconn.Open();

        try
        {
            // Delete data
            dbcmd = dbconn.CreateCommand();
            sqlQuery = string.Format("DELETE FROM Expenses WHERE id = \"{0}\"", id);
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            // Call read expenses function and pass filepath
            ReadData.Instance.LoadReadExpenses(filePath);

            // Close db connection
            dbcmd.Dispose();
            dbconn.Close();
            Debug.Log("Data deleted successfully!");
        }
        catch (Exception e)
        {
            Debug.Log("Error when deleting data: " + e.Message);
        }
    }
}
