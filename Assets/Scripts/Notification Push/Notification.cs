using UnityEngine;
using Unity.Notifications.Android;
using System.Collections;
using UnityEngine.Android;

public class TestNotifications : MonoBehaviour
{
    private const string CHANNEL_ID = "test_channel";

    void Start()
    {
        StartCoroutine(InitializeNotifications());
    }

    IEnumerator InitializeNotifications()
    {
        // Channel d'abord
        var channel = new AndroidNotificationChannel()
        {
            Id = CHANNEL_ID,
            Name = "Canal Test Sevan",
            Importance = Importance.High,
            Description = "Notifs de test pour ton app Unity"
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        // Permission Android 13+ avec PermissionRequest du package
        var request = new PermissionRequest();
        while (request.Status == PermissionStatus.RequestPending)
            yield return null;

        Debug.Log("Permission status: " + request.Status);
    }



    // Appel ça via un bouton pour tester
    public void SendTestNotification()
    {
        var notif = new AndroidNotification()
        {
            Title = "Yo Sevan !",
            Text = "Notif test de Unity sur ton Xiaomi ! 🎮",
            SmallIcon = "icon_0",  // Optionnel, met une icône dans res/drawable
            LargeIcon = "icon_1",
            FireTime = System.DateTime.Now.AddSeconds(5)  // Dans 5 secondes
        };

        AndroidNotificationCenter.SendNotification(notif, CHANNEL_ID);
        Debug.Log("Notif envoyée pour dans 5s !");
    }

    // Bonus : Callback quand reçue
    void OnEnable()
    {
        AndroidNotificationCenter.OnNotificationReceived += OnNotificationReceived;
    }

    void OnDisable()
    {
        AndroidNotificationCenter.OnNotificationReceived -= OnNotificationReceived;
    }

    void OnNotificationReceived(AndroidNotificationIntentData data)
    {
        Debug.Log("Notif reçue : " + data.Notification.Title + " - " + data.Notification.Text);
    }
}
