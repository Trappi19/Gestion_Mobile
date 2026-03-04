using UnityEngine;

public class MenueManager : MonoBehaviour
{
    [SerializeField]
    public GameObject StartPanel;
    public GameObject DetailPersonPanel;
    public GameObject PersonPanel;
    //public GameObject HistoricPanel;
    //public GameObject IngredientsPanel;
    //public GameObject PlatsPanel;


    void Start()
    {
        StartPanel.SetActive(true);
        DetailPersonPanel.SetActive(false);
        PersonPanel.SetActive(false);
        //HistoricPanel.SetActive(false);
        //IngredientsPanel.SetActive(false);
        //PlatsPanel.SetActive(false);
    }



    // Personnal panel
    public void OpenPersonPanel()
    {
        PersonPanel.SetActive(true);
    }

    public void ClosePersonPanel()
    {
        PersonPanel.SetActive(false);
    }
    public void CloseDetailPersonPanel()
    {
        PersonPanel.SetActive(true);
        DetailPersonPanel.SetActive(false);
    }
}
