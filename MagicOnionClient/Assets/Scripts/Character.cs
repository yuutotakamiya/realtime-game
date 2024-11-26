using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] float speed;

    public bool isself = false;

    [SerializeField] float rotationSpeed; // ��]���x

    private Vector3 movement; // �ړ��x�N�g��



    // Start is called before the first frame update
    void Start()
    {
        




    }

    // Update is called once per frame
    void Update()
    {


        movement = Vector3.zero; // ���t���[���ړ��x�N�g�������Z�b�g

        if (!isself) return;

        //�E�Ɉړ�
        if (Input.GetKey(KeyCode.D))
        {
            //transform.position += transform.right * speed * Time.deltaTime;
            //this.transform.Rotate(0, 1f, 0);
            movement += Vector3.right; // X���̐�����
        } 
         //���Ɉړ�
        if (Input.GetKey(KeyCode.A))
        {
            //transform.position -= transform.right * speed * Time.deltaTime;
            //this.transform.Rotate(0, -1f, 0);
            movement += Vector3.left; // X���̕�����
        }
        //�O�Ɉړ�    
        if (Input.GetKey(KeyCode.W))
        {
            //transform.position += transform.forward * speed * Time.deltaTime;
            movement += Vector3.forward; // Z���̐�����

        }
        //���Ɉړ�   
        if (Input.GetKey(KeyCode.S))
        {
            //transform.position -= transform.forward * speed * Time.deltaTime;
            movement += Vector3.back; // Z���̕�����
        }


        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            // �X���[�Y�ɉ�]������
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // �I�u�W�F�N�g���ړ�������
        transform.Translate(movement.normalized * speed * Time.deltaTime, Space.World);
    }
}
