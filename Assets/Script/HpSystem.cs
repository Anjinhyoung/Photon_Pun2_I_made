using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpSystem : MonoBehaviour
{
   
    public static HpSystem hpSystem;

    // UI �����̴�(�÷��̾� ü��)
    [SerializeField] Slider hp_Var;

    // var
    [SerializeField] Image var;

    private void Awake()
    {
        if(hpSystem == null)
        {
            hpSystem = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Damage(int damage)
    {
        hp_Var.value -= damage;

        if(hp_Var.value == 0)
        {
            Destroy(var);
        }

    }
}
