using UnityEngine;

public class HitboxEvent : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null) 
        {
            if (collision.CompareTag("Hurtbox"))
            {
                if (collision.GetComponentInParent<Character>() is Character character)
                {
                    if (!character.CharacterAttack.CanHitConfirm) 
                    {
                        character.OnBeingHit();
                    }
                }
            }

        }
    }
}
