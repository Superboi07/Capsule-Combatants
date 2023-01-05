using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Reinforce : MonoBehaviour
{
    public static int P1Points;
    public static int P1SpecialPoints;

    public static string[,] P1Area = new string[,] { { " ", " ", " ", " ", " ", " ", " " }, { " ", " ", " ", " ", " ", " ", " " }, { " ", " ", " ", " ", " ", " ", " " }, { " ", " ", " ", " ", " ", " ", " " }, { " ", " ", " ", " ", " ", " ", " " }, { " ", " ", " ", " ", " ", " ", " " } };

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
                if (type == "r" || type == "b" || type == "g")
                {
                    P1Points += 1;
                }
                else if (type == "S")
                {
                    P1Points += 5;
                }
                else
                {
                    P1Points += 10;
                }
            }
        }
        else if (ran == 6 && type == "$")
        {
            Place(type, true, ran);
        }
        else if (P1Area[5, ran] != " ")
        {
            Place(type, true, ran);
        }
        else if (P1Area[4, ran] != " ")
        {
            if (type != "S" && type != "$")
            {
                if (P1Area[4, ran] == type && (P1Area[3, ran] == type || P1Area[3, ran] == "S"))
                {
                    Place(type, true, ran);
                }
                else if (ran != 0)
                {
                    if (P1Area[3, ran] == "$" && P1Area[4, ran] == type && ((P1Area[3, ran - 1] == "$" && P1Area[4, ran - 1] == type && P1Area[5, ran - 1] == type) || (ran != 6 && P1Area[3, ran + 1] == "$" && P1Area[4, ran + 1] == type && P1Area[5, ran + 1] == type)))
                    {
                        Place(type, true, ran);
                    }
                    else if (P1Area[5, ran - 1] == type)
                    {
                        if (ran != 6)
                        {
                            if (P1Area[5, ran + 1] == type)
                            {
                                Place(type, true, ran);
                            }
                            else if (ran != 1)
                            {
                                if (P1Area[5, ran - 2] == type)
                                {
                                    Place(type, true, ran);
                                }
                                else
                                {
                                    P1Area[5, ran] = type;
                                }
                            }
                            else
                            {
                                P1Area[5, ran] = type;
                            }
                        }
                        else if (P1Area[5, ran - 2] == type)
                        {
                            Place(type, true, ran);
                        }
                        else
                        {
                            P1Area[5, ran] = type;
                        }
                    }
                    else
                    {
                        P1Area[5, ran] = type;
                    }
                }
                else if (P1Area[5, ran + 1] == type && P1Area[5, ran + 2] == type)
                {
                    Place(type, true, ran);
                }
                else
                {
                    P1Area[5, ran] = type;
                }
            }
            else
            {
                Place(type, true, ran);
            }
        }
        else if (type == "$")
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
            }
        }
        else if (P1Area[3, ran] != " ")
        {
            if (type == "S")
            {
                P1Area[4, ran] = "S";
                P1Area[5, ran] = "S";
            }
            else if (P1Area[3, ran] == type && (P1Area[2, ran] == type || P1Area[2, ran] == "S"))
            {
                Place(type, true, ran);
            }
            else if (ran != 0)
            {
                if (P1Area[2, ran] == "$" && P1Area[3, ran] == type && ((P1Area[2, ran - 1] == "$" && P1Area[3, ran - 1] == type && P1Area[4, ran - 1] == type) || (ran != 6 && P1Area[2, ran + 1] == "$" && P1Area[3, ran + 1] == type && P1Area[4, ran + 1] == type)))
                {
                    Place(type, true, ran);
                }
                else if (P1Area[4, ran - 1] == type)
                {
                    if (ran != 6)
                    {
                        if (P1Area[4, ran + 1] == type)
                        {
                            Place(type, true, ran);
                        }
                        else if (ran != 1)
                        {
                            if (P1Area[4, ran - 2] == type)
                            {
                                Place(type, true, ran);
                            }
                            else
                            {
                                P1Area[4, ran] = type;
                            }
                        }
                        else
                        {
                            P1Area[4, ran] = type;
                        }
                    }
                    else if (P1Area[4, ran - 2] == type)
                    {
                        Place(type, true, ran);
                    }
                    else
                    {
                        P1Area[4, ran] = type;
                    }
                }
                else
                {
                    P1Area[4, ran] = type;
                }
            }
            else if (P1Area[4, ran + 1] == type && P1Area[4, ran + 2] == type)
            {
                Place(type, true, ran);
            }
            else
            {
                P1Area[4, ran] = type;
            }
        }
        else if (P1Area[2, ran] != " ")
        {
            if (type == "S")
            {
                P1Area[3, ran] = "S";
                P1Area[4, ran] = "S";
            }
            if (P1Area[2, ran] == type && (P1Area[1, ran] == type || P1Area[1, ran] == "S"))
            {
                Place(type, true, ran);
            }
            else if (ran != 0)
            {
                if (P1Area[1, ran] == "$" && P1Area[2, ran] == type && ((P1Area[1, ran - 1] == "$" && P1Area[2, ran - 1] == type && P1Area[3, ran - 1] == type) || (ran != 6 && P1Area[1, ran + 1] == "$" && P1Area[2, ran + 1] == type && P1Area[3, ran + 1] == type)))
                {
                    Place(type, true, ran);
                }
                else if (P1Area[3, ran - 1] == type)
                {
                    if (ran != 6)
                    {
                        if (P1Area[3, ran + 1] == type)
                        {
                            Place(type, true, ran);
                        }
                        else if (ran != 1)
                        {
                            if (P1Area[3, ran - 2] == type)
                            {
                                Place(type, true, ran);
                            }
                            else
                            {
                                P1Area[3, ran] = type;
                            }
                        }
                        else
                        {
                            P1Area[3, ran] = type;
                        }
                    }
                    else if (P1Area[3, ran - 2] == type)
                    {
                        Place(type, true, ran);
                    }
                    else
                    {
                        P1Area[3, ran] = type;
                    }
                }
                else
                {
                    P1Area[3, ran] = type;
                }
            }
            else if (P1Area[3, ran + 1] == type && P1Area[3, ran + 2] == type)
            {
                Place(type, true, ran);
            }
            else
            {
                P1Area[3, ran] = type;
            }
        }
        else if (P1Area[1, ran] != " ")
        {
            if (type == "S")
            {
                P1Area[2, ran] = "S";
                P1Area[3, ran] = "S";
            }
            else if (P1Area[1, ran] == type && P1Area[0, ran] == type)
            {
                Place(type, true, ran);
            }
            else if (ran != 0)
            {
                if (P1Area[2, ran - 1] == type)
                {
                    if (ran != 6)
                    {
                        if (P1Area[2, ran + 1] == type)
                        {
                            Place(type, true, ran);
                        }
                        else if (ran != 1)
                        {
                            if (P1Area[2, ran - 2] == type)
                            {
                                Place(type, true, ran);
                            }
                            else
                            {
                                P1Area[2, ran] = type;
                            }
                        }
                        else
                        {
                            P1Area[2, ran] = type;
                        }
                    }
                    else if (P1Area[2, ran - 2] == type)
                    {
                        Place(type, true, ran);
                    }
                    else
                    {
                        P1Area[2, ran] = type;
                    }
                }
                else
                {
                    P1Area[2, ran] = type;
                }
            }
            else if (P1Area[2, ran + 1] == type && P1Area[2, ran + 2] == type)
            {
                Place(type, true, ran);
            }
            else
            {
                P1Area[2, ran] = type;
            }
        }
        else if (P1Area[0, ran] != " ")
        {
            if (type == "S")
            {
                P1Area[1, ran] = "S";
                P1Area[2, ran] = "S";
            }
            else if (ran != 0)
            {
                if (P1Area[1, ran - 1] == type)
                {
                    if (ran != 6)
                    {
                        if (P1Area[1, ran + 1] == type)
                        {
                            Place(type, true, ran);
                        }
                        else if (ran != 1)
                        {
                            if (P1Area[1, ran - 2] == type)
                            {
                                Place(type, true, ran);
                            }
                            else
                            {
                                P1Area[1, ran] = type;
                            }
                        }
                        else
                        {
                            P1Area[1, ran] = type;
                        }
                    }
                    else if (P1Area[1, ran - 2] == type)
                    {
                        Place(type, true, ran);
                    }
                    else
                    {
                        P1Area[1, ran] = type;
                    }
                }
                else
                {
                    P1Area[1, ran] = type;
                }
            }
            else if (P1Area[1, ran + 1] == type && P1Area[1, ran + 2] == type)
            {
                Place(type, true, ran);
            }
            else
            {
                P1Area[1, ran] = type;
            }
        }
        else
        {
            if (type == "S")
            {
                P1Area[0, ran] = "S";
                P1Area[1, ran] = "S";
            }
            else if (ran != 0)
            {
                if (P1Area[0, ran - 1] == type)
                {
                    if (ran != 6)
                    {
                        if (P1Area[0, ran + 1] == type)
                        {
                            Place(type, true, ran);
                        }
                        else if (ran != 1)
                        {
                            if (P1Area[0, ran - 2] == type)
                            {
                                Place(type, true, ran);
                            }
                            else
                            {
                                P1Area[0, ran] = type;
                            }
                        }
                        else
                        {
                            P1Area[0, ran] = type;
                        }
                    }
                    else if (P1Area[0, ran - 2] == type)
                    {
                        Place(type, true, ran);
                    }
                    else
                    {
                        P1Area[0, ran] = type;
                    }
                }
                else
                {
                    P1Area[0, ran] = type;
                }
            }
            else if (P1Area[0, ran + 1] == type && P1Area[0, ran + 2] == type)
            {
                Place(type, true, ran);
            }
            else
            {
                P1Area[0, ran] = type;
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
}