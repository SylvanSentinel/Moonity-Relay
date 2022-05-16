using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkMessage
{
    public int playerID;

    public string playerName = "Borpa";

    public string commands = "";

    public Vector3 position = Vector3.zero;

    public Vector3 rotation = Vector3.zero;

}
