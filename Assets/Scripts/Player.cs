using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public int curHP;
    public int maxHP;
    public float moveSpeed;
    public int damage;
    public float interactRange;
    public List<string> inventory = new List<string>();

    private Vector2 facingDirection;

    [Header("Combat")]
    public KeyCode attackKey;
    public float attackRange;
    public float attackRate;
    
    private float lastAttackTime;

    [Header("Experience")]
    public int curLevel;
    public int curXP;
    public int xpToNextLevel;
    public float levelXpModifier;

    [Header("Sprites")]
    public Sprite downSprite;
    public Sprite upSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    //components
    private Rigidbody2D rig;
    private SpriteRenderer sr;
    private ParticleSystem hitEffect;
    private PlayerUI ui;

    void Awake()
    {
        //get the components
        rig = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        hitEffect = gameObject.GetComponentInChildren<ParticleSystem>();
        ui = FindObjectOfType<PlayerUI>();
    }

    void Start()
    {
        //initialize the UI elements
        ui.UpdateHealthBar();
        ui.UpdateXpBar();
        ui.UpdateLevelText();
    }

    void Update()
    {
        Move();

        if(Input.GetKeyDown(attackKey))
        {
            if (Time.time - lastAttackTime >= attackRate)
            {
                Attack();
            }
        }

        CheckInteract();
    }

    void CheckInteract()
    {
        RaycastHit2D interactor = Physics2D.Raycast(transform.position, facingDirection, interactRange, 1 << 9);

        if (interactor.collider != null)
        {
            Interactable interactable = interactor.collider.GetComponent<Interactable>();
            
            //Debug.Log("Text gets set");
            ui.SetInteractText(interactor.collider.transform.position, interactable.interactDescription);

            if (Input.GetKeyDown(attackKey))
            {
                //Debug.Log("Interaction");
                interactable.Interact();
            }
        }
        else
        {
            //Debug.Log("Text gets disabled");
            ui.DisableInteractText();
        }
        
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDirection, attackRange, 1 << 8);

        if(hit.collider != null)
        {
            hit.collider.GetComponent<Enemy>().TakeDamage(damage);

            //play the hit effect
            hitEffect.transform.position = hit.collider.transform.position;
            hitEffect.Play();
        }
    }

    void Move()
    {
        //get the horizontal and vertical keyboard inputs
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        //calculate the velocity
        Vector2 vel = new Vector2(x,y);

        //calculate the facing direction
        if (vel.magnitude != 0)
        {
            facingDirection = vel;
        }

        UpdateSpriteDirection();

        //set the velocity
        rig.velocity = vel * moveSpeed;
    }

    void UpdateSpriteDirection()
    {
        if(facingDirection == Vector2.up)
        {
            sr.sprite = upSprite;
        }
        else if(facingDirection == Vector2.down)
        {
            sr.sprite = downSprite;
        }
        else if(facingDirection == Vector2.left)
        {
            sr.sprite = leftSprite;
        }
        else if(facingDirection == Vector2.right)
        {
            sr.sprite = rightSprite;
        }
    }

    public void TakeDamage(int damageTaken)
    {
        curHP -= damageTaken;

        ui.UpdateHealthBar();

        if (curHP <= 0)
            Die();
    }

    void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void AddXp(int xp)
    {
        curXP += xp;

        ui.UpdateXpBar();

        if (curXP >= xpToNextLevel)
        LevelUp();
    }

    void LevelUp()
    {
        curXP -= xpToNextLevel;
        curLevel++;

        xpToNextLevel = Mathf.RoundToInt((float)xpToNextLevel * levelXpModifier);

        ui.UpdateLevelText();
        ui.UpdateXpBar();
    }

    public void AddItemToInventory(string item)
    {
        inventory.Add(item);
        ui.UpdateInventoryText();
    }
}
