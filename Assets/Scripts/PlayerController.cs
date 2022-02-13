using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Controls _controls;
    [SerializeField]
    private Rigidbody _rb;
    [SerializeField]
    private CapsuleCollider _collider;
    [SerializeField]
    private ProjectileController _projectilePrefab;

    [Space,SerializeField,Range(1f,6f)]
    private float _moveSpeed = 2f;
    [SerializeField, Range(0.5f, 5f)]
    private float _maxSpeed = 4f;
    [SerializeField, Range(0.5f, 5f)]
    private float _rotateDelay = 0.25f;
    [Range(1f, 20f)]
    public float _health = 5f;

    [SerializeField]
    public bool isP1;

    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Transform _barrel;
    [Space, SerializeField, Range(0.1f, 5f)]
    private float _atkDelay = 0.5f;

    private Transform _bulletsPool;


    private void Start()
    {
        _controls = new Controls();
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        if (isP1) _controls.p1.Enable();
        else _controls.p2.Enable();

        _bulletsPool = FindObjectOfType<GameManager>().transform;

        StartCoroutine(Shooting());
        StartCoroutine(Aiming());
    }

    private void OnDisable()
    {
        if (isP1) _controls.p1.Disable();
        else _controls.p2.Disable();
    }

    private void FixedUpdate()
    {
        var dir = isP1 ?
            _controls.p1.Movement.ReadValue<Vector2>() : 
            _controls.p2.Movement.ReadValue<Vector2>() ;

        if (dir.x == 0 && dir.y == 0) return;
        var velocity = _rb.velocity;
        velocity += new Vector3(dir.x, 0, dir.y) * Time.deltaTime * _moveSpeed;

        velocity.y = 0f;
        velocity = Vector3.ClampMagnitude(velocity, _maxSpeed);
        _rb.velocity = velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        var bul = other.GetComponent<ProjectileController>();
        if (bul == null) return;
        _health -= bul.GetDamage;
        Destroy(other.gameObject);
        if (_health <= 0f) Debug.Log($"{name} died");
    }

    private IEnumerator Shooting()
    {
        while (true)
        {
            var bul = Instantiate(_projectilePrefab,_barrel.position,_barrel.rotation, _bulletsPool);
            yield return new WaitForSeconds(_atkDelay);
        }
    }
    private IEnumerator Aiming()
    {
        while (true)
        {
            transform.LookAt(_target);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            yield return new WaitForSeconds(_rotateDelay);
        }
    }

}
