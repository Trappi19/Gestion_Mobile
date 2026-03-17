using UnityEngine;

public class PlusPerson : MonoBehaviour
{

    public GameObject PlusPersonPanel;
    public GameObject AjoutPersonPanel;
    public GameObject PersonPanel;


    private bool isOpen = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlusPersonPanel.SetActive(false);
    }


    public void OpenClosePlusPersonPanel()
    {
        isOpen = !isOpen;
        PlusPersonPanel.SetActive(isOpen);
    }

    // Ajout Personne

    public void OpenAjoutPerson()
    {
        PersonPanel.SetActive(false);
        PlusPersonPanel.SetActive(false);
        AjoutPersonPanel.SetActive(true);
    }

    public void CloseAjoutPerson()
    {
        AjoutPersonPanel.SetActive(false);
        PersonPanel.SetActive(true);
    }
}
