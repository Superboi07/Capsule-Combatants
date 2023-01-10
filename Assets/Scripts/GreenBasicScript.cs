using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBasicScript : MonoBehaviour
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
                if (this.transform.position.x == PosData.XAxis[0])
                {
                    Position[0] = 0;
                }
                else if (this.transform.position.x == PosData.XAxis[1])
                {
                    Position[0] = 1;
                }
                else if (this.transform.position.x == PosData.XAxis[2])
                {
                    Position[0] = 2;
                }
                else if (this.transform.position.x == PosData.XAxis[3])
                {
                    Position[0] = 3;
                }
                else if (this.transform.position.x == PosData.XAxis[4])
                {
                    Position[0] = 4;
                }
                else
                {
                    Position[0] = 5;
                }
            }
            else
            {
                if (this.transform.position.x == PosData.XAxis[6])
                {
                    Position[0] = 0;
                }
                else if (this.transform.position.x == PosData.XAxis[7])
                {
                    Position[0] = 1;
                }
                else if (this.transform.position.x == PosData.XAxis[8])
                {
                    Position[0] = 2;
                }
                else if (this.transform.position.x == PosData.XAxis[9])
                {
                    Position[0] = 3;
                }
                else if (this.transform.position.x == PosData.XAxis[10])
                {
                    Position[0] = 4;
                }
                else
                {
                    Position[0] = 5;
                }
            }

            if (this.transform.position.y == PosData.YAxis[0])
            {
                Position[1] = 0;
            }
            else if (this.transform.position.y == PosData.YAxis[1])
            {
                Position[1] = 1;
            }
            else if (this.transform.position.y == PosData.YAxis[2])
            {
                Position[1] = 2;
            }
            else if (this.transform.position.y == PosData.YAxis[3])
            {
                Position[1] = 3;
            }
            else if (this.transform.position.y == PosData.YAxis[4])
            {
                Position[1] = 4;
            }
            else if (this.transform.position.y == PosData.YAxis[5])
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
        if ((string)temp[2] == "g" && this.transform.position.x == 0.0f)
        {
            if (PlayerNum == 2)
            {
                int temp2 = (int)temp[0];
                temp2 += 6;
            }

            MundD = new Vector3(PosData.XAxis[(int)temp[0]], PosData.YAxis[(int)temp[1]], this.transform.position.z);

            Instantiate(this, MundD, this.transform.rotation, ParentObj.transform);
        }
    }
}