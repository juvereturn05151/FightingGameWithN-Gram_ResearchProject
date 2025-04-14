using UnityEngine;
using System.Collections.Generic;



public class N_Gram : MonoBehaviour
{
    public static ActionChance calculateNextPick(Queue<Actiontype> actionlog)
    {
        ActionChance chances = new ActionChance();

        if (actionlog.Count == 0) 
        { 
            return chances; 
        }

        ActionChance[] ActionCount = new ActionChance[]
        {
            new ActionChance(), //Attacking
            new ActionChance(), //Blocking
            new ActionChance()  //Grabbing
        };

        Actiontype[] index = actionlog.ToArray();
        for (int i = 0; i < actionlog.Count - 1; i++) 
        {
            int type = (int)index[i];
            int nextType = (int)index[i + 1];
            switch (nextType) 
            {
                case(0):
                    ActionCount[type].Attack += 1;
                    break;
                case(1):
                    ActionCount[type].Block += 1;
                    break;
                case(2):
                    ActionCount[type].Throw += 1;
                    break;
            }
        }
        //ActionChance chances = new ActionChance();

        int relevant_chances = (int)index[actionlog.Count - 1];
        chances = ActionCount[relevant_chances];
        chances.Normalize();

        return chances;
    }

    //Calculates a guess of what the next player input choice will be.
    public static Actiontype calculateGuessedChoice(Queue<Actiontype> actionlog)
    {
        ActionChance odds = calculateNextPick(actionlog);
        float choice = Random.Range(0, 1);
        if(choice <= odds.Attack)
        {
            return Actiontype.Attacking;
        }
        else if(choice <= odds.Attack + odds.Block)
        {
            return Actiontype.Blocking;
        }
        else //must be > A and B so G
        {
            return Actiontype.Grabbing;
        }
    }
}


public class ActionChance
{
    public ActionChance()
    {
        this.Attack = 0;
        this.Block = 0;
        this.Throw = 0;
    }

    public ActionChance(float a, float b, float g)
    {
        this.Attack = a;
        this.Block = b;
        this.Throw = g;
    }

    public float Attack = 0.33f;
    public float Block = 0.33f;
    public float Throw = 0.33f;

    public void Normalize()    //Assumes values not added
    {
        float total = Attack + Block + Throw;
        if (total == 0) return;
        Attack = Attack / total;
        Block = Block / total;
        Throw = Throw / total;
        total = 1;
    }
}

/// All N_GRAM logic
/// 
