using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldown : MonoBehaviour
{
    public IEnumerator CoCooldown(float time)
    {
        float timer = time;

        while (timer > 0)
        {
            GetComponent<Image>().fillAmount = timer / time;
            timer -= Time.deltaTime;
            yield return null;
        }

        GetComponent<Image>().fillAmount = 0;
    }
}
