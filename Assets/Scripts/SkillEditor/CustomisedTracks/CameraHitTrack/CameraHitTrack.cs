using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Assista.SkillEditor
{
    [TrackColor(229 / 255f, 20 / 255f, 0 / 255f)]
    [TrackClipType(typeof(ShakePlayableAsset))]
    [TrackBindingType(typeof(CameraHitFeel))] // ָ��������
    public class CameraHitTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<ShakePlayableBehaviour>.Create(graph, inputCount);
        }
    }
}
