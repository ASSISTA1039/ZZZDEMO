using System.Collections.Generic;
using UnityEngine;

public class SphereManager : MonoBehaviour
{
    public GameObject spherePrefab; // ����Ԥ����
    public Transform spawnParent; // ����ĸ��ڵ�
    public int maxSpheres = 3; // ���������ڵ���������


    [SerializeField, Header("���˼��")] private float detectionRang;
    [SerializeField] protected LayerMask enemyLayer;
    //�̶�����BOSS
    [SerializeField] public Collider[] BOSSTarget;

    private ObjectPool spherePool;
    public Queue<GameObject> activeSpheres = new Queue<GameObject>();

    void Start()
    {
        // ��ʼ�������
        spherePool = new ObjectPool(spherePrefab, maxSpheres, spawnParent);
    }
    private void Update()
    {
        foreach (var sphere in activeSpheres)
        {
            if (sphere.activeInHierarchy) // ȷ�������Ǽ���״̬
            {
                int targetCount = Physics.OverlapSphereNonAlloc(gameObject.transform.position, detectionRang, BOSSTarget, enemyLayer);
                if (targetCount > 0)
                {

                }
            }
        }

    }
    public void SpawnSphere(Vector3 position)
    {
        // ������������ﵽ���ޣ��������������
        if (activeSpheres.Count >= maxSpheres)
        {
            var oldSphere = activeSpheres.Dequeue();
            spherePool.ReturnToPool(oldSphere);
        }

        // �Ӷ���ػ�ȡ������
        var newSphere = spherePool.Get();
        newSphere.transform.position = position;
        activeSpheres.Enqueue(newSphere);
    }
}
