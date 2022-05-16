using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NetUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stateText;
    [SerializeField] TextMeshProUGUI playerText;
    [SerializeField] TextMeshProUGUI pingText;

    MoonNetworkManager moon;
    private void Start()
    {
        moon = MoonNetworkManager.instance;
    }

    private void Update()
    {
        if (moon == null)
        {
            moon = MoonNetworkManager.instance;
        }

        switch (moon.GetConnectionState())
        {
            case NativeWebSocket.WebSocketState.Connecting:
                stateText.text = "Joining";
                playerText.text = "";
                pingText.text = "";
                break;
            case NativeWebSocket.WebSocketState.Open:
                stateText.text = "Connected";
                playerText.text = "Players: " + moon.GetPlayerCount();
                pingText.text = "" + (moon.GetPing() * 100).ToString("F0");
                break;
            case NativeWebSocket.WebSocketState.Closing:
                stateText.text = "Leaving";
                playerText.text = "";
                pingText.text = "";
                break;
            case NativeWebSocket.WebSocketState.Closed:
                stateText.text = "Disconnected";
                playerText.text = "";
                pingText.text = "";
                break;
            default:
                stateText.text = "Disconnected";
                playerText.text = "";
                pingText.text = "";
                break;
        }



    }
}
