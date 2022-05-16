using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoonNetIdentity))]
public class MoonTransform : MonoBehaviour
{
    [Header("Is Player Object?")]
    [SerializeField] bool clientAuthority = true;

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


        MoonNetworkManager.instance.ClientSendMessage(MoonNetworkManager.instance.MOON_KEY + GetComponent<MoonNetIdentity>().netIdentity + ' ' + locMessage);

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
