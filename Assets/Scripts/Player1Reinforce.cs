using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Reinforce : MonoBehaviour
{

    public static int P1Points;
    public static int P1SpecialPoints;

    public static string[,] P1Area = new string[,] {{" ", " ", " ", " ", " ", " ", " "}, {" ", " ", " ", " ", " ", " ", " "}, {" ", " ", " ", " ", " ", " ", " "}, {" ", " ", " ", " ", " ", " ", " "}, {" ", " ", " ", " ", " ", " ", " "}, {" ", " ", " ", " ", " ", " ", " "}};

    object[] tempStorage = new object[] {0, 0, " "};

    bool isStart;
    bool ReapOnce;
    int prevOrgin;

    // Start is called before the first frame update
    void Start()
    {
        P1Points = 35;

        //move these to MessageRelay once I make it
        isStart = true;
        P1Starting();

        Debug.Log(P1Area[0, 0] + P1Area[0, 1] + P1Area[0, 2] + P1Area[0, 3] + P1Area[0, 4] + P1Area[0, 5] + P1Area[0, 6] + "\n" + (P1Area[1, 0] + P1Area[1, 1] + P1Area[1, 2] + P1Area[1, 3] + P1Area[1, 4] + P1Area[1, 5] + P1Area[1, 6] + "\n" + P1Area[2, 0] + P1Area[2, 1] + P1Area[2, 2] + P1Area[2, 3] + P1Area[2, 4] + P1Area[2, 5] + P1Area[2, 6] + "\n" + P1Area[3, 0] + P1Area[3, 1] + P1Area[3, 2] + P1Area[3, 3] + P1Area[3, 4] + P1Area[3, 5] + P1Area[3, 6] + "\n" + P1Area[4, 0] + P1Area[4, 1] + P1Area[4, 2] + P1Area[4, 3] + P1Area[4, 4] + P1Area[4, 5] + P1Area[4, 6] + "\n" + P1Area[5, 0] + P1Area[5, 1] + P1Area[5, 2] + P1Area[5, 3] + P1Area[5, 4] + P1Area[5, 5] + P1Area[5, 6]));
    }

    void P1Starting()
    {
        int ran = Random.Range(0, 35);

        if (ran <= 9)
        {
            P1Points -= 1;
            Place("r", false, 0);
        }
        else if (ran <= 19)
        {
            P1Points -= 1;
            Place("b", false, 0);
        }
        else if (ran <= 29)
        {
            P1Points -= 1;
            Place("g", false, 0);
        }
        else if (P1SpecialPoints >= 2 || P1Points < 5)
        {
            P1Starting();
        }
        else if (ran <= 31)
        {
            P1Points -= 5;
            P1SpecialPoints += 1;
            Place("S", false, 0);
        }
        else if (P1Points >= 10)
        {
            P1Points -= 10;
            P1SpecialPoints += 2;
            Place("$", false, 0);
        }
        else
        {
            P1Starting();
        }
    }

    void P1Reinforce()
    {
        int ran = Random.Range(0, 35);

        if (ran <= 6)
        {
            P1Points -= 1;
            Place("r", false, 0);
        }
        else if (ran <= 13)
        {
            P1Points -= 1;
            Place("b", false, 0);
        }
        else if (ran <= 20)
        {
            P1Points -= 1;
            Place("g", false, 0);
        }
        else if (P1SpecialPoints >= 2 || P1Points < 5)
        {
            P1Reinforce();
        }
        else if (ran <= 31)
        {
            P1Points -= 5;
            P1SpecialPoints += 1;
            Place("S", false, 0);
        }
        else if (P1Points >= 10)
        {
            P1Points -= 10;
            P1SpecialPoints += 2;
            Place("$", false, 0);
        }
        else
        {
            P1Reinforce();
        }
    }

    void Place(string type, bool isReap, int prev)
    {
        Debug.Log(P1Area[0, 0] + P1Area[0, 1] + P1Area[0, 2] + P1Area[0, 3] + P1Area[0, 4] + P1Area[0, 5] + P1Area[0, 6] + "\n" + (P1Area[1, 0] + P1Area[1, 1] + P1Area[1, 2] + P1Area[1, 3] + P1Area[1, 4] + P1Area[1, 5] + P1Area[1, 6] + "\n" + P1Area[2, 0] + P1Area[2, 1] + P1Area[2, 2] + P1Area[2, 3] + P1Area[2, 4] + P1Area[2, 5] + P1Area[2, 6] + "\n" + P1Area[3, 0] + P1Area[3, 1] + P1Area[3, 2] + P1Area[3, 3] + P1Area[3, 4] + P1Area[3, 5] + P1Area[3, 6] + "\n" + P1Area[4, 0] + P1Area[4, 1] + P1Area[4, 2] + P1Area[4, 3] + P1Area[4, 4] + P1Area[4, 5] + P1Area[4, 6] + "\n" + P1Area[5, 0] + P1Area[5, 1] + P1Area[5, 2] + P1Area[5, 3] + P1Area[5, 4] + P1Area[5, 5] + P1Area[5, 6]));
        Debug.Log(P1Points);

        int ran = Random.Range(0, 7);

        if (isReap == true)
        {
            if (ReapOnce == false)
            {
                prevOrgin = prev;
            }

            ReapOnce = true;

            ran = prev + 1;

            if (ran == 7)
            {
                ran = 0;
            }

            if (ran == prevOrgin)
            {
                if (type == "$")
                {
                    P1Points += 10;
                }
                else if (type == "S")
                {
                    P1Points += 5;
                }
                else
                {
                    P1Points += 1;
                }
                
                Debug.Log(type + " overflowed");
            }
        }
        else if (P1Area[5, ran] != " ")
        {
            Place(type, true, ran);
        }
        else if (P1Area[4, ran] != " ")
        {
            if (type == "S" || type == "$")
            {
                Place(type, true, ran);
            } 
            else
            {
                tempStorage = new object[] {5, ran, type};

                if ((string)CheckMerge(tempStorage) == "x")
                {
                    P1Area[5, ran] = type;
                    BroadcastMessage("Spawn", tempStorage);   
                }
                else
                {
                    Place(type, true, ran);
                }
            }
        }
        else if (type == "$")
        {
            if (ran == 6)
            {
                Place(type, true, ran);
            }
            else 
            {    
                int temp;
                int temp2;
            
                if (P1Area[3, ran] != " ")
                {
                    temp = 4;
                }
                else if (P1Area[2, ran] != " ")
                {
                    temp = 3;
                }
                else if (P1Area[1, ran] != " ")
                {
                    temp = 2;
                }
                else if (P1Area[0, ran] != " ")
                {
                    temp = 1;
                }
                else
                {
                    temp = 0;
                }

                ran += 1;

                if (P1Area[3, ran] != " ")
                {
                    temp2 = 4;
                }
                else if (P1Area[2, ran] != " ")
                {
                    temp2 = 3;
                }
                else if (P1Area[1, ran] != " ")
                {
                    temp2 = 2;
                }
                else if (P1Area[0, ran] != " ")
                {
                    temp2 = 1;
                }
                else
                {
                    temp2 = 0;
                }

                if (temp >= temp2)
                {
                    P1Area[temp, ran - 1] = "$";
                    P1Area[temp, ran] = "$";
                    P1Area[temp + 1, ran - 1] = "$";
                    P1Area[temp + 1, ran] = "$";
                    tempStorage = new object[] {temp, ran - 1, "$"};
                    BroadcastMessage("Spawn", tempStorage);
                }
                else
                {
                    P1Area[temp2, ran - 1] = "$";
                    P1Area[temp2, ran] = "$";
                    P1Area[temp2 + 1, ran - 1] = "$";
                    P1Area[temp2 + 1, ran] = "$";
                    tempStorage = new object[] {temp, ran - 1, "$"};
                    BroadcastMessage("Spawn", tempStorage);
                }
            }
        }
        else if (P1Area[3, ran] != " ")
        {
            if (type == "S")
            {
                P1Area[4, ran] = "S";
                P1Area[5, ran] = "S";
                tempStorage = new object[] {4, ran, "S"};
                BroadcastMessage("Spawn", tempStorage);
            }
            else
            {
                tempStorage = new object[] {4, ran, type};

                if ((string)CheckMerge(tempStorage) == "x")
                {
                    P1Area[4, ran] = type;
                    tempStorage = new object[] {4, ran, type};
                    BroadcastMessage("Spawn", tempStorage);   
                }
                else
                {
                    Place(type, true, ran);
                }
            }
        }
        else if (P1Area[2, ran] != " ")
        {
            if (type == "S")
            {
                P1Area[3, ran] = "S";
                P1Area[4, ran] = "S";
                tempStorage = new object[] {3, ran, "S"};
                BroadcastMessage("Spawn", tempStorage);
            }
            else
            {
                tempStorage = new object[] {3, ran, type};

                if ((string)CheckMerge(tempStorage) == "x")
                {
                    P1Area[3, ran] = type;
                    tempStorage = new object[] {3, ran, type};
                    BroadcastMessage("Spawn", tempStorage);   
                }
                else
                {
                    Place(type, true, ran);
                }
            }
        }
        else if (P1Area[1, ran] != " ")
        {
            if (type == "S")
            {
                P1Area[1, ran] = "S";
                P1Area[2, ran] = "S";
                tempStorage = new object[] {2, ran, "S"};
                BroadcastMessage("Spawn", tempStorage);
            }
            else
            {
                tempStorage = new object[] {2, ran, type};

                if ((string)CheckMerge(tempStorage) == "x")
                {
                    P1Area[2, ran] = type;
                    tempStorage = new object[] {2, ran, type};
                    BroadcastMessage("Spawn", tempStorage);   
                }
                else
                {
                    Place(type, true, ran);
                }
            }
        }
        else if (P1Area[0, ran] != " ")
        {
            if (type == "S")
            {
                P1Area[1, ran] = "S";
                P1Area[2, ran] = "S";
                tempStorage = new object[] {1, ran, "S"};
                BroadcastMessage("Spawn", tempStorage);
            }   
            else 
            {
                tempStorage = new object[] {1, ran, type};

                if ((string)CheckMerge(tempStorage) == "x")
                {
                    P1Area[1, ran] = type;
                    tempStorage = new object[] {1, ran, type};
                    BroadcastMessage("Spawn", tempStorage);   
                }
                else
                {
                    Place(type, true, ran);
                }
            }   
        }
        else
        {
            if (type == "S")
            {
                P1Area[0, ran] = "S";
                P1Area[1, ran] = "S";
                tempStorage = new object[] {0, ran, "S"};
                BroadcastMessage("Spawn", tempStorage);
            }
            else
            {
                tempStorage = new object[] {0, ran, type};

                if ((string)CheckMerge(tempStorage) == "x")
                {
                    P1Area[0, ran] = type;
                    tempStorage = new object[] {0, ran, type};
                    BroadcastMessage("Spawn", tempStorage);   
                }
                else
                {
                    Place(type, true, ran);
                }
            }            
        }

        if (P1Points > 0)
        {
            ReapOnce = false;

            if (isStart == false)
            {
                P1Reinforce();
            }
            else
            {
                P1Starting();
            }
        }
        else
        {
            isStart = false;
        }
    }

    string CheckMerge(object[] temp)
    {
        if ((int)temp[0] >= 3 && (P1Area[(int)temp[0] - 1, (int)temp[1]] == (string)temp[2] && P1Area[(int)temp[0] - 2, (int)temp[1]] == "S"))
        {
            Debug.Log("S");
            return("S");
        }
        else if ((int)temp[0] >= 2 && (P1Area[(int)temp[0] - 1, (int)temp[1]] == (string)temp[2] && P1Area[(int)temp[0] - 2, (int)temp[1]] == (string)temp[2]))
        {
            Debug.Log("|");
            return("|");
        }
        else if ((int)temp[1] != 0)
        {
            if ((int)temp[0] >= 3 && (P1Area[(int)temp[0] - 2, (int)temp[1]] == "$" && P1Area[(int)temp[0] - 1, (int)temp[1]] == (string)temp[2] && ((P1Area[(int)temp[0] - 2, (int)temp[1] - 1] == "$" && P1Area[(int)temp[0] - 1, (int)temp[1] - 1] == (string)temp[2] && P1Area[(int)temp[0], (int)temp[1] - 1] == (string)temp[2]) || ((int)temp[1] != 6 && P1Area[(int)temp[0] - 2, (int)temp[1] + 1] == "$" && P1Area[(int)temp[0] - 1, (int)temp[1] + 1] == (string)temp[2] && P1Area[(int)temp[0], (int)temp[1] + 1] == (string)temp[2]))))
            {
                Debug.Log("$");
                return("$");
            }
            else if (P1Area[(int)temp[0], (int)temp[1] - 1] == (string)temp[2])
            {
                if ((int)temp[1] != 6)
                {
                    if (P1Area[(int)temp[0], (int)temp[1] + 1] == (string)temp[2])
                    {
                        Debug.Log("_");
                        return("_");
                    }
                    else if ((int)temp[1] != 1)
                    {
                        if (P1Area[(int)temp[0], (int)temp[1] - 2] == (string)temp[2])
                        {
                            Debug.Log("_");
                            return("_");
                        }
                        else
                        {
                            Debug.Log("x");
                            return("x");
                        }
                    }
                    else
                    {
                        Debug.Log("x");
                        return("x");
                    }
                }
                else if (P1Area[(int)temp[0], (int)temp[1] - 2] == (string)temp[2])
                {
                    Debug.Log("_");
                    return("_");
                }
                else
                {
                    Debug.Log("x");
                    return("x");
                }
            }
            else if ((int)temp[1] < 5 && P1Area[(int)temp[0], (int)temp[1] + 1] == (string)temp[2] && P1Area[(int)temp[0], (int)temp[1] + 2] == (string)temp[2])
            {
                Debug.Log("_");
                return("_");
            }
            else
            {
                Debug.Log("x");
                return("x");
            }
        }
        else if ((int)temp[1] < 5 && P1Area[(int)temp[0], (int)temp[1] + 1] == (string)temp[2] && P1Area[(int)temp[0], (int)temp[1] + 2] == (string)temp[2])
        {
            Debug.Log("_");
            return("_");
        }
        else
        {
            Debug.Log("x");
            return("x");
        }
    }
}