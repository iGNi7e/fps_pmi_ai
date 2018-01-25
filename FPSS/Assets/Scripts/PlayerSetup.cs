using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;
    Camera sceneCam;

    [SerializeField]
    string remoteLayerName = "RemotePlayer"; //Слой других игроков

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer(); //Присваивание другим игрокам слоя "RemotePlayer"
        }
        else
        {
            sceneCam = Camera.main;
            if (sceneCam != null)
                sceneCam.gameObject.SetActive(false); //Выключение MainCamera(камера с видом на сцену)
        }
        GetComponent<Player>().Setup();
    }

    public override void OnStartClient() //Вызов при подключении к хосту
    {
        base.OnStartClient();
        string _netID = GetComponent<NetworkIdentity>().netId.ToString(); //Берем ID из NetworkIdentity(при спавне игрока)
        Player _player = GetComponent<Player>(); //Берем скрипт Player(компонент) из префаба

        GameManager.RegisterPlayer(_netID,_player);
    }

    void OnDisable()
    {
        if (sceneCam != null)
        {
            sceneCam.gameObject.SetActive(true);
        }

        GameManager.UnRegisterPlayer(transform.name); 
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
            componentsToDisable[i].enabled = false;
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }
}
