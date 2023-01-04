using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Reinforce : MonoBehaviour
{
    public static int P1Points;
    public static int P1SpecialPoints;

    public static char[,] P1Area = new char[,] { { ' ', ' ', ' ', ' ', ' ', ' ', ' ' }, { ' ', ' ', ' ', ' ', ' ', ' ', ' ' }, { ' ', ' ', ' ', ' ', ' ', ' ', ' ' }, { ' ', ' ', ' ', ' ', ' ', ' ', ' ' }, { ' ', ' ', ' ', ' ', ' ', ' ', ' ' }, { ' ', ' ', ' ', ' ', ' ', ' ', ' ' } };

    bool ReapOnce;
    int prevOrgin;

    // Start is called before the first frame update
    void Start()
    {
        P1Points = 35;

        //move Reinforce to MessageRelay once I make it
        Reinforce();

        Debug.Log(P1Area[0, 0].ToString() + P1Area[0, 1].ToString() + P1Area[0, 2].ToString() + P1Area[0, 3].ToString() + P1Area[0, 4].ToString() + P1Area[0, 5].ToString() + P1Area[0, 6].ToString() + "\n" + (P1Area[1, 0].ToString() + P1Area[1, 1].ToString() + P1Area[1, 2].ToString() + P1Area[1, 3].ToString() + P1Area[1, 4].ToString() + P1Area[1, 5].ToString() + P1Area[1, 6].ToString() + "\n" + P1Area[2, 0].ToString() + P1Area[2, 1].ToString() + P1Area[2, 2].ToString() + P1Area[2, 3].ToString() + P1Area[2, 4].ToString() + P1Area[2, 5].ToString() + P1Area[2, 6].ToString() + "\n" + P1Area[3, 0].ToString() + P1Area[3, 1].ToString() + P1Area[3, 2].ToString() + P1Area[3, 3].ToString() + P1Area[3, 4].ToString() + P1Area[3, 5].ToString() + P1Area[3, 6].ToString() + "\n" + P1Area[4, 0].ToString() + P1Area[4, 1].ToString() + P1Area[4, 2].ToString() + P1Area[4, 3].ToString() + P1Area[4, 4].ToString() + P1Area[4, 5].ToString() + P1Area[4, 6].ToString() + "\n" + P1Area[5, 0].ToString() + P1Area[5, 1].ToString() + P1Area[5, 2].ToString() + P1Area[5, 3].ToString() + P1Area[5, 4].ToString() + P1Area[5, 5].ToString() + P1Area[5, 6].ToString()));
    }

    void Reinforce()
    {
        int ran = Random.Range(0, 35);

        if (ran <= 9)
        {
            P1Points -= 1;
            Place('r', false, 0);
        }
        else if (ran <= 19)
        {
            P1Points -= 1;
            Place('b', false, 0);
        }
        else if (ran <= 29)
        {
            P1Points -= 1;
            Place('g', false, 0);
        }
        else if (P1SpecialPoints >= 2 || P1Points < 5)
        {
            Reinforce();
        }
        else if (ran <= 31)
        {
            P1Points -= 5;
            P1SpecialPoints += 1;
            Place('S', false, 0);
        }
        else if (P1Points >= 10)
        {
            P1Points -= 10;
            P1SpecialPoints += 2;
            Place('$', false, 0);
        }
        else
        {
            Reinforce();
        }
    }

    void Place(char type, bool isReap, int prev)
    {
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
                //nothing
            }
        }
        else if (ran == 6 && type == '$')
        {
            Place(type, true, ran);
        }
        else if (P1Area[5, ran] != ' ')
        {
            Place(type, true, ran);
        }
        else if (P1Area[4, ran] != ' ')
        {
            if (type != 'S' && type != '$')
            {
                P1Area[5, ran] = type;
            }
            else
            {
                Place(type, true, ran);
            }
        }
        else if (type == '$')
        {
            int temp;
            int temp2;

            if (P1Area[3, ran] != ' ')
            {
                temp = 4;
            }
            else if (P1Area[2, ran] != ' ')
            {
                temp = 3;
            }
            else if (P1Area[1, ran] != ' ')
            {
                temp = 2;
            }
            else if (P1Area[0, ran] != ' ')
            {
                temp = 1;
            }
            else
            {
                temp = 0;
            }

            ran += 1;

            if (P1Area[3, ran] != ' ')
            {
                temp2 = 4;
            }
            else if (P1Area[2, ran] != ' ')
            {
                temp2 = 3;
            }
            else if (P1Area[1, ran] != ' ')
            {
                temp2 = 2;
            }
            else if (P1Area[0, ran] != ' ')
            {
                temp2 = 1;
            }
            else
            {
                temp2 = 0;
            }

            if (temp >= temp2)
            {
                P1Area[temp, ran - 1] = '$';
                P1Area[temp, ran] = '$';
                P1Area[temp + 1, ran - 1] = '$';
                P1Area[temp + 1, ran] = '$';
            }
        }
        else if (P1Area[3, ran] != ' ')
        {
            P1Area[4, ran] = type;

            if (type == 'S')
            {
                P1Area[5, ran] = 'S';
            }
        }
        else if (P1Area[2, ran] != ' ')
        {
            P1Area[3, ran] = type;

            if (type == 'S')
            {
                P1Area[4, ran] = 'S';
            }
        }
        else if (P1Area[1, ran] != ' ')
        {
            P1Area[2, ran] = type;

            if (type == 'S')
            {
                P1Area[3, ran] = 'S';
            }
        }
        else if (P1Area[0, ran] != ' ')
        {
            P1Area[1, ran] = type;

            if (type == 'S')
            {
                P1Area[2, ran] = 'S';
            }
        }
        else
        {
            P1Area[0, ran] = type;

            if (type == 'S')
            {
                P1Area[1, ran] = 'S';
            }
        }

        if (P1Points > 0)
        {
            ReapOnce = false;
            Reinforce();
        }
    }
}