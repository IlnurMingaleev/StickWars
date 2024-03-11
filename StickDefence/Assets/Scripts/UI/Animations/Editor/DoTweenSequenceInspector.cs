using DG.DOTweenEditor;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace UI.Animations.Editor
{
    [CustomEditor(typeof(DoTweenSequence))]
    public class DoTweenSequenceInspector : UnityEditor.Editor
    {
        private bool _isPlaying;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(_isPlaying);
            if (GUILayout.Button("Play") && !_isPlaying)
            {
                _isPlaying = true;
                DoTweenSequence s = (DoTweenSequence) target;
                float delay = 0;
                DOTweenEditorPreview.Stop(true);
                foreach (var anim in s.Animations)
                {
                    anim.CreateTween();
                    Tween t = anim.tween;
                    t.SetDelay(delay);
                    delay += t.Duration();
                    delay += t.Delay();
                    DOTweenEditorPreview.PrepareTweenForPreview(t);
                }
                DOTweenEditorPreview.Start();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!_isPlaying);
            if (GUILayout.Button("Stop") && _isPlaying)
            {
                DOTweenEditorPreview.Stop(true);
                _isPlaying = false;
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }
    }
}