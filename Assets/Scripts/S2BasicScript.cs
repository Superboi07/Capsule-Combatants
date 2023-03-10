using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S2BasicScript : MonoBehaviour
{
    public PosData PosData;
    public GameObject ParentObj;

    public int PlayerNum;

    public int[] Position = new int[2];
    Vector3 MundD;

    // Start is called before the first frame update
    void Start()
    {
        if (this.transform.position.x != 0.0f)
        {
            if (PlayerNum == 1)
            {
                if (this.transform.position.x == PosData.XAxis[0] + 0.5f)
                {
                    Position[0] = 0;
                }
                else if (this.transform.position.x == PosData.XAxis[1] + 0.5f)
                {
                    Position[0] = 1;
                }
                else if (this.transform.position.x == PosData.XAxis[2] + 0.5f)
                {
                    Position[0] = 2;
                }
                else if (this.transform.position.x == PosData.XAxis[3] + 0.5f)
                {
                    Position[0] = 3;
                }
                else
                {
                    Position[0] = 4;
                }
            }
            else
            {
                if (this.transform.position.x == PosData.XAxis[6] + 0.5f)
                {
                    Position[0] = 0;
                }
                else if (this.transform.position.x == PosData.XAxis[7] + 0.5f)
                {
                    Position[0] = 1;
                }
                else if (this.transform.position.x == PosData.XAxis[8] + 0.5f)
                {
                    Position[0] = 2;
                }
                else if (this.transform.position.x == PosData.XAxis[9] + 0.5f)
                {
                    Position[0] = 3;
                }
                else
                {
                    Position[0] = 4;
                }
            }

            if (this.transform.position.y == PosData.YAxis[0] + 0.5f)
            {
                Position[1] = 0;
            }
            else if (this.transform.position.y == PosData.YAxis[1] + 0.5f)
            {
                Position[1] = 1;
            }
            else if (this.transform.position.y == PosData.YAxis[2] + 0.5f)
            {
                Position[1] = 2;
            }
            else if (this.transform.position.y == PosData.YAxis[3] + 0.5f)
            {
                Position[1] = 3;
            }
            else if (this.transform.position.y == PosData.YAxis[4] + 0.5f)
            {
                Position[1] = 4;
            }
            else if (this.transform.position.y == PosData.YAxis[5] + 0.5f)
            {
                Position[1] = 5;
            }
            else
            {
                Position[1] = 6;
            }
        }
    }

    void DataPacket(object[] temp)
    {
        PlayerNum = (int)temp[0];
    }

    void Spawn(object[] temp)
    {
        if ((string)temp[2] == "$" && this.transform.position.x == 0.0f)
        {
            if (PlayerNum == 2)
            {
                int temp2 = (int)temp[0];
                temp2 += 6;
            }

            MundD = new Vector3(PosData.XAxis[(int)temp[0]] + 0.5f, PosData.YAxis[(int)temp[1]] + 0.5f, this.transform.position.z);
            Debug.Log(temp[0] + ", " + temp[1]);

            Instantiate(this, MundD, this.transform.rotation, ParentObj.transform);
        }
    }
}