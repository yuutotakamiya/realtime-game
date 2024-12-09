using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Title()
    {
        Initiate.Fade("Title",Color.black,1);
    }

    public void Game()
    {
        Initiate.Fade("Game", Color.black, 1);
    }
}
