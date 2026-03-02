using UnityEngine;
using TMPro;

public class PersonDetailUI : MonoBehaviour
{
    public GameObject panelRoot;          // Parent du panel
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI aimeText;
    public TextMeshProUGUI aimePasText;

    PersonData currentPerson;
    DBManager db;

    public void Show(PersonData person, DBManager dbManager)
    {
        currentPerson = person;
        db = dbManager;

        titleText.text = person.nom;

        string aimeStr, aimePasStr;
        db.GetGouts(person.id, out aimeStr, out aimePasStr);

        aimeText.text = db.BuildNamesFromIds(aimeStr);
        aimePasText.text = db.BuildNamesFromIds(aimePasStr);

        panelRoot.SetActive(true);
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
    }

    // Exemples de fonctions à appeler par des boutons "Ajouter"
    public void OnAddPlatAime(int idPlat)
    {
        if (currentPerson == null) return;
        db.AddPlatToGouts(currentPerson.id, idPlat, true);
        Refresh();
    }

    public void OnAddPlatAimePas(int idPlat)
    {
        if (currentPerson == null) return;
        db.AddPlatToGouts(currentPerson.id, idPlat, false);
        Refresh();
    }

    void Refresh()
    {
        string aimeStr, aimePasStr;
        db.GetGouts(currentPerson.id, out aimeStr, out aimePasStr);
        aimeText.text = db.BuildNamesFromIds(aimeStr);
        aimePasText.text = db.BuildNamesFromIds(aimePasStr);
    }
}
