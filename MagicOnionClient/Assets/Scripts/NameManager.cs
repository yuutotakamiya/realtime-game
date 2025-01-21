using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameManager : MonoBehaviour
{
    [SerializeField] Text NameText;
    // Start is called before the first frame update
    void Start()
    {
        if(NameText == null)
        {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Name(string Name)
    {
        NameText.text = Name;
    }
}
