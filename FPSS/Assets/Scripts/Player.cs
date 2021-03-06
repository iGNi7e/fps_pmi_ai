﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    [SerializeField]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = false; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHelth;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;

    private bool cursorOn = true;

    public void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            //Switch cameras
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadCastNewPlayerSetup();
    } 

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;
        }

        SetDefaults();
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetupPlayer();
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.K))
            RpcTakeDamage(1009);

        if (Input.GetKeyDown(KeyCode.L))
            RpcTakeDamage(20);

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!cursorOn)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            cursorOn = !cursorOn;
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(int _amount)
    {
        if (_isDead) return;

        currentHelth -= _amount;
        Debug.Log(transform.name + " now has " + currentHelth + " health");

        if (currentHelth <= 0 && !_isDead)
        {
            Die();
        }
    }

    void Die()
    {
        _isDead = true;

        //Disable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        //Disable GameObjects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        //Disable the collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;

        //Spawn DeathEffect
        GameObject _gfxInst = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxInst, 3f);

        //Switch cameras
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        Debug.Log(transform.name + " Die!");
        StartCoroutine(Respawn());
    }

    public void SetDefaults()
    {
        _isDead = false;

        currentHelth = maxHealth;

        //Set the components active
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //Set the GameObjects active
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        //Set the collider active
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;

        //Create a spawn effect
        GameObject _gfxInst = Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxInst, 3f);
    }

    public float GetHealthPct()
    {
        return (float)currentHelth / maxHealth;
    }
}
