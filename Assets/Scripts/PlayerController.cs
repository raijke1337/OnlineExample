using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPunObservable
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
    [Space, SerializeField, Range(1f, 10f),Tooltip("Multiplier to move with graviuty ebnabled")]
    private float _msMult = 10f;
    [SerializeField, Range(0.5f, 5f)]
    private float _maxSpeed = 4f;
    [SerializeField, Range(0.5f, 10f)]
    private float _turnRate = 4f;

    [Range(1f, 20f)]
    public float _health = 5f;

    //[SerializeField]
    //public bool _isGreen;
    // replaced with photonview implementation
    // private bool _isGreen;
    // because p1 prefab is green
    // REMOVED ENTIRELY

    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Transform _barrel;
    [Space, SerializeField, Range(0.1f, 5f)]
    private float _atkDelay = 0.5f;

    [SerializeField]
    private PhotonView _photonView;
    public PhotonView GetPhoton() => _photonView;

    private CameraController _camera;

    public event EventHandler<PlayerController> ZeroHealthEvent;

    private ParticlesCOntroller _particles;


    private void Start()
    {
        _controls = new Controls();
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();

        FindObjectOfType<GameManager>().AddPlayer(this);

        var bar = gameObject.GetComponentInChildren<HealthBarScript>();
        if (bar != null) bar.SetUp(this);

        _particles = GetComponent<ParticlesCOntroller>();

        if (!_photonView.IsMine) return;

        _camera = FindObjectOfType<CameraController>();
        _camera.Target = transform;



    }

    private void FixedUpdate()
    {
        // we need this to run only for the owner of playercontroller

        if (!_photonView.IsMine) return;
        // tldr: mine means this user ran this and instantiated the controller
        //var GO = PhotonNetwork.Instantiate(_playerPrefabName + PhotonNetwork.NickName, pos, new Quaternion());

        var dir =  _controls.p1.Movement.ReadValue<Vector2>();

        if (dir.x == 0 && dir.y == 0) return;
        var velocity = _rb.velocity;
        velocity += new Vector3(dir.x, 0, dir.y) * Time.deltaTime * _moveSpeed * _msMult;

        //velocity.y = 0f;
        // to allow climbing on ramps
        velocity = Vector3.ClampMagnitude(velocity, _maxSpeed);
        _rb.velocity = velocity;


    }

    private void OnTriggerEnter(Collider other)
    {
#if UNITY_EDITOR
        Debugger.PrintLog($"Entered trigger: {other.name} at time {Time.time}");
#endif
        if (other.GetComponent<LavaTrigger>() != null)
        {
            _health = 0f;
        }
        // this event is used by gamemanager
        if (_health <= 0f) ZeroHealthEvent?.Invoke(null, this);
    }

    private IEnumerator Shooting()
    {
        while (true)
        {
            var bul = PhotonNetwork.Instantiate("Bullet", _barrel.position, _barrel.rotation);
            bul.transform.SetParent(GameManager.instance.BulletsPool);
            _particles.MuzzleFlash();
            yield return new WaitForSeconds(_atkDelay);
        }
    }
    private IEnumerator Aiming()
    {
        while (true)
        {
            // looks awful in practice
            //transform.LookAt(_target);
            //transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0);
            //yield return new WaitForSeconds(_rotateDelay);
            Vector3 relative = _target.position - transform.position;
            Quaternion desired = Quaternion.LookRotation(relative);
            transform.rotation = Quaternion.Slerp(transform.rotation,desired,Time.deltaTime * _turnRate);

            yield return null;
        }
    }

    //  allows us to send any data
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // works for host only
        // the creator through photon
        if (stream.IsWriting)
        {
            // this custom struct can't be serialized by default Photon!
            // we need a csutom serializer
            stream.SendNext(PlayerData.Create(this));
        }
        // works for guest
        else
        {
            ((PlayerData)stream.ReceiveNext()).Set(this);
        }
    }

    public void SetTarget(Transform player)
    {
        _target = player;
        if (!_photonView.IsMine) return;
        /// when p1 joins he doesnt do anything until theres an enemy
        _controls.p1.Enable();
        StartCoroutine(Shooting());
        StartCoroutine(Aiming());
    }

    public void SessionOver()
    {
        _controls.Disable();
        StopAllCoroutines();
    }

    private void OnCollisionEnter(Collision collision)
    {
        var bul = collision.gameObject.GetComponent<ProjectileController>();
        if (bul != null)
        {
            _health -= bul.GetDamage;

            Debugger.PrintLog($"Ouch! Took {bul.GetDamage} dmg!");

            Destroy(bul.gameObject);
            if (_health <= 0f) ZeroHealthEvent?.Invoke(null, this);
        }      
    }
}
