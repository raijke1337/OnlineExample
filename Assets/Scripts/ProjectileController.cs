using System.Collections;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField, Range (1f,10f)]
    private float _speed = 3f;
    [SerializeField, Range(0.01f, 10f)]
    private float _damage = 1f;
    [SerializeField, Range(1f, 60f),Tooltip("Seconds")]
    private float _expiry = 7f;

    Collider _col;

    public float GetDamage => _damage;

    private void Start()
    {
        StartCoroutine(OnExpiry());
        _col = GetComponent<Collider>();
    }

    private void Update()
    {
        transform.position += transform.up * _speed * Time.deltaTime;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(_col.bounds.center, _col.bounds.size);
    }
#endif

    private IEnumerator OnExpiry()
    {
        yield return new WaitForSeconds(_expiry);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() == null 
            && collision.gameObject.GetComponent<ProjectileController>() == null)
        Destroy(gameObject);
    }
    
}
