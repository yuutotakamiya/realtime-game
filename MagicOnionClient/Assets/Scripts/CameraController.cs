using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player;   //���j�e�B�����̃I�u�W�F�N�g�����i�[
    private Vector3 offset;      //�J�����Ƃ̑��΋������i�[
    void Start()
    {
        //���j�e�B�����̃I�u�W�F�N�g�����i�[
        this.player = GameObject.Find("wizard(Clone)");
        //���C���J����(���g�̃I�u�W�F�N�g)�ƃ��j�e�B�����ƃg�����X�t�H�[���̑��΋������Z�o
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
        //���C���J�����ɑ��΋����𔽉f�������V�����g�����X�t�H�[���̒l���Z�b�g����
        transform.position = this.player.transform.position + offset;
    }
}
