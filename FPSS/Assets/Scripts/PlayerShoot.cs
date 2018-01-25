using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{
    public PlayerWeapon weapon;

    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask; // Поверхности по которым можно стрелять

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot: cam == null");
            this.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    [Client] // Команды только для клиента
    void Shoot()
    {
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position,cam.transform.forward,out _hit,weapon.range,mask))
        {
           if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShoot(_hit.collider.name, weapon.damage);
            }
        }
    }

    [Command] // Только для сервера
    void CmdPlayerShoot(string _playerID, int _damage)
    {
        Debug.Log(_playerID + " has been shoot");
        Player _player = GameManager.GetPlayer(_playerID); // Берём из всех игроков на карте того, в кого попали лучом
        _player.RpcTakeDamage(_damage);
    }
}
