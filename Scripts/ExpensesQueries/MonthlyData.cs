using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using System;
using System.Linq;
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

        int totalRecords = 0;
        decimal totalExpenses = 0;
        int monthCounter = 1;
        List<DateTime> daysInMonth = new List<DateTime>();

        using (dbconn = new SqliteConnection(conn))
        {
            dbconn.Open();
            IDbCommand dbcmd = dbconn.CreateCommand();
            string sqlQuery = "SELECT id, day, month_letter, year, outward_price, launch_price, return_price, total_expenses FROM Expenses";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                totalRecords++;
                totalExpenses += reader.GetDecimal(7);

                // Guardar la fecha del día actual
                string currentDateStr = $"{reader.GetInt32(1)}-{reader.GetString(2)}-{reader.GetInt32(3)}";
                DateTime currentDate = DateTime.Parse(currentDateStr);
                daysInMonth.Add(currentDate);

                // Si el mes cambia, calculamos las fechas de inicio y fin del mes anterior y luego imprimimos el total
                if (totalRecords > 0 && currentDate.Month != daysInMonth.First().Month)
                {
                    DateTime startDate = daysInMonth.Min();
                    DateTime endDate = daysInMonth.Max();
                    string startDateFormatted = startDate.ToString("MMMM yyyy");
                    //Debug.Log($"Total de gastos en {startDateFormatted}: ${totalExpenses}");
                    dataMonthlyResult.text += $"{startDateFormatted}: ${totalExpenses}\n";
                    totalExpenses = 0;
                    monthCounter++;

                    // Reiniciamos las variables para el próximo mes
                    daysInMonth.Clear();
                }
            }

            // Si quedan registros pendientes que no forman un mes completo, imprimimos el total con el mes actual
            if (daysInMonth.Count > 0)
            {
                DateTime startDate = daysInMonth.Min();
                DateTime endDate = daysInMonth.Max();
                string startDateFormatted = startDate.ToString("MMMM yyyy");
                //Debug.Log($"Total de gastos en {startDateFormatted}: ${totalExpenses}");
                dataMonthlyResult.text += $"{startDateFormatted}: ${totalExpenses}\n";
                totalExpenses = 0;
                monthCounter++;
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
        }
    }
}
