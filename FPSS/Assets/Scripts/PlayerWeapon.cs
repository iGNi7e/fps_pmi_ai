﻿using UnityEngine;

//Свойства оружия
[System.Serializable]
public class PlayerWeapon
{
    public string name = "Glock";

    public int damage = 10;
    public float range = 100f;
    public float fireRate = 10f;

    public GameObject graphics;
}
