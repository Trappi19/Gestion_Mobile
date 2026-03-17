using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PersonButtonUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI daysText;

    PersonData data;
    DBManager db;
    PersonDetailUI detailPanel;

    public void Init(PersonData d, DBManager dbManager, PersonDetailUI panel)
    {
        data = d;
        db = dbManager;
        detailPanel = panel;

        nameText.text = d.nom;
        daysText.text = d.joursDepuis + " j";

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        detailPanel.Show(data, db);
    }
}
