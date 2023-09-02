using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using System;
using TMPro;

public class MonthlyData : MonoBehaviour
{
    private string conn;
    private IDbConnection dbconn;

    public TMP_Text dataMonthlyResult;

    private void Start()
    {
        PrintExpensesByMonth();
    }

    public void PrintExpensesByMonth()
    {
        string filePath = InitializeDB.Instance.CurrentDatabasePath;
        conn = "URI=file:" + filePath;
        dbconn = new SqliteConnection(conn);
        dbconn.Open();

        Dictionary<string, decimal> monthlyExpenses = new Dictionary<string, decimal>();
        string lastMonth = null;

        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT id, day, month_letter, year, outward_price, launch_price, return_price, total_expenses FROM Expenses";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                decimal totalExpenses = reader.GetDecimal(7);

                // Obtener el mes y año de la fecha actual
                string monthYear = $"{reader.GetString(2)}-{reader.GetInt32(3)}";

                // Agregar los gastos al mes correspondiente en el diccionario
                if (monthlyExpenses.ContainsKey(monthYear))
                {
                    monthlyExpenses[monthYear] += totalExpenses;
                }
                else
                {
                    monthlyExpenses[monthYear] = totalExpenses;
                }

                lastMonth = monthYear; // Actualizar el último mes en cada iteración
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
        }

        // Mostrar los resultados en el componente TMP_Text
        foreach (var kvp in monthlyExpenses)
        {
            string[] monthYearParts = kvp.Key.Split('-');
            string monthName = monthYearParts[0];
            int year = int.Parse(monthYearParts[1]);
            string monthInfo = $"{monthName} {year}: ${kvp.Value}\n";

            // Verificar si es el último mes en el diccionario y agregar el texto "(EN PROCESO)"
            if (kvp.Key == lastMonth)
            {
                monthInfo = "(EN PROCESO) " + monthInfo;
            }

            dataMonthlyResult.text += monthInfo;
        }
    }
}
