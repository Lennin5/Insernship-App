using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using System;
using System.Linq;
using TMPro;

public class WeeklyData : MonoBehaviour
{
    private string conn;
    private IDbConnection dbconn;

    public TMP_Text dataWeeklyResult;

    private void Start()
    {
        PrintExpensesByWeek();
    }

    public void PrintExpensesByWeek()
    {
        string filePath = InitializeDB.Instance.CurrentDatabasePath;
        conn = "URI=file:" + filePath;
        dbconn = new SqliteConnection(conn);
        dbconn.Open();

        int totalRecords = 0;
        decimal totalExpenses = 0;
        int weekCounter = 1;
        List<DateTime> daysInWeek = new List<DateTime>();

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
                daysInWeek.Add(currentDate);

                // Si ya hemos leído 5 registros, calculamos las fechas de inicio y fin de la semana y luego imprimimos el total
                if (totalRecords % 5 == 0)
                {
                    DateTime startDate = daysInWeek.Min(); // La fecha más pequeña es el inicio de la semana
                    DateTime endDate = daysInWeek.Max();   // La fecha más grande es el fin de la semana
                    string startDateFormatted = startDate.ToString("dd") + " de " + startDate.ToString("MMMM");
                    string endDateFormatted = endDate.ToString("dd") + " de " + endDate.ToString("MMMM");
                    //Debug.Log($"Total de gastos en Semana {weekCounter} ({startDateFormatted} - {endDateFormatted}): ${totalExpenses}");
                    dataWeeklyResult.text += $"Semana {weekCounter} ({startDateFormatted} - {endDateFormatted}): ${totalExpenses}\n";
                    totalExpenses = 0;
                    weekCounter++;

                    // Reiniciamos las variables para la próxima semana
                    daysInWeek.Clear();
                }
            }

            // Si quedan registros pendientes que no forman un grupo de 5, los imprimimos como una nueva semana incompleta
            if (totalRecords % 5 != 0)
            {
                DateTime startDate = daysInWeek.Min();
                DateTime endDate = daysInWeek.Max();
                string startDateFormatted = startDate.ToString("dd") + " de " + startDate.ToString("MMMM");
                string endDateFormatted = endDate.ToString("dd") + " de " + endDate.ToString("MMMM");
                Debug.Log($"(EN PROCESO) Semana {weekCounter} ({startDateFormatted} - {endDateFormatted}): ${totalExpenses}");
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
        }
    }
}
