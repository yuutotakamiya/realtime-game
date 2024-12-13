using UnityEngine;

public class AN_Button : MonoBehaviour
{
    [Tooltip("True for rotation like valve (used for ramp/elevator only)")]
    public bool isValve = false;
    [Tooltip("SelfRotation speed of valve")]
    public float ValveSpeed = 10f;
    [Tooltip("If it isn't valve, it can be lever or button (animated)")]
    public bool isLever = false;
    [Tooltip("If it is false door can't be used")]
    public bool Locked = false;
    [Space]
    [Tooltip("Any object for ramp/elevator behavior")]
    public Transform RampObject;
    [Tooltip("True for rotation by X local rotation by valve")]
    public bool xRotation = true;
    [Tooltip("True for vertical movement by valve (if xRotation is false)")]
    public bool yPosition = false;
    public float max = 90f, min = 0f, speed = 5f;

    bool valveBool = true;
    bool isOpened = false;
    float current, startYPosition;
    Quaternion startQuat, rampQuat;

    Animator anim;

    // NearView()
    float distance;
    float angleView;
    Vector3 direction;

    void Start()
    {
        anim = GetComponent<Animator>();
        startYPosition = RampObject.position.y;
        startQuat = transform.rotation;
        rampQuat = RampObject.rotation;
    }

    void Update()
    {
        if (!Locked)
        {
            // バルブ処理
            if (isValve && RampObject != null && Input.GetKey(KeyCode.E) && NearView())
            {
                if (valveBool)
                {
                    if (!isOpened && current < max) current += speed * Time.deltaTime;
                    if (isOpened && current > min) current -= speed * Time.deltaTime;

                    if (current >= max)
                    {
                        isOpened = true;
                        valveBool = false;
                    }
                    else if (current <= min)
                    {
                        isOpened = false;
                        valveBool = false;
                    }
                }
                else
                {
                    if (!isOpened && current > min) current -= speed * Time.deltaTime;
                    if (isOpened && current < max) current += speed * Time.deltaTime;
                    valveBool = true;
                }

                // バルブの回転処理
                transform.rotation = startQuat * Quaternion.Euler(0f, 0f, current * ValveSpeed);
                if (xRotation) RampObject.rotation = rampQuat * Quaternion.Euler(current, 0f, 0f);
                else if (yPosition) RampObject.position = new Vector3(RampObject.position.x, startYPosition + current, RampObject.position.z);
            }
        }
    }

    bool NearView() // 近くにいるときに true
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        direction = transform.position - Camera.main.transform.position;
        angleView = Vector3.Angle(Camera.main.transform.forward, direction);
        if (angleView < 45f && distance < 2f) return true;
        else return false;
    }
}
