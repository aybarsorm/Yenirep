using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    // platforms
    public Transform[] friendlySpawnPoints;
    public Transform[] hostileSpawnPoints;
    //unit prefabs
    public GameObject[] friendlyPrefabs;
    public GameObject[] hostilePrefabs;
    

    // unit parameters
    Unit playersUnit;
    Unit attackTarget;
    Unit SelectedEnemy;
    Unit[] turnOrder;
    Unit[] friendlyUnits = new Unit[4];
    Unit[] hostileUnits = new Unit[4];

    public BattleHud[] friendlyHuds;

    //sys parameters
    int turnOrderIndex = 0;
    int currentSelectedEnemyIndex = 0;
    int randomNumber;
    bool canPlayerTakeAction = false;
    public Text dialogueText;
    public BattleState state;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    private void Update()
    {
        if (Input.GetKeyDown("right"))
            SettleArrowKey("right");
        else if (Input.GetKeyDown("left"))
            SettleArrowKey("left");

    }

    IEnumerator SetupBattle()
    {
        for (int i = 0; i < friendlyPrefabs.Length; i++)
        {
            GameObject friendly = Instantiate(friendlyPrefabs[i], friendlySpawnPoints[i]);
            friendly.transform.parent = friendlySpawnPoints[i].transform;
            friendly.GetComponent<Unit>().SetHud(friendlyHuds[i]);
            Instantiate(hostilePrefabs[i], hostileSpawnPoints[i]).transform.parent =
                hostileSpawnPoints[i].transform;

        }
        turnOrder = FindObjectsOfType<Unit>();

        SetTurnOrder();
        SetUnitArrays();

        for(int i = 0; i < friendlyHuds.Length; i++)
        {
            friendlyHuds[i].SetHud(friendlyUnits[i]);
        }

        SelectedEnemy = hostileUnits[currentSelectedEnemyIndex];
        SelectedEnemy.transform.parent.
                    Find("SelectedHostile").gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        NextTurn();
    }

    public void PlayerTurn()
    {
        dialogueText.text = "It is " + turnOrder[turnOrderIndex] + "'s turn ";
        canPlayerTakeAction = true;
    }

    public void OnAttackButton()
    {
        if(state == BattleState.PLAYERTURN && canPlayerTakeAction)
        {
            canPlayerTakeAction = false;
            StartCoroutine(PlayerAttack());
        }
    }

    IEnumerator PlayerAttack()
    {
        playersUnit = turnOrder[turnOrderIndex];
        if(!DidDodge(SelectedEnemy))
        {
            bool isAlive = SelectedEnemy.TakeDamage(playersUnit.GetDamage());
            dialogueText.text = SelectedEnemy.GetUnitName() + " Took " + playersUnit.GetDamage() + " points of damage" + "new enemy hp = " + SelectedEnemy.GetCurrentHP();
            yield return new WaitForSeconds(3f);

            if (!isAlive)
            {
                SelectedEnemy.gameObject.SetActive(false);
                if (IsTeamDead("hostiles"))
                {
                    state = BattleState.WON;
                    StartCoroutine(BattleWon());
                }
                else
                {
                    dialogueText.text = SelectedEnemy.GetUnitName() + " is dead!";
                    yield return new WaitForSeconds(2f);
                    SettleArrowKey("right");
                    turnOrderIndex++;
                    NextTurn();
                }
            }
            turnOrderIndex++;
            NextTurn();
        }
        else
        {
            dialogueText.text = SelectedEnemy.GetUnitName() + " dodged your attack!";
            yield return new WaitForSeconds(2f);
            turnOrderIndex++;
            NextTurn();
        }
    }
    IEnumerator EnemyTurn()
    {
        Unit hostileUnit = turnOrder[turnOrderIndex];
        int hostileDmg = hostileUnit.GetDamage();
        dialogueText.text = hostileUnit.GetUnitName() + " Attacks!";
        yield return new WaitForSeconds(2f);
        SelectAttackTarget();
        dialogueText.text = hostileUnit.GetUnitName() + " Attacks to " + attackTarget.GetUnitName() + "!";

        if(!DidDodge(attackTarget))
        {
            bool isAlive = attackTarget.TakeDamage(hostileDmg);
            attackTarget.GetHud().SetHP(attackTarget);
            dialogueText.text = hostileUnit.GetUnitName() + "Deals " + hostileDmg + "points of damage to " + attackTarget.GetUnitName() + "!";
            yield return new WaitForSeconds(2f);

            if (!isAlive)
            {
                attackTarget.gameObject.SetActive(false);
                if (IsTeamDead("friendlies"))
                {
                    state = BattleState.LOST;
                    StartCoroutine(BattleLost());
                }
                else
                {
                    dialogueText.text = attackTarget.GetUnitName() + " is unconscious!";
                    yield return new WaitForSeconds(2f);
                    turnOrderIndex++;
                    NextTurn();
                }
            }
            turnOrderIndex++;
            NextTurn();
        }
        else
        {
            dialogueText.text = attackTarget.GetUnitName() + " dodged the attack!";
            Debug.Log("random number = " + randomNumber + "dodge = " + attackTarget.GetCurrentDodge());
            yield return new WaitForSeconds(2f);
            turnOrderIndex++;
            NextTurn();
        }
    }

    IEnumerator BattleWon()
    {
        dialogueText.text = "Battle Won!";
        yield return new WaitForSeconds(2f);
    }

    IEnumerator BattleLost()
    {
        dialogueText.text = "BattleLost!";
        yield return new WaitForSeconds(2f);
    }

    public bool DidDodge(Unit unit)
    {
        randomNumber = Random.Range(1, 101);
        if (randomNumber > unit.GetCurrentDodge() || randomNumber >= 95)
            return false;
        else 
            return true;
    }

    private void SelectAttackTarget()
    {
        int randomNumber = Random.Range(0, friendlyUnits.Length);
        attackTarget = friendlyUnits[randomNumber];
        if (!attackTarget.gameObject.activeSelf)
            SelectAttackTarget();
    }

    public void NextTurn()
    {
        if (turnOrderIndex >= turnOrder.Length) // turu baþa sarma olayý
            turnOrderIndex = 0;
        isInTurn();

        if (turnOrder[turnOrderIndex].GetIsFriendly())
        {
            state = BattleState.PLAYERTURN;
            dialogueText.text = "player turn";
            PlayerTurn();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            dialogueText.text = "enemy turn";
            StartCoroutine(EnemyTurn());
        }
    }

    public bool IsTeamDead(string targetTeam)
    {
        if (targetTeam == "friendlies")
        {
            for (int i = 0; i < friendlyUnits.Length; i++)
            {
                if (friendlyUnits[i].GetIsAlive())
                    return false;
            }
            return true;
        }
        else
        {
            for (int i = 0; i < hostileUnits.Length; i++)
            {
                if (hostileUnits[i].GetIsAlive())
                    return false;
            }
            return true;
        }
    }

    public void SettleArrowKey(string key)
    {
        if(key == "right") 
        { 
            Transform prewSelectedEnemy = SelectedEnemy.transform;
            SelectedEnemy = NextEnemy();
            SetSelectedEnemyArrow(prewSelectedEnemy, SelectedEnemy.transform);
        }
        else
        {
            Transform prewSelectedEnemy = SelectedEnemy.transform;
            SelectedEnemy = PrewEnemy();
            SetSelectedEnemyArrow(prewSelectedEnemy, SelectedEnemy.transform);
        }
        
    }

    public void SetSelectedEnemyArrow(Transform prewSelectedEnemy, Transform newSelectedEnemy)
    {
        prewSelectedEnemy.transform.parent.
                    Find("SelectedHostile").gameObject.SetActive(false);
        newSelectedEnemy.transform.parent.
                    Find("SelectedHostile").gameObject.SetActive(true);
    }

    public Unit NextEnemy()
    {
        currentSelectedEnemyIndex++;
        if (currentSelectedEnemyIndex >= hostileUnits.Length)
            currentSelectedEnemyIndex = 0;
        if (!hostileUnits[currentSelectedEnemyIndex].gameObject.activeSelf)
            NextEnemy();
        return hostileUnits[currentSelectedEnemyIndex];
    }

    public Unit PrewEnemy()
    {
        currentSelectedEnemyIndex--;
        if (currentSelectedEnemyIndex < 0)
            currentSelectedEnemyIndex = (hostileUnits.Length - 1);
        if (!hostileUnits[currentSelectedEnemyIndex].gameObject.activeSelf)
            PrewEnemy();
        return hostileUnits[currentSelectedEnemyIndex];
    }

    public void isInTurn()
    {
        if (!turnOrder[turnOrderIndex].gameObject.activeSelf) // if unit isn't active
        {
            turnOrderIndex++;// check next dude
            isInTurn();
        }
    }

    public void SetTurnOrder()// OH NO N^2
    {
        for (int i = 0; i < turnOrder.Length; i++)
        {
            for (int j = 0; j < turnOrder.Length; j++)
            {
                if (turnOrder[i].GetInitiative() > turnOrder[j].GetInitiative())
                {
                    Unit temp = turnOrder[j];
                    turnOrder[j] = turnOrder[i];
                    turnOrder[i] = temp;
                }
            }
        }
    }

    public void SetUnitArrays()
    {
        int friendlyCounter = 0;
        int hostileCounter = 0;
        for (int i = 0; i < turnOrder.Length; i++)
        {

            if (turnOrder[i].isFriendly)
            {
                friendlyUnits[friendlyCounter] = turnOrder[i];
                friendlyCounter++;

            }
            else
            {
                hostileUnits[hostileCounter] = turnOrder[i];
                hostileCounter++;
            }
        }
    }
}
