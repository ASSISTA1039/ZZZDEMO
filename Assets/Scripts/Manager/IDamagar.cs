using UnityEngine;

//�ӿڣ������ܻ������˺�����Ѫ�������ܻ��������ܻ�λ��
public interface IDamagar
{
    //�˺�ֵ�����˶����������ߡ��ܻ���Ч���ܻ���Ч
    void TakeDamager(float damagar, string hitAnimationName, Transform attacker, AudioClip audioClip, GameObject gameObject);
    void TakeDamager_NoSound(float damagar, string hitAnimationName, Transform attacker);
}
