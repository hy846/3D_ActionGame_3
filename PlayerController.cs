using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int attackDamage = 10;
    public int maxHealth = 100;
    public int currentHealth;
    public float speed = 5f;
    public float timeToDestroy = 2f;
    public Slider slider;
    public GameObject swordObject;
    public GameObject bowObject;
    public GameObject shieldObject; 
    private Collider swordCollider;
    private Collider bowCollider;
    private Collider shieldCollider;
    private Transform cameraTransform;
    private Animator animator;
    private bool isAttacking;
    public bool isGuarding = false;
    private bool isDead = false;
    private bool isUsingSword = true;

    public GameObject arrowPrefab; // 矢のプレハブ
    public Transform arrowSpawnPoint; // 矢の生成位置
    public Camera mainCamera; // メインカメラ
    public float aimFov = 30f; // エイム時の視野角
    public float defaultFov = 60f; // デフォルトの視野角
    private bool isAiming = false;

    private void Start()
    {
        currentHealth = maxHealth;
        cameraTransform = Camera.main.transform;
        slider.value = 1;
        animator = GetComponent<Animator>();
        swordCollider = swordObject.GetComponent<Collider>();
        bowCollider = bowObject.GetComponent<Collider>(); 
        shieldCollider = shieldObject.GetComponent<Collider>(); 
        swordObject.SetActive(true);
        bowObject.SetActive(false);
        swordCollider.enabled = false;
        bowCollider.enabled = false;
        shieldCollider.enabled = false;
        mainCamera.fieldOfView = defaultFov;
    }

    void FixedUpdate()
    {
        // 攻撃アニメーション中かどうかを確認
        bool isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");

        if (Input.GetKeyDown(KeyCode.Alpha1) && !isAttacking && !isGuarding)
        {
            swordObject.SetActive(true);
            bowObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && !isAttacking && !isGuarding)
        {
            swordObject.SetActive(false);
            bowObject.SetActive(true);
        }

        // 攻撃アニメーション中ならば剣のコライダーを有効にする
        if (isUsingSword && isAttacking)
        {
            swordCollider.enabled = true;
        }
        else // 攻撃アニメーション中でなければ剣のコライダーを無効にする
        {
            swordCollider.enabled = false;
        }

        if (!isAttacking && !isGuarding)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

            if (movement.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                transform.position += moveDirection.normalized * speed * Time.fixedDeltaTime;
            }

            if (horizontalInput != 0 || verticalInput != 0)
            {
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isAttacking && swordObject.activeSelf)
        {
            animator.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.E) && !isAttacking)
        {
            animator.SetBool("isGuarding", true);
            isGuarding = true;
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            animator.SetBool("isGuarding", false);
            isGuarding = false;
        }

        // 右クリックでエイム開始
        if (Input.GetMouseButtonDown(1) && bowObject.activeSelf)
        {
            isAiming = true;
            mainCamera.fieldOfView = aimFov;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
            mainCamera.fieldOfView = defaultFov;
        }


        // 左クリックで矢を放つ
        if (Input.GetMouseButtonUp(0) && isAiming && bowObject.activeSelf && Time.timeScale > 0)
        {
            animator.SetTrigger("Shot");

            // 矢を生成
            GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);

            // カメラの向きに矢を発射
            Vector3 direction = mainCamera.transform.forward;
            arrow.GetComponent<Rigidbody>().AddForce(direction * 1000f);
        }
    }

    // ダメージを受けた時に呼び出される関数
    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        if (isGuarding)
        {
            return;
        }
        else
        {
            currentHealth -= damage;
            Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);
            slider.value = (float)currentHealth / (float)maxHealth;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
            else
            {
                animator.SetTrigger("Hit");
            }
        }
    }

    // 死亡した時に呼び出される関数
    void Die()
    {
        isDead = true;
        animator.SetBool("Dead", true);
        StartCoroutine(DestroyAfterDelay(timeToDestroy));
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            animator.SetTrigger("Gather");
            currentHealth = maxHealth;  // HPを全回復
            slider.value = 1;
            Destroy(other.gameObject);  // 回復アイテムを消去
        }
    }
}