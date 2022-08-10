using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WebSocketSharp;

public class BiDirectionalChat : MonoBehaviour
{
    [Tooltip("Server URL, e.g., ws://localhost:8943")]
    [SerializeField] private string serverURL;

    private WebSocket _conn;

    private readonly int _peerId = 666666;
    private readonly int _myId = 777777;

    private bool _connReady = false;

    private int number = 1;

    void Start()
    {
        _conn = new WebSocket(serverURL);

        _conn.OnOpen += OnOpen;
        _conn.OnError += OnError;
        _conn.OnMessage += OnMessage;
        _conn.OnClose += OnClose;

        _conn.ConnectAsync();
    }

    void OnClose(object sender, CloseEventArgs e)
    {
        Debug.Log("OnClose");
    }

    void OnError(object sender, ErrorEventArgs e)
    {
        Debug.Log("OnError");

        Debug.LogError(e.Message);
    }

    void OnOpen(object sender, System.EventArgs e)
    {
        Debug.Log("OnOpen");
        var ws = sender as WebSocket;
        ws.SendAsync($"HELLO HOLOLENS-{_myId}", (b) => Debug.Log($"Opened {b}"));
    }

    void OnMessage(object sender, MessageEventArgs args)
    {
        Debug.Log("OnMessage");
        var msg = args.Data;
        switch (msg)
        {
            case "HELLO":
                SetupCall();
                break;
            case "SESSION_OK":
                SendOffer();
                break;
            default:
                if (msg.StartsWith("ERROR"))
                {
                    Debug.LogError(msg);
                }
                else if (msg.StartsWith("DATA"))
                {
                    HandleData(msg);
                }
                else
                {
                    Debug.Log("Handle SDP");
                    HandleSdp(msg);
                }

                break;
        }
    }

    void SetupCall()
    {
        _conn.Send($"SESSION PC-{_peerId}");
    }

    void HandleData(string msg)
    {
        Debug.Log($"Handle data: {msg}");
    }

    void HandleSdp(string msg)
    {
        Debug.Log("HandleSdp");
        _connReady = true;
    }

    private void SendOffer()
    {
        var bodyRaw = Encoding.UTF8.GetBytes("{'sdp': 'initial sdp'}");

        _conn.SendAsync(bodyRaw, (b) => Debug.Log($"SendOffer {b}"));
    }

    void Update()
    {
        if (_connReady)
        {
            _conn.SendAsync(Encoding.UTF8.GetBytes($"DATA {number}"), (b) => { });
            number += 1;
        }
    }
}