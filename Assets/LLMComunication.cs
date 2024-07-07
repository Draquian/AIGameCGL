using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LLMUnity;
using TMPro;

public class LLMComunication : MonoBehaviour
{
    LLM llm;
    string prompt;

    string replay;

    public string negativeP;
    public string contextP;
    public string startingP;

    // Start is called before the first frame update
    void Start()
    {
        llm = GetComponent<LLM>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) llm.CancelRequests();
    }

    void SendMSGToLLM()
    {
        _ = llm.Chat(contextP + negativeP + startingP + prompt, GeneratedText, Replay, true);
    }

    void GeneratedText(string msg)
    {
        replay = msg;
    }

    void Replay()
    {
        GameObject.Find("EpicWar").GetComponent<TextMeshProUGUI>().text = replay;
    }

    public void GetPrompt(string prompt)
    {
        this.prompt = prompt;
        SendMSGToLLM();
    }
}
