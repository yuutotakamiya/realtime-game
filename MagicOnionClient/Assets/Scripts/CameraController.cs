using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target; // �J�������Ǐ]����v���C���[��Transform
    public float distance = 5.0f; // �v���C���[����̋���
    public float height = 2.0f; // �v���C���[�̏�ɂ���J�����̍���
    public float rotationSpeed = 5.0f; // �J�����̉�]���x

    private float horizontalInput; // ���������̃}�E�X����
    private float verticalInput; // ���������̃}�E�X����

    // Start is called before the first frame update
    void Start()
    {
        //�ŏ��̃v���C���[�̈ʒu�̎擾
        //pastPos = player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // �}�E�X�̓��͂��擾
        horizontalInput += Input.GetAxis("Mouse X") * rotationSpeed;
        verticalInput -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // ���������̃J�����p�x�𐧌��i�㉺�̓����𐧌�j
        verticalInput = Mathf.Clamp(verticalInput, -30f, 60f);

        // �J�����̉�]�v�Z
        Quaternion rotation = Quaternion.Euler(verticalInput, horizontalInput, 0);

        // �J�����̈ʒu���v���C���[�̔w��ɐݒ�
        Vector3 position = target.position - (rotation * Vector3.forward * distance) + Vector3.up * height;

        // �J�����̈ʒu�Ɖ�]��ݒ�
        transform.position = position;
        transform.LookAt(target.position + Vector3.up * height);
    }
}
