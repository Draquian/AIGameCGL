using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LLMUnity;

public class LLMComunication : MonoBehaviour
{
    LLM llm;
    string prompt;

    string replay;

    // Start is called before the first frame update
    void Start()
    {
        prompt = "make some joke in spanish";

        llm = GetComponent<LLM>();
        llm.prompt = prompt;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) SendMSGToLLM();
        if (Input.GetKeyDown(KeyCode.Q)) llm.CancelRequests();
    }

    void SendMSGToLLM()
    {
        Debug.Log("Sending");
        string message = "Hello bot!";
        _ = llm.Chat(message + prompt, GeneratedText, Replay, false);
    }

    void GeneratedText(string msg)
    {
        replay = msg;
        Debug.Log("Generating" + msg);
    }

    void Replay()
    {
        Debug.Log(replay);
    }
}
