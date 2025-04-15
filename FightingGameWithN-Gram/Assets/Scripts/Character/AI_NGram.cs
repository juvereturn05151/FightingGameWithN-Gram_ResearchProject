using UnityEngine;
using System.Collections.Generic;



public class N_Gram : MonoBehaviour
{
    //Calculate next choice of the opponent;
    public static ActionChance calculateNextPick(Queue<Actiontype> actionlog)
    {
        ActionChance chances = new ActionChance();

        // Safety check: need at least 2 actions to make a prediction
        if (actionlog.Count < 2)
        {
            //Assume chances of all outcomes are equal
            chances.Attack++;
            chances.Block++;
            chances.Throw++;
            chances.Normalize();
            return chances;
        }

        ActionChance[] ActionCount = new ActionChance[]
        {
            new ActionChance(), //Attacking
            new ActionChance(), //Blocking
            new ActionChance()  //Throwing
        };

        Actiontype[] index = actionlog.ToArray();
        for (int i = 0; i < actionlog.Count - 1; i++)
        {
            int currentType = (int)index[i];
            Actiontype nextType = index[i + 1];

            if (currentType < 0 || currentType >= ActionCount.Length) continue;

            switch (nextType)
            {
                case Actiontype.Attacking:
                    ActionCount[currentType].Attack += 1;
                    break;
                case Actiontype.Blocking:
                    ActionCount[currentType].Block += 1;
                    break;
                case Actiontype.Throwing:
                    ActionCount[currentType].Throw += 1;
                    break;
            }
        }


        int lastAction = (int)index[actionlog.Count - 1];
        if (lastAction >= 0 && lastAction < ActionCount.Length)
        {
            chances = ActionCount[lastAction];
        }
        chances.Normalize();

        return chances;
    }

    //Calculates a guess of what the next player input choice will be.
    public static Actiontype calculateGuessedChoice(Queue<Actiontype> actionlog)
    {
        ActionChance odds = calculateNextPick(actionlog);
        float choice = Random.Range(0f, 1f);

        if (choice <= odds.Attack)
        {
            return Actiontype.Attacking;
        }
        else if(choice <= odds.Attack + odds.Block)
        {
            return Actiontype.Blocking;
        }
        else //must be > A and B so G
        {
            return Actiontype.Throwing;
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
