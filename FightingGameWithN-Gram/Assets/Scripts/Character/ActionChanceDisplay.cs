using UnityEngine;
using TMPro;

public class ActionChanceDisplay : MonoBehaviour
{

    [SerializeField] private Character target_character;
    public TextMeshProUGUI LinkedText;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (target_character)
        {
            LinkedText.text = "";
            foreach (Actiontype action in target_character.actionLog)
            {
                //display
                switch (action) {
                    case (Actiontype.Attacking):
                        LinkedText.text += "A -> ";
                        break;
                    case (Actiontype.Blocking):
                        LinkedText.text += "B -> ";
                        break;
                    case (Actiontype.Grabbing):
                        LinkedText.text += "G -> ";
                        break;
                    default:
                        break;

                }
                
            }
        }
    }

    public void Init()
    {
    }
}