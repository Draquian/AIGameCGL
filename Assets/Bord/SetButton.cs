using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject padre = GameObject.Find("ChaosCards");
        ChaosManager CM = FindObjectOfType<ChaosManager>();

        transform.parent = padre.transform;

        GetComponent<Button>().onClick.AddListener(delegate { CM.SelectedCard(this.gameObject); });
    }
}
