using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameTextManger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);

        // �J�����̏㉺���]��h�����߁AY���̉�]�݂̂�K�p
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, eulerRotation.y, 0f);
    }
}
