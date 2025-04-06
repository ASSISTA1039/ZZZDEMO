using System.Collections;
using System.Collections.Generic;
using UnityEngine.Timeline;
using Animancer;
using UnityEngine;
using UnityEngine.Playables;


namespace Assista.SkillEditor
{
    [System.Serializable]
    public class NewAnimationPlayableAsset :PlayableAsset, ITimelineClipAsset
    {
        NewAnimationPlayableBehaviour playableBehaviour = new NewAnimationPlayableBehaviour();

        //public ExposedReference<AnimancerComponent> _Animancer;

        public ClipTransition _AnimationClip;
        //���ù����clip�ĳ���
        public override double duration => (_AnimationClip.Clip == null) ? 2f : _AnimationClip.Clip.length;



        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending; }
    
        }

        //public ClipCaps clipCaps => throw new System.NotImplementedException();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var scriptPlayable = ScriptPlayable<NewAnimationPlayableBehaviour>.Create(graph, playableBehaviour);

            NewAnimationPlayableBehaviour playable = scriptPlayable.GetBehaviour();
            //��ȡ��AnimancerComponent���
            playable._Animancer = go.GetComponent<AnimancerComponent>();

            //playable._Animancer = _Animancer.Resolve(graph.GetResolver());

            playable._AnimationClip = _AnimationClip;

            return scriptPlayable;
        }




    
    }
}

