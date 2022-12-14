using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class ObiRopeCreator : MonoBehaviour
{
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] float _thickness=1;

    [SerializeField] ObiRopeBlueprint _blueprint;
    [SerializeField] ObiRope _obiRope;
    [SerializeField] ObiSolver _obiSolver;

    private Vector3[] _startControlPoints;
    private void Awake()
    {
        _blueprint.path.Clear();
    }
    private void OnEnable()
    {
        _lineRenderer.GetPositions(_startControlPoints=new Vector3[_lineRenderer.positionCount]);
        _blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
        _blueprint.resolution = 1f;
        FillBrluprint();
        var rope = gameObject.AddComponent<ObiRope>();
        var ropeRenderer = gameObject.AddComponent<ObiRopeExtrudedRenderer>();
        var attachment1 = gameObject.AddComponent<ObiParticleAttachment>();
        var cursor = gameObject.AddComponent<ObiRopeCursor>();
        var lenghtController = gameObject.AddComponent<RopeLenghtController>();
        // Load the default rope section for rendering:
        ropeRenderer.section = Resources.Load<ObiRopeSection>("DefaultRopeSection");

        // Set the blueprint:
        rope.ropeBlueprint = _blueprint;
        
        transform.SetParent(_obiSolver.transform);

    }

    private void FillBrluprint()
    {
        _blueprint.path.Clear();

        int filter = ObiUtils.MakeFilter(ObiUtils.CollideWithEverything, 0);

        _blueprint.path.AddControlPoint(_startControlPoints[0],( _startControlPoints[_startControlPoints.Length - 1]- _startControlPoints[0]).normalized,( _startControlPoints[1]- _startControlPoints[0]).normalized, Vector3.forward, 0.1f, 0.4f, _thickness, filter, Color.blue, "point 0");

        for (int i = 1; i< _startControlPoints.Length-2; i++) 
        {
            _blueprint.path.AddControlPoint(_startControlPoints[i], (_startControlPoints[i - 1]- _startControlPoints[i]).normalized, (_startControlPoints[i + 1]- _startControlPoints[i]).normalized, Vector3.forward, 0.1f, 0.4f, _thickness, filter, Color.blue, "point " + i);
        }

        _blueprint.path.AddControlPoint(_startControlPoints[_startControlPoints.Length - 1],( _startControlPoints[_startControlPoints.Length - 2]-_startControlPoints[_startControlPoints.Length - 1]).normalized, (_startControlPoints[0]- _startControlPoints[_startControlPoints.Length - 1]).normalized, Vector3.forward, 0.1f, 0.4f, _thickness, filter, Color.blue, "point last");
      
        _blueprint.path.FlushEvents();
        _blueprint.path.Closed = true;
        _blueprint.GenerateImmediate();
    }
}
