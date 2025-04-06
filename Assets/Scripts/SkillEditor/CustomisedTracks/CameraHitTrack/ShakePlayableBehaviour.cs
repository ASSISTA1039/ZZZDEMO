using UnityEngine;
using UnityEngine.Playables;

public class ShakePlayableBehaviour : PlayableBehaviour
{
    public float shakeForce;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // ��ȡ�󶨵� CameraHitFeel �ű�ʵ��
        var cameraHitFeel = playerData as CameraHitFeel;
        if (cameraHitFeel != null)
        {
            Debug.Log("111");
            cameraHitFeel.CameraShake(shakeForce);
        }
    }
}
