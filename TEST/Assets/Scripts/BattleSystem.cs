using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    public Transform[] friendlyPlatforms;
    public Transform[] hostilePlatforms;

    public GameObject[] friendlyPrefabs;
    public GameObject[] hostilePrefabs;

    public Text dialogueText;

    Unit playersUnit;
    Unit attackTarget;
    Unit SelectedEnemy;
    Unit[] turnOrder;
    Unit[] friendlyUnits = new Unit[4];
    Unit[] hostileUnits = new Unit[4];

    public BattleHud[] friendlyHuds;

    int turnOrderIndex = 0;
    int currentSelectedEnemyIndex = 0;
    bool canPlayerTakeAction = false;

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
            GameObject friendly = Instantiate(friendlyPrefabs[i], friendlyPlatforms[i]);
            friendly.transform.parent = friendlyPlatforms[i].transform;
            friendly.GetComponent<Unit>().SetHud(friendlyHuds[i]);
            Instantiate(hostilePrefabs[i], hostilePlatforms[i]).transform.parent =
                hostilePlatforms[i].transform;

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

    public void NextTurn()
    {
        if (turnOrderIndex >= turnOrder.Length) // turu baþa sarma olayý
            turnOrderIndex = 0;
        isInTurn();

        if (turnOrder[turnOrderIndex].IsFriendly())
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

    public void PlayerTurn()
    {
        dialogueText.text = "It is " + turnOrder[turnOrderIndex] + "'s turn ";
        canPlayerTakeAction = true;
    }

    public void OnAttackButton()
    {
        if(state == BattleState.PLAYERTURN && canPlayerTakeAction)
        {
            StartCoroutine(PlayerAttack());
            canPlayerTakeAction = false;

        }
    }

    IEnumerator PlayerAttack()
    {
        playersUnit = turnOrder[turnOrderIndex];
        bool isAlive = SelectedEnemy.TakeDamage(playersUnit.GetDamage());
        dialogueText.text = SelectedEnemy.GetUnitName() + " Took " + playersUnit.GetDamage() + " points of damage" + "new enemy hp = " + SelectedEnemy.GetCurrentHP();
        yield return new WaitForSeconds(3f);

        if (!isAlive)
        {
            SelectedEnemy.gameObject.SetActive(false);
            dialogueText.text = SelectedEnemy.GetUnitName() + " is dead!";
            yield return new WaitForSeconds(2f);
            SettleArrowKey("right");
            NextTurn();
        }else
        {
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
        bool isAlive = attackTarget.TakeDamage(hostileDmg);
        attackTarget.GetHud().SetHP(attackTarget);
        dialogueText.text = hostileUnit.GetUnitName() + "Deals " + hostileDmg + "points of damage to " + attackTarget.GetUnitName() + "!";
        yield return new WaitForSeconds(2f);

        if (!isAlive)
        {
            attackTarget.gameObject.SetActive(false);
            dialogueText.text = attackTarget.GetUnitName() + " is unconscious!";
            yield return new WaitForSeconds(2f);
            NextTurn();
        }
        else
        {
            turnOrderIndex++;
            NextTurn();
        }
    }

    private void SelectAttackTarget()
    {
        int randomNumber = Random.Range(0, friendlyUnits.Length);
        attackTarget = friendlyUnits[randomNumber];
        if (!attackTarget.gameObject.activeSelf)
            SelectAttackTarget();
    }

    public void SetTurnOrder()// OH NO N^2
    {
        for(int i = 0; i < turnOrder.Length; i++)
        {
            for(int j = 0; j < turnOrder.Length; j++)
            {
                if(turnOrder[i].GetInitiative() > turnOrder[j].GetInitiative())
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
            currentSelectedEnemyIndex = (hostileUnits.Length);
        if (!hostileUnits[currentSelectedEnemyIndex].gameObject.activeSelf)
            PrewEnemy();
        return hostileUnits[currentSelectedEnemyIndex];
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

    public void isInTurn()
    {
        if (!turnOrder[turnOrderIndex].gameObject.activeSelf) // if unit isn't active
        {
            turnOrderIndex++;// check next dude
            isInTurn();
        }
    }
}
