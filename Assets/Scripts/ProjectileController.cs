using System.Collections;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField, Range (1f,10f)]
    private float _speed = 3f;
    [SerializeField, Range(1f, 10f)]
    private float _damage = 1f;
    [SerializeField, Range(1f, 60f),Tooltip("Seconds")]
    private float _expiry = 7f;

    private Collider _collider;


    public float GetDamage => _damage;

    private void Start()
    {
        StartCoroutine(OnExpiry());
    }

    private void Update()
    {
        transform.position += transform.up * _speed * Time.deltaTime;
    }


    private IEnumerator OnExpiry()
    {
        yield return new WaitForSeconds(_expiry);
        Destroy(gameObject);
    }


}
