using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NGramPredictor : MonoBehaviour
{
    private int nValue;
    private Dictionary<string, ActionChance> actionHistory;
    private Queue<Actiontype> actionSequence;

    public NGramPredictor(int nValue)
    {
        this.nValue = nValue;
        actionHistory = new Dictionary<string, ActionChance>();
        actionSequence = new Queue<Actiontype>();
    }

    public void RecordAction(Actiontype action)
    {
        actionSequence.Enqueue(action);

        // Maintain sequence length
        if (actionSequence.Count > nValue)
        {
            actionSequence.Dequeue();
        }

        // Only record patterns when we have enough data
        if (actionSequence.Count == nValue)
        {
            string sequenceKey = GetSequenceKey();
            Actiontype nextAction = action; // In practice, this would be the following action

            if (!actionHistory.ContainsKey(sequenceKey))
            {
                actionHistory[sequenceKey] = new ActionChance();
            }

            // Update counts for this sequence
            switch (nextAction)
            {
                case Actiontype.Attacking:
                    actionHistory[sequenceKey].A++;
                    break;
                case Actiontype.Blocking:
                    actionHistory[sequenceKey].B++;
                    break;
                case Actiontype.Grabbing:
                    actionHistory[sequenceKey].G++;
                    break;
            }
        }
    }

    public Actiontype PredictNextAction(Queue<Actiontype> recentActions)
    {
        if (recentActions.Count < nValue - 1)
        {
            return GetRandomAction(); // Not enough data
        }

        string currentSequence = GetCurrentSequenceKey(recentActions);

        if (actionHistory.ContainsKey(currentSequence))
        {
            ActionChance chances = actionHistory[currentSequence];
            chances.Normalize();

            // Return the most likely action
            if (chances.A >= chances.B && chances.A >= chances.G)
                return Actiontype.Attacking;
            if (chances.B >= chances.A && chances.B >= chances.G)
                return Actiontype.Blocking;
            return Actiontype.Grabbing;
        }

        return GetRandomAction();
    }

    private string GetSequenceKey()
    {
        return string.Join(",", actionSequence.ToArray());
    }

    private string GetCurrentSequenceKey(Queue<Actiontype> actions)
    {
        return string.Join(",", actions.Take(nValue - 1).ToArray());
    }

    private Actiontype GetRandomAction()
    {
        float rand = Random.Range(0f, 1f);
        if (rand < 0.4f) return Actiontype.Attacking;
        if (rand < 0.7f) return Actiontype.Blocking;
        return Actiontype.Grabbing;
    }
}
