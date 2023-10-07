using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

// References
using Mono.Data.Sqlite;
using TMPro;
using UnityEngine.UI;

public class ReadData : MonoBehaviour
{
    public static ReadData Instance { get; private set; }
    private string conn;
    IDbConnection dbconn;

    public GameObject tableContainer; // Referencia al objeto contenedor de la tabla
    public GameObject rowPrefab; // Referencia al prefab de la celda

    private void Awake()
    {
        // Assign 'this' when Instance method or variable is needed in another script
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        string filePath = InitializeDB.Instance.CurrentDatabasePath;

        // Call read function and pass filepath
        LoadReadExpenses(filePath);
    }

    public void LoadReadExpenses(string filePath)
    {
        ReadExpenses(filePath);
    }

    private void ReadExpenses(string filepath)
    {
        // Open db connection
        conn = "URI=file:" + filepath;
        dbconn = new SqliteConnection(conn);
        dbconn.Open();

        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open(); // Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT id, day, month_letter, year, outward_price, launch_price, return_price, total_expenses FROM Expenses ORDER BY id DESC";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            int rowIndex = 1;
            while (reader.Read())
            {
                GameObject cell;
                if (rowIndex < tableContainer.transform.childCount)
                {
                    // Obtener la fila existente en el índice rowIndex
                    cell = tableContainer.transform.GetChild(rowIndex).gameObject;
                }
                else
                {
                    // Instanciar una nueva fila si no hay ninguna disponible
                    cell = Instantiate(rowPrefab, tableContainer.transform);
                }

                // Configurar los valores en la celda
                cell.transform.GetChild(0).GetComponentInChildren<TMP_Text>().text = reader.GetInt32(0).ToString();
                cell.transform.GetChild(1).GetComponentInChildren<TMP_Text>().text = $"{reader.GetInt32(1)}-{reader.GetString(2)}-{reader.GetInt32(3)}";
                cell.transform.GetChild(2).GetComponentInChildren<TMP_Text>().text = $"${reader.GetDecimal(4)}";
                cell.transform.GetChild(3).GetComponentInChildren<TMP_Text>().text = $"${reader.GetDecimal(5)}";
                cell.transform.GetChild(4).GetComponentInChildren<TMP_Text>().text = $"${reader.GetDecimal(6)}";
                cell.transform.GetChild(5).GetComponentInChildren<TMP_Text>().text = $"${reader.GetDecimal(7)}";

                cell.SetActive(true); // Asegurarse de que la fila esté activa

                rowIndex++;
            }

            // Desactivar filas restantes si hay más filas de las necesarias
            for (int i = rowIndex; i < tableContainer.transform.childCount; i++)
            {
                tableContainer.transform.GetChild(i).gameObject.SetActive(false);
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
        }
    }

}
