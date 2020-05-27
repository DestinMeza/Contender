using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [TextArea]
    public string[] dialogs;
    [TextArea]
    public string victoryMessage;
    public float textScrollInterval = 0.1f;
    public float closeTextDelay = 5;
    public GameObject dialogArea;
    Queue<IEnumerator> dialogQueue = new Queue<IEnumerator>();
    public Text dialogBoxText;
    public Text characterNameText;
    
    void Awake(){
        DialogTriggerController.onTriggerSet += GetTrigger;
        StopAllCoroutines();
        BossHealthController.onBossDeath += VictoryMessage;
    }


    void OnEnable(){
        StartCoroutine(DialogQueue());
    }

    void GetTrigger(int i, string characterName){
        dialogQueue.Enqueue(ReadDialog(i, characterName));
    }

    void VictoryMessage(){
        dialogQueue.Enqueue(ReadDialog(victoryMessage, "Captain Saunder"));
    }

    IEnumerator DialogQueue(){
        while(enabled){
            if(dialogQueue.Count > 0){

                dialogArea.SetActive(true);
                while (dialogQueue.Count > 0){
                    IEnumerator coroutine = dialogQueue.Dequeue();
                    yield return StartCoroutine(coroutine);
                }
                dialogArea.SetActive(false);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ReadDialog(int i, string characterName){
    string sentence = "";

        foreach (char letter in dialogs[i])
        {
            AudioManager.Play("TypingSound" + Random.Range(1,3));
            sentence += letter;
            characterNameText.text = characterName;
            dialogBoxText.text = string.Format("{0}", sentence);
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(textScrollInterval);
        }
        yield return new WaitForSeconds(closeTextDelay);
    }

    IEnumerator ReadDialog(string dialog, string characterName){
    string sentence = "";

        foreach(char letter in dialog)
        {
            AudioManager.Play("TypingSound" + Random.Range(1,3));
            sentence += letter;
            characterNameText.text = characterName;
            dialogBoxText.text = string.Format("{0}", sentence);
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(textScrollInterval);
        }
        yield return new WaitForSeconds(closeTextDelay);
    }
}
