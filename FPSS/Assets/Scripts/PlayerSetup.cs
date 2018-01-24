using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;
    Camera sceneCam;

    void Start()
    {
        if (!isLocalPlayer)
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
                componentsToDisable[i].enabled = false;
        }
        else
        {
            sceneCam = Camera.main;
            if (sceneCam != null)
                sceneCam.gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        if (sceneCam != null)
        {
            sceneCam.gameObject.SetActive(true);
        }
    }
}
