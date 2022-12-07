using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    
    public int maxHealth = 5;
    public int RobotsFixed = 0;
    public GameObject projectilePrefab;
    public int ammo { get { return currentAmmo; }}
    public int currentAmmo = 5;

    public Text ammoText;
    
    public int health { get { return currentHealth; }}
    int currentHealth;
    public GameObject Damage;
    public GameObject WinText;
    public GameObject LoseText;
    public Text FixedRobots;
    public float timeInvincible = 2.0f;
    bool isInvincible;
    public bool hasBoots;
    public bool hasShield;
    float invincibleTimer;

    public AudioSource DamageAudio;
    public AudioClip NormalHit;
    public AudioClip ShieldHit;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip backgroundSound;

    AudioSource audioSource;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        AmmoText();
        currentHealth = maxHealth;
        audioSource.clip = backgroundSound;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        FixedRobots.text= $"Robots Fixed {RobotsFixed}/5";
        if(RobotsFixed==5){
            WinText.SetActive(true);
            audioSource.clip = backgroundSound;
            audioSource.Stop();
            audioSource.clip = winSound;
            audioSource.Play();
        }
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        
        if(Input.GetKeyDown(KeyCode.C))
        {
            Launch();
            
            if (currentAmmo > 0)
            {
                ChangeAmmo(-1);
                AmmoText();
            }
        }
        
        if(Input.GetKey(KeyCode.LeftShift)&& hasBoots)
        {
            speed=4.5f;
        }
        if(!Input.GetKey(KeyCode.LeftShift)&& hasBoots)
        {
            speed=3f;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
            
        }
    }
    public void ChangeAmmo(int amount)
    {
        // Ammo math code
        currentAmmo = Mathf.Abs(currentAmmo + amount);
        Debug.Log("Ammo: " + currentAmmo);
    }

    public void AmmoText()
    {
        ammoText.text = "Ammo: " + currentAmmo.ToString();
    }
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            if (hasShield)
            {
                hasShield=false;
                DamageAudio.clip=ShieldHit;
                DamageAudio.Play();
                return;
            }
            Damage.GetComponent<ParticleSystem>().Play();
                DamageAudio.clip=NormalHit;
                DamageAudio.Play();
            isInvincible = true;
            invincibleTimer = timeInvincible;
        }
        if (currentHealth <= 0)
        {
            LoseText.SetActive(true);

            audioSource.clip = backgroundSound;
             audioSource.Stop();

            audioSource.clip = loseSound;
            audioSource.Play();
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }
    
    void Launch()
    {
       if (currentAmmo > 0) // If player has ammo, they can launch cogs
        {
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(lookDirection, 300);

            animator.SetTrigger("Launch");

        }
    }
}
