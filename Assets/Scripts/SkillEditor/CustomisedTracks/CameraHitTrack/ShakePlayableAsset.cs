using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class ShakePlayableAsset : PlayableAsset
{
    public float shakeForce;

    // �����ݵ� PlayableBehaviour
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ShakePlayableBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.shakeForce = shakeForce;
        return playable;
    }
}
