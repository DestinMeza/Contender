using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScreenDamageEffect : MonoBehaviour
{
    public float duration = 0.25f;
    Image flashEffectImage;
    public Color color1;
    public Color color2;
    void Awake(){
        flashEffectImage = GetComponent<Image>();
    }
    void Start(){
        PlayerController.player.GetComponent<HealthController>().onHealthDecrease += Effect;
    }

    void Effect(){
        StartCoroutine(DamageFlash());
    }

    IEnumerator DamageFlash(){
        for(float t = 0; t < duration; t += Time.deltaTime){
            flashEffectImage.color = Color.Lerp(color1, color2, t/duration);
            yield return new WaitForEndOfFrame();
        }
        for(float t = 0; t < duration; t += Time.deltaTime){
            flashEffectImage.color = Color.Lerp(color2, color1, t/duration);
            yield return new WaitForEndOfFrame();
        }
    }
}
