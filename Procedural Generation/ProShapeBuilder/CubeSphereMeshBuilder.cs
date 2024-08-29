using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.ProceduralGeneration.ProShapeBuilder
{
	public class CubeSphereMeshBuilder : CubeMeshBuilder
	{
        [SerializeField, Tooltip("tell what base scale should look like")]
        private BaseScaleUnitOfSolid _baseScaleType;

        #region Public API

        public BaseScaleUnitOfSolid BaseScaleType
        {
            get { return _baseScaleType; }
            set { _baseScaleType = value; }
        }

        #endregion

        protected override void OnBuildTrianglesAndVertices(ref List<Vector3> vertices, ref List<int> triangles)
        {
            base.OnBuildTrianglesAndVertices (ref vertices, ref triangles);

            NormalizeToCenterOfCube(ref vertices);
        }

        private void NormalizeToCenterOfCube(ref List<Vector3> vertices)
        {
            float refUnit = 1;

            if (_baseScaleType == BaseScaleUnitOfSolid.BiggestRadiusHalfSqrtTwo)
                refUnit = Mathf.Sqrt(2);
            if (_baseScaleType == BaseScaleUnitOfSolid.BiggestRadiusHalfSqrtThree)
                refUnit = Mathf.Sqrt(3);
            if (_baseScaleType == BaseScaleUnitOfSolid.BiggestRadiusOneMeter || _baseScaleType == BaseScaleUnitOfSolid.EdgesOneMeter)
                refUnit = 2;
            if (_baseScaleType == BaseScaleUnitOfSolid.EdgesSqrtTwo)
                refUnit = Mathf.Sqrt(2) * 2;
            if (_baseScaleType == BaseScaleUnitOfSolid.EdgesSqrtThree)
                refUnit = Mathf.Sqrt(3) * 2;

            CalculateCenterOfMesh(vertices);

            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 dir = vertices[i] - _relativeCenterPos;
                Vector3 normalizeDir = dir.normalized * ((_scaleFactor / 2) * refUnit);
                Vector3 newVertexPos = normalizeDir + _relativeCenterPos;

                vertices[i] = newVertexPos;
            }
        }

    } 
}
