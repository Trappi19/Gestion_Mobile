using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using TMPro;

public class DBManager : MonoBehaviour
{
    public TextMeshProUGUI outputText;   // Assigne ton TMP ici dans l'Inspector

    string dbPath;

    //    void Awake()
    //    {
    //        // 1) Chemins
    //        dbPath = Path.Combine(Application.persistentDataPath, "bdd.db");
    //        string srcPath = Path.Combine(Application.streamingAssetsPath, "bdd.db");

    //        // 2) Copie depuis StreamingAssets si pas encore fait
    //        if (!File.Exists(dbPath))
    //        {
    //#if UNITY_ANDROID && !UNITY_EDITOR
    //            StartCoroutine(CopyDBFromStreamingAssetsAndroid(srcPath, dbPath));
    //#else
    //            File.Copy(srcPath, dbPath, true);
    //            Debug.Log("DB copiée vers : " + dbPath);
    //#endif
    //        }
    //        else
    //        {
    //            Debug.Log("DB déjà présente : " + dbPath);
    //        }
    //    }

    void Awake()
    {
        Debug.Log("DBManager START MOBILE");
        outputText.text = "HELLO MOBILE";

        dbPath = Path.Combine(Application.persistentDataPath, "bdd.db");
        string srcPath = Path.Combine(Application.streamingAssetsPath, "bdd.db");

        // MODE DEV : recopie toujours la BDD de StreamingAssets
#if UNITY_ANDROID && !UNITY_EDITOR
    // Si tu utilises la coroutine Android, tu peux appeler une méthode qui recopie toujours
    StartCoroutine(CopyDBFromStreamingAssetsAndroid(srcPath, dbPath));
#else
        File.Copy(srcPath, dbPath, true);
        Debug.Log("DB recopiée (dev) : " + dbPath);
#endif
    }

    // 
    // C:\Users\sevan\AppData\LocalLow\Sevan&Co\Gestion_Mobile_com\bdd.db

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
    }
#endif

    void Start()
    {
        Debug.Log("DBManager START MOBILE");
        outputText.text = "HELLO MOBILE";
        Debug.Log("DB path = " + dbPath + " | exists = " + File.Exists(dbPath));
        AfficherTables();
        AfficherPersonnes();
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


    void AfficherPersonnes()
    {
        string connStr = "Data Source=" + dbPath + ";Version=3;";
        StringBuilder sb = new StringBuilder();

        try
        {
            Debug.Log("DBMOBILE: AfficherPersonnes START");

            using (var conn = new SqliteConnection(connStr))
            {
                conn.Open();
                Debug.Log("DBMOBILE: connexion OK");

                string sql = "SELECT id, nom FROM personnes";
                using (var cmd = new SqliteCommand(sql, conn))
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        int id = rdr.GetInt32(0);
                        string nom = rdr.GetString(1);
                        sb.AppendLine(id + " - " + nom);
                    }
                }
            }

            if (sb.Length == 0)
            {
                outputText.text = "AUCUNE LIGNE";
                Debug.Log("DBMOBILE: aucune ligne lue");
            }
            else
            {
                outputText.text = sb.ToString();
                Debug.Log("DBMOBILE: lignes lues =\n" + sb.ToString());
            }
        }
        catch (System.Exception e)
        {
            string msg = "ERR: " + e.GetType().Name + " - " + e.Message;
            outputText.text = msg;
            Debug.LogError("DBMOBILE EX: " + msg);
        }
    }

}
