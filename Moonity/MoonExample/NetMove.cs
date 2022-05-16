using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMove : MonoBehaviour
{
    public float speed = 2;
    public float lerpSpeed = 2;
    public bool isClient = false;
    public string playerName;

    private MoonNetIdentity identity;

    private void Awake()
    {
        identity = GetComponent<MoonNetIdentity>();
    }

    private void Start()
    {
        isClient = identity.isClient;
        
    }


    private void Update()
    {
        /*
        if(isClient == false)
        {
            if (Vector3.Distance(transform.position, pos) > .1f)
            {
                float step = lerpSpeed * Time.deltaTime; // calculate distance to move
                transform.position = Vector3.MoveTowards(transform.position, pos, step);
            }
        }
        */


        if (!isClient) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        gameObject.transform.position = new Vector2(transform.position.x + (h * speed),
           transform.position.y + (v * speed));


        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Connection.NetManager.Send("sxbPPressed Space");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Connection.NetManager.Send("sxbPPressed W");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Connection.NetManager.Send("sxbPPressed A");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Connection.NetManager.Send("sxbPPressed S");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Connection.NetManager.Send("sxbPPressed D");
        }
        */



        

    }

    Vector3 pos;
    public void MOVETO(Vector3 p)
    {
        if (isClient) return;

        pos = p;
    }


}
