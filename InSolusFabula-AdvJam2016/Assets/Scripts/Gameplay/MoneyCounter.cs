using UnityEngine;
using System.Collections;

public class MoneyCounter : MonoBehaviour
{
    public TextComponent counter;
    private decimal money;

    void Start()
    {
        StartCoroutine(RunCounter());
    }

    IEnumerator RunCounter()
    {
        yield return 1;
        while (this.enabled)
        {
            if (money != Global.Money)
            {
                money = Global.Money;
                yield return counter.SetText("$ " + Global.Money.ToString("0.00"));
            }
            yield return 1;
        }
    }
}
