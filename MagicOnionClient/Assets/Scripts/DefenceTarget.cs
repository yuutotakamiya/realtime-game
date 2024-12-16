using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MoveMode
{
    Idle = 1,
    Follow = 2
}
public class DefenceTarget : MonoBehaviour
{
    [SerializeField] GameDirector gameDirector;
    public float move_speed = 3f;
    private bool isHolding;//�����Ă��邩�ǂ���
    public float followDistance = 5f;  // �Ǐ]����ő勗��
    private float hideDistance = 6f;   // �{�^�����\���ɂ���臒l����

    protected Rigidbody rb;
    protected Transform followTarget;
    protected MoveMode currentMoveMode;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        currentMoveMode = MoveMode.Idle;
        isHolding = false;

        // ������ԂŃ{�^�����\��
        gameDirector.holdButton.SetActive(false);
        gameDirector.notholdButton.SetActive(false);
    }

    void Update()
    {
        if (isHolding && followTarget != null)
        {
            Vector3 direction = (followTarget.position - transform.position).normalized;
            rb.MovePosition(transform.position + direction * move_speed * Time.deltaTime);
        }
        CheckDistanceAndUpdateButtons();
    }

    // �v���C���[���߂��ɂ��邩�ǂ������`�F�b�N���ă{�^����\��/��\���ɂ���
    private void CheckDistanceAndUpdateButtons()
    {
        if (followTarget != null)
        {
            float distance = Vector3.Distance(followTarget.position, transform.position);

            if (distance > hideDistance)
            {
                gameDirector.holdButton.SetActive(false);
                gameDirector.notholdButton.SetActive(false);
            }
            else
            {
                UpdateButtonState();
            }
        }
    }

    private void UpdateButtonState()
    {
        if (!isHolding)
        {
            // �܂����������Ă��Ȃ���ԂȂ�u��������v�{�^����\��
            gameDirector.holdButton.SetActive(true);
            gameDirector.notholdButton.SetActive(false);
        }
        else
        {
            // ���������Ă����ԂȂ�u��߂�v�{�^����\��
            gameDirector.holdButton.SetActive(false);
            gameDirector.notholdButton.SetActive(true);
        }
    }


    // �v���C���[���ڐG������
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Human")&&gameDirector.IsEnemy==false)
        {
            followTarget = other.transform;  // �Ǐ]�Ώۂ�ݒ�

            // ������Ԃł́u��������v�{�^����\�����A�u��߂�v�{�^�����\���ɂ���
            gameDirector.holdButton.SetActive(true);
            gameDirector.notholdButton.SetActive(false);

            // �����Ɋ�Â��ă{�^�����X�V
            CheckDistanceAndUpdateButtons();
        }
    }

    // �u��������v�{�^���������ꂽ��
    public void OnHoldButtonPressed()
    {
        isHolding = true;  // �Ǐ]���J�n
        UpdateButtonState();
    }

    // �u��߂�v�{�^���������ꂽ��
    public void OnNotHoldButtonPressed()
    {
        isHolding = false;  // �Ǐ]���~
        UpdateButtonState();
    }
}
