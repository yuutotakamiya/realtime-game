using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestPoint : MonoBehaviour
{
    [SerializeField] GameDirector gameDirector;
    [SerializeField] Character character;
    [SerializeField] DefenceTarget defenceTarget;
    [SerializeField] RoomHubModel roomHubModel;
    public async void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Chest"))
        {
            /*WinText.SetActive(true);
            WinText2.SetActive(true);
            gameDirector.StopCoroutine("CountdownTimer");
            character.Isstart = false;
            defenceTarget.move_speed = 0;
            Invoke("LoadResult", 3.0f);*/

            /*if ()
            {

            }*/

            await gameDirector.GainChest();//•ó” Žæ“¾“¯Šú

            defenceTarget.currentMoveMode = MoveMode.Idle;
            gameDirector.characterList[roomHubModel.ConnectionId].GetComponent<HumanManager>().DropTreasure();
        }
    }

    public void LoadResult()
    {
        Initiate.Fade("Result", Color.black, 1);
    }
}
