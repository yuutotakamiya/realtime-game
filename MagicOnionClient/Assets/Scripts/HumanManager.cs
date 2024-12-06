using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanManager : Character
{
    //[SerializeField] GameDirector gameDirector;
    //public bool isAttack = false;
    // Start is called before the first frame update
    //Character character;
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("weapon"))
        {
            Destroy(this.gameObject);
        }
    }
}

