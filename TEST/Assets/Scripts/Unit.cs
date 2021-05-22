using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Basic Settings")]
    public string unitName;
    public int unitLevel;
    public bool isFriendly;
    private int currentInitiative;
    [Header("Stats")]
    public int damage;
    public int maxHP;
    public int currentHP;
    public int baseInitiative;
    public int baseDodge;
    public int currentDodge;

    BattleSystem battleSystem;
    BattleHud battleHud;

    private bool isAlive = true;
    //bool canAct = true;

    private void Start()
    {
        currentInitiative = baseInitiative;
        currentHP = maxHP;
        currentDodge = baseDodge;
        battleSystem = FindObjectOfType<BattleSystem>();

    }

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0)
        {
            currentHP = 0;
            isAlive = false;
            return isAlive;
        }
        else
            return isAlive;
    }

    public void Heal(int heal)
    {
        currentHP += heal;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    public void ChangeInitiative(int amount)
    {
        currentInitiative += amount;
        battleSystem.SetTurnOrder();
    }
    public void ChangeCurrentDodge(int amount)
    {
        currentDodge += amount;
    }

    // getters
    public int GetInitiative()
    {
        return baseInitiative;
    }
    public string GetUnitName()
    {
        return unitName;
    }
    public int GetCurrentHP()
    {
        if (currentHP > maxHP)
            currentHP = maxHP;
        return currentHP;
    }

    public int GetMaxHP()
    {
        return maxHP;
    }

    public int GetDamage()
    {
        return damage;
    }
    public BattleHud GetHud()
    {
        return battleHud;
    }

    public bool GetIsFriendly()
    {
        return isFriendly;
    }

    public bool GetIsAlive()
    {
        return isAlive;
    }

    public int GetCurrentDodge()
    {
        return currentDodge;
    }
    //setters
    public void SetHud(BattleHud hud)
    {
        battleHud = hud;
    }

    public void SetCurrentHP(int hp)
    {
        currentHP = hp;
    }

}
