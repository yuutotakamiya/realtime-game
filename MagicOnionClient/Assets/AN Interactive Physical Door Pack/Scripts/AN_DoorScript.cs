using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AN_DoorScript : MonoBehaviour
{

    public bool Locked = false;
    
    public bool Remote = false;
    public bool CanOpen = true;
    
    public bool CanClose = true;
    public bool RedLocked = false;
    public bool BlueLocked = false;
   
    AN_HeroInteractive HeroInteractive;
    
    public bool isOpened = false;//ドアが開いてるかどうか
 
    [SerializeField]float OpenSpeed;

    // NearView()
    float distance;
    float angleView;
    Vector3 direction;

    // Hinge
    [HideInInspector]
    public Rigidbody rbDoor;
   [SerializeField] HingeJoint hinge;
   [SerializeField] JointLimits hingeLim;
   [SerializeField] float currentLim;

   [SerializeField] GameDirector gameDirecter;

    void Start()
    {
        rbDoor = GetComponent<Rigidbody>();
        hinge = GetComponent<HingeJoint>();
        hingeLim = hinge.limits;

        // ドアが最初に閉じている状態にする
        currentLim = 0f;

        // Unityエディターで変更した内容を無視する
        hingeLim.max = 500f;  // 開く最大角度を設定
        hingeLim.min = -90f; // 閉じる最小角度を設定
        hinge.limits = hingeLim;

       //gameDirecter = GameObject.Find("GameDirector").GetComponent<GameDirector>();

    }

    void Update()
    {
        if ( !Remote && Input.GetKeyDown(KeyCode.E) && NearView() )
            Action();
        
    }

    public void Action()
    {
        // ドアを開けるアクション
        isOpened = !isOpened; // 状態を反転
        if (isOpened)
        {
            rbDoor.AddRelativeTorque(new Vector3(0, 0, 50f)); // ドアを開ける力
        }
    }


    public bool NearView() // it is true if you near interactive object
   {
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        if (distance < 3f) // 近づいているかどうかだけチェック
        { 
            gameDirecter.openButton.gameObject.SetActive(true);
            return true;
        }
        return false;
    }

    void FixedUpdate()
    {
        // ドアが開いているか閉じているかの状態を管理
        if (isOpened)
        {
            currentLim = 500f; // ドアを完全に開ける
        }
        else
        {
            if (currentLim > 0f)
                currentLim -= OpenSpeed * Time.deltaTime; // 開くスピードを調整
        }

        // ドアが開いている状態で角度を設定
        hingeLim.max = currentLim;
        hingeLim.min = -currentLim;
        hinge.limits = hingeLim;
    }
}
