using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public class MoonNetworkManager : MonoBehaviour
{
    WebSocket moonSocket;
    public static MoonNetworkManager instance;

    [Header("Info")]
    [SerializeField] string address = "wss://relay.moonjam.dev/v1";
    [SerializeField] bool dontDestroyOnLoad = false;
    public float serverSendRate = 0.4f;
    public string MOON_KEY;

    [Header("Player")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] List<MoonNetIdentity> playerList = new List<MoonNetIdentity>();


    public GameObject GetLocalPlayer()
    {
        return localPlayer;
    }

    private void Awake()
    {
        if (instance)
        {
            Debug.LogWarning("Another Moon Net Manager detected, deleting duplicate");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    //StartServer
    async void Start()
    {
        if (address == "" || address == string.Empty || address == null)
        {
            address = "wss://relay.moonjam.dev/v1";
        }

        moonSocket = new WebSocket(address);

        moonSocket.OnOpen += () =>
        {
            OnClientConnect();
            Debug.Log("Connection open!");
        };

        moonSocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        moonSocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed! " + e);
        };

        moonSocket.OnMessage += (bytes) =>
        {
            //Debug.Log(bytes.Length + " bytes");
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            NetworkRecieveMessage(message);
        };



        // Keep sending messages
        InvokeRepeating("MoonSocketSendMessage", 0.0f, serverSendRate);

        await moonSocket.Connect();
    }

    [SerializeField] GameObject localPlayer;
    void OnClientConnect()
    {
        int ID = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        moonSocket.SendText(MOON_KEY + ID + " join");
        GameObject p = Instantiate(playerPrefab, transform.position, transform.rotation);
        p.gameObject.GetComponent<MoonNetIdentity>().netIdentity = ID;
        p.gameObject.GetComponent<MoonNetIdentity>().isClient = true;
        localPlayer = p;
        playerList.Add(localPlayer.GetComponent<MoonNetIdentity>());
    }

    //Send moon packets to server //async //
    public void MoonSocketSendMessage()
    {
        if(moonSocket.State == WebSocketState.Open)
        {
            //await moonSocket.SendText(MOON_KEY);
            if (localPlayer)
            {
                localPlayer.GetComponent<MoonTransform>().SendNetTransform();
            }
        }
    }
    public void ClientSendMessage(string message)
    {
        if (moonSocket.State == WebSocketState.Open)
        {
            moonSocket.SendText(message);
        }
    }

    //Get Messages from the Relay (null is usually our own)
    //Current only for transform sync
    float latestRecieve;
    float previousRecieve;
    void NetworkRecieveMessage(string message)
    {
        
        if (message == "" || message == string.Empty || message == null)
        {
            previousRecieve = latestRecieve;
            latestRecieve = Time.time;
            return;
        }
        
        //[0]ID [1]X [2]Y [3]Z [4]Rx [5]Ry [6]Rz
        string[] data = message.Split(' ');
        //[1] different state

        if(data[1] == "join")
        {
            GameObject newclient = Instantiate(playerPrefab, transform.position, transform.rotation);
            newclient.GetComponent<MoonNetIdentity>().netIdentity = int.Parse(data[0]);
            playerList.Add(newclient.GetComponent<MoonNetIdentity>());
            return;
        }
        if(data[1] == "leave")
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                if(playerList[i].netIdentity == int.Parse(data[0]))
                {
                    Destroy(playerList[i].gameObject);
                    playerList.RemoveAt(i);
                    return;
                }
            }
        }

        //MOVEMENT_SYNC
        bool doRotation = false;
        float posX = float.Parse(data[1]);
        float posY = float.Parse(data[2]);
        float posZ = float.Parse(data[3]);

        Vector3 position = new Vector3(posX, posY, posZ);
        Vector3 rotation = new Vector3(0,0,0);
        if (data[4] != null)
        {
            doRotation = true;
        }
        if (doRotation)
        {
            float rotX = float.Parse(data[4]);
            float rotY = float.Parse(data[5]);
            float rotZ = float.Parse(data[6]);
            
            rotation = new Vector3(rotX, rotY, rotZ);
            //Debug.Log(rotation);
        }

        MoonNetIdentity[] players = FindObjectsOfType<MoonNetIdentity>();
        bool existsPlayer = false;
        MoonTransform netTransform;
        for (int i = 0; i < players.Length; i++)
        {
            if(players[i].netIdentity == int.Parse(data[0]))
            {
                existsPlayer = true;
                netTransform = players[i].gameObject.GetComponent<MoonTransform>();
                netTransform.SyncNetTransform(position);
                if (doRotation)
                {
                    netTransform.SyncNetRotation(rotation);
                }
                return;
            }
        }
        if (existsPlayer == false)
        {
            GameObject client = Instantiate(playerPrefab, transform.position, transform.rotation);
            client.GetComponent<MoonNetIdentity>().netIdentity = int.Parse(data[0]);
            client.GetComponent<MoonTransform>().SyncNetTransform(position);
            if (doRotation)
            {
                client.GetComponent<MoonTransform>().SyncNetRotation(rotation);
            }
            playerList.Add(client.GetComponent<MoonNetIdentity>());
        }


    }


    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        moonSocket.DispatchMessageQueue();
#endif

    }

    public int GetPlayerCount()
    {
        return playerList.Count;
    }
    public WebSocketState GetConnectionState()
    {
        return moonSocket.State;
    }
    public float GetPing()
    {
        return latestRecieve - previousRecieve;
    }
    

    private async void OnApplicationQuit()
    {
        await moonSocket.SendText(MOON_KEY + localPlayer.GetComponent<MoonNetIdentity>().netIdentity + " leave");
        await moonSocket.Close();
    }

}
