using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBasicScript : MonoBehaviour
{
    //it only sends the player number now, but it might come in a handy later
    object[] Data = new object[] {"b"};

    void Awake()
    {
        SendMessage("DataPacket2", Data);
    }

}