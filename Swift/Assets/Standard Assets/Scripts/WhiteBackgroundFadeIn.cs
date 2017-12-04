using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBackgroundFadeIn : MonoBehaviour {

    public bool fadeIn = false;
    public float fadeTime = 0.5f;

    private YieldInstruction fadeInstruction = new YieldInstruction();

    public IEnumerator FadeIn(SpriteRenderer renderer)
    {
        float elapsedTime = 0f;
        Color c = renderer.color;
        while(elapsedTime < fadeTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsedTime / fadeTime);
            renderer.color = c;
        }
        GameObject.Find("MainControl").GetComponent<MainControl>().showMenu2();
    }
}
