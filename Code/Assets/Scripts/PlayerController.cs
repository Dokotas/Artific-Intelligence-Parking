using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    private Rigidbody _rb;
    private PhotonView _photonView;
    private Vector3 _position, _spawnPosition;

    private bool _isGrounded;
    private int _health = 3;
    private float _axisX, _axisZ;
    public const string PLAYER_NAME = "";

    [SerializeField] private float speed = 5f, _jumpForce = 5f, deathLevel;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();
        _spawnPosition = transform.position;

        if (_photonView.IsMine)
            FindObjectOfType<CameraFollow>().Target = this.transform;
    }

    void FixedUpdate()
    {
        if (_photonView.IsMine)
        {
            _axisX = Input.GetAxis("Horizontal");
            _axisZ = Input.GetAxis("Vertical");
        }

        if (Mathf.Abs(_axisX) > 0 || Mathf.Abs(_axisZ) > 0)
        {
            _rb.velocity = new Vector3(_axisX * speed, _rb.velocity.y, _axisZ * speed);
            transform.rotation = Quaternion.LookRotation(new Vector3(_axisX, 0, _axisZ));
        }

        if (transform.position.y < deathLevel)
            _photonView.RPC("DrowDown", RpcTarget.All);
    }

    private void Update()
    {
        _position = transform.position;
        _isGrounded = Physics.Raycast(_position + Vector3.up * .005f, Vector3.down, .1f);

        if (_photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
                _photonView.RPC("Jump", RpcTarget.All);
        }
    }

    [PunRPC]
    private void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.y, _jumpForce, _rb.velocity.z);
    }

    [PunRPC]
    private void DrowDown()
    {
        transform.position = _spawnPosition;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_axisX);
            stream.SendNext(_axisZ);
        }
        else
        {
            _axisX = (float) stream.ReceiveNext();
            _axisZ = (float) stream.ReceiveNext();
        }
    }
}