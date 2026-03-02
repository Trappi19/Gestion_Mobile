using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using System;
using TMPro;
using UnityEngine.UI;

public class DBManager : MonoBehaviour
{
    [Header("UI Liste")]
    public Transform personnesContent;       // Content du ScrollView
    public GameObject personneButtonPrefab;  // Prefab avec PersonButtonUI

    [Header("UI Debug (optionnel)")]
    public TextMeshProUGUI outputText;       // Comme avant, pour log texte

    [Header("Panel Détail")]
    public PersonDetailUI detailPanel;       // Script du panel détail

    string dbPath;

    void Awake()
    {
        Debug.Log("DBManager START MOBILE");

        dbPath = Path.Combine(Application.persistentDataPath, "bdd.db");
        string srcPath = Path.Combine(Application.streamingAssetsPath, "bdd.db");

#if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(CopyDBFromStreamingAssetsAndroid(srcPath, dbPath));
#else
        File.Copy(srcPath, dbPath, true);    // Mode dev : recopie toujours
        Debug.Log("DB recopiée (dev) : " + dbPath);
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    IEnumerator CopyDBFromStreamingAssetsAndroid(string srcPath, string dstPath)
    {
        if (srcPath.Contains("jar:") || srcPath.Contains("://"))
        {
            using (var uwr = UnityEngine.Networking.UnityWebRequest.Get(srcPath))
            {
                yield return uwr.SendWebRequest();
                File.WriteAllBytes(dstPath, uwr.downloadHandler.data);
            }
            Debug.Log("DB copiée (Android) : " + dstPath);
        }
        else
        {
            File.Copy(srcPath, dstPath, true);
        }

        // Une fois la copie faite, on peut charger la liste
        StartCoroutine(LoadAfterCopy());
    }

    IEnumerator LoadAfterCopy()
    {
        yield return null;
        StartApp();
    }
#endif

    void Start()
    {
#if !UNITY_ANDROID || UNITY_EDITOR
        StartApp();
#endif
    }

    void StartApp()
    {
        Debug.Log("DB path = " + dbPath + " | exists = " + File.Exists(dbPath));
        AfficherTables();
        AfficherPersonnesListe();    // <- nouvelle fonction pour créer les boutons
    }

    void AfficherTables()
    {
        string connStr = "Data Source=" + dbPath + ";Version=3;";
        using (var conn = new SqliteConnection(connStr))
        {
            conn.Open();
            string sql = "SELECT name FROM sqlite_master WHERE type='table'";
            using (var cmd = new SqliteCommand(sql, conn))
            using (IDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    Debug.Log("TABLE: " + rdr.GetString(0));
                }
            }
        }
    }

    // -------- NOUVEAU : création des boutons personnes --------

    void AfficherPersonnesListe()
    {
        string connStr = "Data Source=" + dbPath + ";Version=3;";
        List<PersonData> personnes = new List<PersonData>();

        try
        {
            Debug.Log("DBMOBILE: AfficherPersonnesListe START");

            using (var conn = new SqliteConnection(connStr))
            {
                conn.Open();
                Debug.Log("DBMOBILE: connexion OK");

                string sql = "SELECT id, nom, dernier_passage FROM personnes";
                using (var cmd = new SqliteCommand(sql, conn))
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        PersonData p = new PersonData();
                        p.id = rdr.GetInt32(0);
                        p.nom = rdr.GetString(1);

                        string dateStr = rdr.GetString(2); // format YYYY-MM-DD dans la BDD
                        DateTime dernierPassage = DateTime.Parse(dateStr);
                        p.dernierPassage = dernierPassage;

                        TimeSpan ts = DateTime.Now.Date - dernierPassage.Date;   // nb jours
                        p.joursDepuis = (int)ts.TotalDays;                       // [web:41]

                        personnes.Add(p);
                    }
                }
            }

            if (outputText != null)
            {
                if (personnes.Count == 0)
                {
                    outputText.text = "AUCUNE PERSONNE";
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var p in personnes)
                        sb.AppendLine(p.id + " - " + p.nom + " (" + p.joursDepuis + " j)");
                    outputText.text = sb.ToString();
                }
            }

            // Clear anciens boutons
            foreach (Transform child in personnesContent)
                Destroy(child.gameObject);

            // Instancier les boutons
            foreach (var p in personnes)
            {
                GameObject go = Instantiate(personneButtonPrefab, personnesContent);
                PersonButtonUI ui = go.GetComponent<PersonButtonUI>();
                ui.Init(p, this, detailPanel);
            }

            Debug.Log("DBMOBILE: personnes affichées = " + personnes.Count);
        }
        catch (System.Exception e)
        {
            string msg = "ERR: " + e.GetType().Name + " - " + e.Message;
            if (outputText != null) outputText.text = msg;
            Debug.LogError("DBMOBILE EX: " + msg);
        }
    }

    // -------- Fonctions utilisées par le panel détail --------

    public string GetConnectionString()
    {
        return "Data Source=" + dbPath + ";Version=3;";
    }

    // Récupère aime / aime_pas pour 1 personne
    public void GetGouts(int idPersonne, out string aime, out string aimePas)
    {
        aime = "";
        aimePas = "";

        string connStr = GetConnectionString();
        using (var conn = new SqliteConnection(connStr))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT aime, aime_pas FROM gouts WHERE id_personne = @id";
                cmd.Parameters.AddWithValue("@id", idPersonne);

                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        aime = rdr.IsDBNull(0) ? "" : rdr.GetString(0);
                        aimePas = rdr.IsDBNull(1) ? "" : rdr.GetString(1);
                    }
                }
            }
        }
    }

    // Convertit une liste d'IDs (string "1,2,3") en noms de plats/ingrédients
    public string BuildNamesFromIds(string idsStr)
    {
        if (string.IsNullOrEmpty(idsStr))
            return "-";

        string[] parts = idsStr.Split(',');
        List<string> names = new List<string>();

        string connStr = GetConnectionString();
        using (var conn = new SqliteConnection(connStr))
        {
            conn.Open();

            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part)) continue;
                int id = int.Parse(part.Trim());

                using (var cmd = conn.CreateCommand())
                {
                    // 1) Essaye dans plats
                    cmd.CommandText = "SELECT nom_plat FROM plats WHERE id = @id";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@id", id);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        names.Add(result.ToString());
                    }
                    else
                    {
                        // 2) Sinon essaye dans ingredients
                        cmd.CommandText = "SELECT nom_ingredient FROM ingredients WHERE id = @id";
                        result = cmd.ExecuteScalar();
                        if (result != null)
                            names.Add(result.ToString());
                    }
                }
            }
        }

        return string.Join(", ", names);
    }

    // Ajoute un plat/ingrédient à aime OU aime_pas (concat sur la liste d'IDs)
    public void AddPlatToGouts(int idPersonne, int idPlat, bool aimeCol)
    {
        string col = aimeCol ? "aime" : "aime_pas";
        string connStr = GetConnectionString();

        using (var conn = new SqliteConnection(connStr))
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = $@"
                UPDATE gouts
                SET {col} =
                    CASE
                        WHEN {col} IS NULL OR {col} = '' THEN @val
                        ELSE {col} || ',' || @val
                    END
                WHERE id_personne = @idPers;
                ";
                cmd.Parameters.AddWithValue("@val", idPlat.ToString());
                cmd.Parameters.AddWithValue("@idPers", idPersonne);
                cmd.ExecuteNonQuery();
            }
        }
    }
}

// DTO pour stocker 1 personne
[System.Serializable]
public class PersonData
{
    public int id;
    public string nom;
    public DateTime dernierPassage;
    public int joursDepuis;
}
