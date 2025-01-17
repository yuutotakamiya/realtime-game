using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseModel : MonoBehaviour
{
#if DEBUG
    public const string ServerURL = "http://localhost:7000";
#else
    public const  string ServerURL = "http://realtimegame.japaneast.cloudapp.azure.com:7000";
#endif
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
