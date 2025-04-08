using UnityEngine;
using System.Collections.Generic;

public class ActionChance
{
    public ActionChance()
    {
        this.A = 1;
        this.B = 1;
        this.G = 1;
    }

    public ActionChance(float a, float b, float g)
    {
        this.A = a;
        this.B = b;
        this.G = g;
    }
    
    public float A = 0.33f;
    public float B = 0.33f;
    public float G = 0.33f;
}

public class N_Gram : MonoBehaviour
{
    ActionChance calculateNextPick(Queue<Actiontype> actionlog)
    {
        
        for(int i = 0; i<actionlog.Count; i++){
            
        }
        ActionChance chances = new ActionChance();
        return chances;
    }

        //Calculates a guess of what the next player input choice will be.
    Actiontype calculateGuessedChoice(Queue<Actiontype> actionlog)
    {
        ActionChance odds = calculateNextPick(actionlog);
        float choice = Random.Range(0, 1);
        if(choice <= odds.A)
        {
            return Actiontype.Attacking;
        }
        else if(choice <= odds.A + odds.B)
        {
            return Actiontype.Blocking;
        }
        else //must be > A and B so G
        {
            return Actiontype.Grabbing;
        }
    } 
}


/// All N_GRAM logic
/// 
