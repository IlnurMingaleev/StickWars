using UnityEngine;
using UnityEngine.UI;

namespace UI.Helpers
{
    public class UISkewEffect : BaseMeshEffect
    {
        private const int DefaultValue = 10;

        #region Inspector
        [SerializeField] private int _angle;

        public float Angle => _angle;

        #endregion

        #region public methods
        public override void ModifyMesh(VertexHelper vh)
        {
            float tanAngle = Mathf.Tan(_angle * Mathf.Deg2Rad);
            if (!enabled)
                tanAngle = 0;

            for (int i = 0; i < vh.currentVertCount; i++)
            {
                UIVertex vertex = default;
                vh.PopulateUIVertex(ref vertex, i);
                var vertexPosition = vertex.position;
                vertexPosition.x += vertexPosition.y * tanAngle;
                vertex.position = vertexPosition;
                vh.SetUIVertex(vertex, i);
            }
        }
        #endregion

#if UNITY_EDITOR
        protected override void Reset()
        {
            _angle = DefaultValue;
        }
#endif
    }
}