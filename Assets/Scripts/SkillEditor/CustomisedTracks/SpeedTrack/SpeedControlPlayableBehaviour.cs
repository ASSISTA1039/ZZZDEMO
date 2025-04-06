using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Animancer;
using Assista.FSM;

namespace Assista.SkillEditor
{
    public class SpeedControlPlayableBehaviour : PlayableBehaviour
    {
        public PlayableDirector director; // ���� PlayableDirector ������
        public float speed = 1f;          // Ŀ���ٶ�

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            // ÿ֡��鲢�����ٶ�
            UpdateSpeed();
        }

        /// <summary>
        /// ���� PlayableGraph ���ٶ�
        /// </summary>
        private void UpdateSpeed()
        {
            if (director != null)
            {
                PlayableGraph graph = director.playableGraph;
                // ȷ�� PlayableGraph ����
                if (graph.IsValid() && graph.GetRootPlayable(0).IsValid())
                {
                    //Debug.Log(graph.GetRootPlayable(0));
                    graph.GetRootPlayable(0).SetSpeed(speed);
                    // ����ͬ�� Animancer ���ٶ�
                    var animancer = director.GetComponent<AnimancerComponent>();
                    if (animancer != null)
                    {
                        animancer.Playable.Speed = speed;
                    }
                }
            }
        }
    }
}