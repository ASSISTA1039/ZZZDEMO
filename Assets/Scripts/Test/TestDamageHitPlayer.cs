using Assista.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDamageHitPlayer : MonoBehaviour
{
    public Transform pivot;
    // ��ת�ٶ�
    public float rotationSpeed = 50f;

    // �Ƿ�������ת
    private bool isRotating = false;

    public Collider player;
    void Update()
    {
        RotateDoor();
        
    }

    private void RotateDoor()
    {
        // ������Y����ת
        transform.RotateAround(pivot.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �������ҵ���ײ
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("��������巢������ײ��");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // ����뿪��ײ
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("����뿪���������ײ��Χ��");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �������ҵ���ײ
        if (other.CompareTag("Player"))
        {
            Debug.Log("��ҽ�������������");

            other.GetComponent<PlayerStateMachine>().isEnemyAttacked = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ����뿪����
        if (other.CompareTag("Player"))
        {
            Debug.Log("����뿪����������");
            other.GetComponent<PlayerStateMachine>().isEnemyAttacked = false;
        }
    }
}
