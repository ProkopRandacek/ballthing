using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BallController : MonoBehaviour
{
    public int     lives = 30;
    public bool    editor;
    public Vector2 checkpointPosition;
    public Vector3 startPosition;
    
    public ArrowController  arrow;
    public ParticleSystem   damagePS;
    public ParticleSystem   respawnPS;
    public Light2D          light2d;
    public SpriteRenderer   sr;
    public CameraShake      shake;
    public CircleCollider2D collider;
 
    private bool             _dragging;
    private bool             _slowdown;
    private bool             _center;
    private bool             _dead;
    private float ?          _respawnTime;
    private Vector2          _velocity;
    private Vector2          _direction;
    

    private Rigidbody2D _rb;
    private Camera      _camera;


    private void Awake()
    {
        _camera       = Camera.main;
        _rb           = gameObject.GetComponent<Rigidbody2D>();
        shake         = GameObject.Find("Cam").GetComponent<CameraShake>();
        startPosition = transform.position;
        collider      = gameObject.GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        _rb.isKinematic = true;
    }

    private void Update()
    {
        if (transform.position.y < -4.0f)
            Die();
        if (_respawnTime != null)
        {
            light2d.enabled = true;
            if (_respawnTime > 0)
                _respawnTime -= Time.deltaTime;

            if (_respawnTime < 0)
            {
                Respawn();
                _respawnTime = null;
            }
            else if (_respawnTime < 1)
            {
                light2d.pointLightOuterRadius += Time.deltaTime * 3;
                PreRespawn();
            }
        }
        if (_slowdown)
        {
            _rb.velocity = _rb.velocity * (1 - (3.5f * Time.deltaTime));
            if (_rb.velocity.magnitude < 0.1f)
            {
                _rb.velocity = Vector2.zero;
                _slowdown    = false;
                _center      = true;
                StartCoroutine(FadeColor(light2d.gameObject, Color.cyan, 0.3f));
            }
        } 
        else if (_center)
        {
            transform.position = Vector2.SmoothDamp(transform.position, checkpointPosition, ref _velocity, 0.3f);
            if (Vector2.Distance(transform.position, checkpointPosition) < 0.001f)
            {
                _center = false;
                Freeze();
                Show();
            }
        }
        if (_dragging && !_slowdown && !_center)
        {
            Vector3 worldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position      = transform.position;

            if (Vector2.Distance(transform.position, worldPosition) > (lives / 10.0f))
            {
                Vector2 dir = worldPosition - position;
                dir = dir.normalized * (lives / 10.0f);
                worldPosition = (Vector2)position + dir;
            }
            
            arrow.MoveArrow(worldPosition, position);
            _direction = position - worldPosition;
        }
    }
    
    private void Hide()
    {
        arrow.gameObject.SetActive(false);
        sr.enabled                    = false;
        light2d.enabled               = false;
        light2d.pointLightOuterRadius = 0;
    }
    
    private void Show()
    {
        arrow.gameObject.SetActive(true);
        Vector3 position = transform.position;
        arrow.MoveArrow(position, position + Vector3.up); // position arrow
        sr.enabled = true;
        light2d.enabled = true;
        light2d.pointLightOuterRadius = lives / 10.0f;
    }

    public void Freeze()
    {
        _rb.velocity        = Vector2.zero;
        _rb.isKinematic     = true;
        _rb.freezeRotation  = true;
        _rb.gravityScale    = 0;
    }

    private void Unfreeze()
    {
        _rb.freezeRotation  = false;
        _rb.isKinematic     = false;
        collider.isTrigger = false;
        _rb.gravityScale    = 1;
        _rb.rotation        = 0;
    }

    private void Shoot()
    {
        arrow.gameObject.SetActive(false);
        _rb.velocity = _direction * 3;
        Unfreeze();
        StartCoroutine(FadeColor(light2d.gameObject, Color.yellow, 0.3f));
    }
    
    private void OnMouseDown()
    {
        _dragging = true;
    }

    private void OnMouseUp()
    {
        if (editor) return;
        _dragging = false;
        Shoot();
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (editor) return;
        if (lives > 3)
        {
            damagePS.Emit(3);
            lives -= 3;

            light2d.pointLightOuterRadius = lives / 10.0f;
        }
        else
        {
            damagePS.Emit(lives);
            Die();
        }
    }

    public void HitCheckpoint()
    {
        _rb.gravityScale = 0;
        _slowdown        = true;
    }

    public void HitGoal()
    {
        _rb.gravityScale = 0;
        _slowdown        = true;
        Debug.Log("WIN");
    }

    public void Die()
    {
        if (_dead)
            return;
        
        damagePS.Emit(lives);
        shake.TriggerShake();
        lives        = 0;
        _slowdown    = false;
        _center      = false;
        _dead        = true;
        _respawnTime = 3.0f;
        
        Freeze();
        Hide();
    }

    private void PreRespawn()
    {
        if (!_dead)
            return;
        transform.position = startPosition;

        _dead = false;
        lives = 30;
        respawnPS.Emit(lives);
    }

    private void Respawn()
    {
        Show();
        Freeze();
        _dragging = false;

        light2d.pointLightOuterRadius = 3;
    }

    public void ResetFlags()
    {
        _dragging     = false;
        _slowdown     = false;
        _center       = false;
        _dead         = false;
        _respawnTime  = null;
        _velocity     = Vector2.zero;
        _direction    = Vector2.zero;
        lives         = 30;
        startPosition = transform.position;
        Show();
        Freeze();
        light2d.pointLightOuterRadius = 3;

        checkpointPosition = transform.position;
    }

    private IEnumerator FadeColor(GameObject objectToFade, Color newColor, float fadeTime = 3)
    {
        Light2D l = objectToFade.GetComponent<Light2D>();
        Color currentColor = l.color;
        float counter = 0;

        while (counter < fadeTime)
        {
            counter += Time.deltaTime;
            l.color = Color.Lerp(currentColor, newColor, counter / fadeTime);
            yield return null;
        }
    }
}