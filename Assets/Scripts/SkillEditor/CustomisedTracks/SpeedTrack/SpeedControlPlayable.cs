using UnityEngine.Playables;
using UnityEngine;
using UnityEngine.Timeline;
using Assista.FSM;

namespace Assista.SkillEditor
{
    [System.Serializable]
    public class SpeedControlPlayable : PlayableAsset
    {
        public float playbackSpeed = 1f; // �Զ��岥���ٶ�

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            // ���� ScriptPlayable
            var playable = ScriptPlayable<SpeedControlPlayableBehaviour>.Create(graph);

            // ������Ϊ����
            var behaviour = playable.GetBehaviour();
            behaviour.speed = playbackSpeed;
            behaviour.director = owner.GetComponent<PlayableDirector>();
            //Debug.Log(behaviour.director);
            return playable;
        }
    }
}