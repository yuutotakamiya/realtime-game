using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] float speed;
    public bool isself = false;
    public bool isstart = false;
    FixedJoystick joystick;
    Rigidbody rb;
    [SerializeField]float rotateSpeed = 10f;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
         rb= GetComponent<Rigidbody>();
        joystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
        Collider characterCollider = GetComponent<Collider>();
        characterCollider.material = (PhysicMaterial)Resources.Load("CharacterMaterial");
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isstart == true)
        {
            Vector3 move = (Camera.main.transform.forward * joystick.Vertical +
            Camera.main.transform.right * joystick.Horizontal) * speed;
            move.y = rb.velocity.y;
            rb.velocity = move;
            //i‚Þ•ûŒü‚ÉŠŠ‚ç‚©‚ÉŒü‚­B
            transform.forward = Vector3.Slerp(transform.forward, move, Time.deltaTime * rotateSpeed);
        }
        
        /*if (rb.velocity.magnitude > 0)
        {
            animator.SetBool("Run",true);
        }
        else
        {
            animator.SetBool("Run", false);
        }*/
        
    }
}
