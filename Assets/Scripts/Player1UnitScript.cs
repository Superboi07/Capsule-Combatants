using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1UnitScript : MonoBehaviour
{
    //it only sends the player number now, but it might come in a handy later
    object[] Data = new object[] { 1 };

    void Awake()
    {
        SendMessage("DataPacket", Data);
    }

}