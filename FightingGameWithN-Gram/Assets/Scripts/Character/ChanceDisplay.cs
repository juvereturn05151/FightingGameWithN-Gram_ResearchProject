using UnityEngine;
using TMPro;

public class ChanceDisplay : MonoBehaviour
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
            ActionChance chances = new ActionChance();
            chances = N_Gram.calculateNextPick(target_character.actionLog);
            //display
            LinkedText.text += "Attack : " + (int)(chances.Attack*100) + "%\n";
            LinkedText.text += "Block : " + (int)(chances.Block*100) + "%\n";
            LinkedText.text += "Throw : " + (int)(chances.Throw*100) + "%\n";
        }
    }

    public void Init()
    {
    }
}