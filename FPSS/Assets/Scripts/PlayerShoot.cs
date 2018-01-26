using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{

    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask; // Поверхности по которым можно стрелять

    [SerializeField]
    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot: cam == null");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
    }

    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();
         
        if (currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot",0f,1f / currentWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }

    [Client] // Команды только для клиента
    void Shoot()
    {
        Debug.Log("SHOOT!!!1!11!");
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position,cam.transform.forward,out _hit,currentWeapon.range,mask))
        {
           if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShoot(_hit.collider.name,currentWeapon.damage);
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
