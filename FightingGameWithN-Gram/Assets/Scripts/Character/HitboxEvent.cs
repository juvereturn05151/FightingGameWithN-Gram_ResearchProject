using UnityEngine;

public class HitboxEvent : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
                    character.OnBeingHit();
                }

                Debug.Log("Hit");
            }
        }
    }
}
