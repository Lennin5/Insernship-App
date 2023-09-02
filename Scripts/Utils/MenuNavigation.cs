using System.Collections;
using System.Collections.Generic;
using UI.Tables;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuNavigation : MonoBehaviour
{
    public GameObject tableDailyExpenses;
    public GameObject panelWeeklyExpenses;
    public GameObject panelMonthlyExpenses;

    // Start is called before the first frame update
    void Start()
    {
        panelWeeklyExpenses.SetActive(false);
        panelMonthlyExpenses.SetActive(false);
        //tableDailyExpenses.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonExpenses(string option)
    {
        switch (option)
        {
            case "daily":
                tableDailyExpenses.SetActive(true);
                panelWeeklyExpenses.SetActive(false);
                panelMonthlyExpenses.SetActive(false);
                break;
            case "weekly":
                tableDailyExpenses.SetActive(false);
                panelWeeklyExpenses.SetActive(true);
                panelMonthlyExpenses.SetActive(false);
                break;
            case "monthly":
                tableDailyExpenses.SetActive(false);
                panelWeeklyExpenses.SetActive(false);
                panelMonthlyExpenses.SetActive(true);
                break;
        }
    }
}
