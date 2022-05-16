using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoonNetIdentity))]
public class MoonTransform : MonoBehaviour
{
    [Header("Is Player Object?")]
    [SerializeField] bool clientAuthority = true;
    public string playerName = "Borpa";

    [Header("Sync")]
    [SerializeField] bool syncRotation = true;

    private float sendRate;
    private Vector3 startPos;
    private void Start()
    {
        startPos = transform.position;
        sendRate = MoonNetworkManager.instance.serverSendRate;
    }

    public void SendNetTransform()
    {
        if (clientAuthority)
        {
            if (GetComponent<MoonNetIdentity>().isClient == false) return;
        }
        

        //if (Vector3.Distance(startPos, transform.position) < 0.2f) return; //Dont send if didnt move much

        string pX = transform.position.x.ToString("F2");
        string pY = transform.position.y.ToString("F2");
        string pZ = transform.position.z.ToString("F2");

        string locMessage = pX + ' ' + pY + ' ' + pZ;

        if (syncRotation)
        {
            string rX = transform.rotation.eulerAngles.x.ToString("F2");
            string rY = transform.rotation.eulerAngles.y.ToString("F2");
            string rZ = transform.rotation.eulerAngles.z.ToString("F2");

            locMessage += " " + rX + " " + rY + " " + rZ;
        }


        //MoonNetworkManager.instance.ClientSendMessage(MoonNetworkManager.instance.MOON_KEY + GetComponent<MoonNetIdentity>().netIdentity + ' ' + locMessage);

        MoonNetworkManager.instance.ClientSendMessage(MoonNetworkManager.instance.MOON_KEY + CreateMessage());

    }

    private void OnDisable()
    {
        NetworkMessage networkMessage = new NetworkMessage
        {
            playerID = GetComponent<MoonNetIdentity>().netIdentity,
            playerName = this.playerName,
            commands = "Leave",
            position = Vector3.zero,
            rotation = Vector3.zero
        };

        MoonNetworkManager.instance.ClientSendMessage(MoonNetworkManager.instance.MOON_KEY + CreateMessage());
    }

    public string CreateMessage()
    {

        //Vector3 pos = new Vector3((float)System.Math.Round(transform.position.x, 2), (float)System.Math.Round(transform.position.y, 2), (float)System.Math.Round(transform.position.z, 2));
        //Vector3 rot = new Vector3((float)System.Math.Round(transform.rotation.eulerAngles.x, 2), (float)System.Math.Round(transform.rotation.eulerAngles.y, 2), (float)System.Math.Round(transform.rotation.eulerAngles.z, 2));


        NetworkMessage networkMessage = new NetworkMessage
        {
            playerID = GetComponent<MoonNetIdentity>().netIdentity,
            playerName = this.playerName,
            commands = "Movement",
            position = transform.position,
            rotation = transform.rotation.eulerAngles
        };


        string jsonFile = JsonUtility.ToJson(networkMessage);

        return jsonFile;
    }

    public void SyncNetTransform(Vector3 pos)
    {
        startPos = pos;
        StartCoroutine(SyncPosition(pos,transform.position));
    }
    public void SyncNetRotation(Vector3 rot)
    {
        StartCoroutine(SyncRotation(Quaternion.Euler(rot), sendRate));
    }

    IEnumerator SyncPosition(Vector3 pos, Vector3 startPos)
    {
        float timeElapsed = 0;

        while (timeElapsed < sendRate)
        {
            transform.position = Vector3.Lerp(startPos, pos, timeElapsed / sendRate);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = pos;
    }

    IEnumerator SyncRotation(Quaternion endValue, float duration)
    {
        float time = 0;
        Quaternion startValue = transform.rotation;

        while (time < duration)
        {
            transform.rotation = Quaternion.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endValue;
    }

}
