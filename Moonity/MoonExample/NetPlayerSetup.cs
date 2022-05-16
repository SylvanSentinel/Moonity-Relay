using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NetPlayerSetup : MonoBehaviour
{
    private MoonNetIdentity identity;

    [SerializeField] private TextMeshPro namText;


    public void SetName(string n)
    {
        namText.text = n;
    }


    [SerializeField] Component[] removeThese;
    private void Start()
    {
        identity = GetComponent<MoonNetIdentity>();

        if(identity.isClient == false)
        {
            for (int i = 0; i < removeThese.Length; i++)
            {
                //Destroy(removeThese[i]);
            }
        }


        SetName(GameStart.instance.playerName);

        
    }






}
