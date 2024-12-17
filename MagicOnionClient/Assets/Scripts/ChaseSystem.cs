using UnityEngine;

public class SimpleChaseAndEscape : MonoBehaviour
{
    public Transform runner;  // ������L�����N�^�[��Transform
    public float chaseSpeed = 5.0f;  // �S�̒ǐՑ��x
    public float escapeSpeed = 4.0f;  // ������L�����N�^�[�̓������x
    public float attackDistance = 1.5f;  // �U���ł��鋗��
    public Camera mainCamera;  // ���C���J����

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        // ������]��180�x�ݒ�iZ������ɉ�]�j
        transform.rotation = Quaternion.Euler(0, 180, 0);  // �S�͍ŏ���180�x��]���Ă�����
    }

    void Update()
    {
        // �S�Ɠ�����L�����N�^�[�̋������v�Z
        float distanceToRunner = Vector3.Distance(transform.position, runner.position);

        // �S��������L�����N�^�[�̔w����ǂ�������
        ChaseRunner();

        // ������L�����N�^�[���S���瓦����
        EscapeFromChaser();

        // �U���͈͓��ł���΍U��
        if (distanceToRunner <= attackDistance)
        {
            animator.SetInteger("State", 1);  // �U���A�j���[�V����
        }
        else
        {
            animator.SetInteger("State", 0);  // ��~�A�j���[�V����
        }

        // �J�����̊O�ɏo�Ȃ��悤�Ɉʒu�𐧌�
        RestrictPositionToCameraView();
    }

    // �S��������L�����N�^�[�̔w����ǂ�������
    void ChaseRunner()
    {
        // �S���i�ޕ������v�Z�i������L�����N�^�[�̈ʒu�Ɍ������Đi�ށj
        Vector3 direction = runner.position - transform.position;  // ������L�����N�^�[�Ɍ������Đi�ޕ���
        direction.y = 0;  // �����𖳎����āAX-Z���ʂŒǂ�������

        // �ǂ�����������Ɉړ�
        transform.position = Vector3.MoveTowards(transform.position, runner.position, chaseSpeed * Time.deltaTime);

        // ������L�����N�^�[�̔w���̕����Ɍ�������
        if (direction.magnitude > 0.1f)  // �ړ�����������L���ȏꍇ
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);  // �S��������L�����N�^�[�Ɍ�����
        }
    }

    // ������L�����N�^�[���S���瓦����
    void EscapeFromChaser()
    {
        // �S�̔��Ε����ɓ�����
        Vector3 direction = (runner.position - transform.position).normalized;  // �S�̔��Ε���
        runner.position += direction * escapeSpeed * Time.deltaTime;

        // ������L�����N�^�[�����Ε����Ɍ���
        runner.forward = -direction;  // ������L�����N�^�[�͋S�̔��Ε���������
    }

    // �J�����̎��E���ɃL�����N�^�[�����邩���`�F�b�N���A�͈͊O�ɏo�Ȃ��悤�Ɉʒu�𐧌�
    void RestrictPositionToCameraView()
    {
        // �J�����͈̔͂��v�Z�i�X�N���[�����W�ɕϊ��j
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // ��ʂ͈̔͊O�ɏo�Ȃ��悤�ɐ���
        if (screenPos.x < 0)
        {
            screenPos.x = 0;
        }
        if (screenPos.x > screenWidth)
        {
            screenPos.x = screenWidth;
        }
        if (screenPos.y < 0)
        {
            screenPos.y = 0;
        }
        if (screenPos.y > screenHeight)
        {
            screenPos.y = screenHeight;
        }

        // ��ʂ͈͓̔��ɖ߂�
        transform.position = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, mainCamera.WorldToScreenPoint(transform.position).z));
    }
}
