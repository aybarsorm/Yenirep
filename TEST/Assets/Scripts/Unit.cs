using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    public int damage;
    public int maxHP;
    public int currentHP;
    public int baseInitiative;
    public bool isFriendly;
    private int currentInitiative;
    BattleSystem battleSystem;
    BattleHud battleHud;

    bool isAlive = true;
    //bool canAct = true;

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0)
        {
            currentHP = 0;
            return isAlive = false;
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
        //battleSystem.SetTurnOrder();
    }
    public int GetInitiative()
    {
        return baseInitiative;
    }
    private void Start()
    {
        currentInitiative = baseInitiative;
        currentHP = maxHP;
        battleSystem = FindObjectOfType<BattleSystem>();

    }

    public void SetHud(BattleHud hud)
    {
        battleHud = hud;
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

    public bool IsFriendly()
    {
        return isFriendly;
    }
}
