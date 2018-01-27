using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer"; //Слой других игроков

    [SerializeField]
    string dontDrawLayerName = "DontDraw"; //Слой, который не будет прорисовываться камерой игрока
    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab;

    [HideInInspector]
    public GameObject playerUIInstance;

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer(); //Присваивание другим игрокам слоя "RemotePlayer"
        }
        else
        {
            SetLayerRecursively(playerGraphics,LayerMask.NameToLayer(dontDrawLayerName)); //Выключение графики слоя DontDraw

            playerUIInstance = Instantiate(playerUIPrefab); //Создаём PlayerUI
            playerUIInstance.name = playerUIPrefab.name;
        }
        GetComponent<Player>().Setup();
    }

    void SetLayerRecursively(GameObject obj,LayerMask newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject,newLayer);
        }
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
        Destroy(playerUIInstance);

        GameManager.instance.SetSceneCameraActive(true);

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
