using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestPoint : MonoBehaviour
{
    [SerializeField] GameDirector gameDirector;
    [SerializeField] Character character;
    [SerializeField] GameObject WinText;
    [SerializeField] GameObject WinText2;
    [SerializeField] DefenceTarget defenceTarget;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Chest"))
        {
            WinText.SetActive(true);
            WinText2.SetActive(true);
            gameDirector.StopCoroutine("CountdownTimer");
            character.Isstart = false;
            defenceTarget.move_speed = 0;
            Invoke("LoadResult", 3.0f);
        }
    }

    public void LoadResult()
    {
        Initiate.Fade("Result", Color.black, 1);
    }
}
