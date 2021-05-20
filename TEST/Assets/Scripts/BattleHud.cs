using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    public Text nameText;
    public Text HPText;

    public void SetHud(Unit unit)
    {
        unit.GetHud().nameText.text = unit.GetUnitName();
        unit.GetHud().HPText.text = unit.GetCurrentHP() + "/" + unit.GetMaxHP();
    }

    public void SetHP(Unit unit)
    {
        unit.GetHud().HPText.text = unit.GetCurrentHP() + "/" + unit.GetMaxHP();
    }
}
