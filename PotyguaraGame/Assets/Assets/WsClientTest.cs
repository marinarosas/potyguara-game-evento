using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WsClientTest : MonoBehaviour
{
    WebSocket ws;
    // Start is called before the first frame update
    void Start()
    {
        ws = new WebSocket ("ws://localhost:9000/");

        ws.OnMessage += (sender, e) => {
              Debug.Log("Server says: " + e.Data);
        };
            
        ws.Connect ();
        ws.Send ("BALUS");
        Debug.Log("coisado");

        string json = "{ \"type\": \"move\", \"actor\": \"player1\", \"target\": \"(1,1,1)\", \"parameters\": \"\" }";
        ActionServer action = ActionServer.FromJson(json);
        Debug.Log(json);
        Debug.Log(action.ToJson());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
