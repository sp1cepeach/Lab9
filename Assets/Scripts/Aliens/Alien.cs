using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Alien : MonoBehaviour
{
    public Action OnAlienHit;
    public Action OnAlienReachPlayer;
    public float speed { get; set; }
    public int score { get; set; }
    public int damage { get; set; }
    public SpriteRenderer spriteRenderer { get { return _spriteRenderer; } }
    private SpriteRenderer _spriteRenderer;
    public new Rigidbody2D rigidbody { get { return _rigidbody; } }
    Rigidbody2D _rigidbody;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rigidbody.velocity = Vector2.right * (2 + (2 * speed));
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.tag)
        {
            case "Bullet":
                OnAlienHit?.Invoke();
                Destroy(gameObject);
                break;

            case "BorderSides":
                rigidbody.velocity *= -1;
                rigidbody.position += Vector2.down * 3;
                break;

            case "BorderBottom":
                OnAlienReachPlayer?.Invoke();
                Destroy(gameObject);
                break;
        }
    }
}
