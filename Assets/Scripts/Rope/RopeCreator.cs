using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RopeMinikit;

public class RopeCreator : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Material _material;
    [SerializeField] private Rigidbody _cube;

    private Rope _rope;
    private RopeConnection _ropeConnection;

    private void Update()
    {
        if (_rope != null)
        {

        Debug.Log(_rope.GetCurrentLength());
        }
    }
    public void CreateRope()
    {
        _rope = Instantiate(new GameObject(), transform).AddComponent<Rope>();
        _ropeConnection = _rope.gameObject.AddComponent<RopeConnection>();
        //_rope.simulation.lengthMultiplier = 10f;
        for (int i = 0; i < _lineRenderer.positionCount - 1; i++)
        {
            _rope.spawnPoints.Add(_lineRenderer.GetPosition(i));
        }

        _rope.isLoop = true;
        _rope.radius = 0.5f;
        _rope.material = _material;
        _rope.simulation.gravityMultiplier = 0;
        _rope.simulation.gravity = new Vector3(0, 0, 0);
        _rope.collisions.enabled = true;
        _rope.simulation.massPerMeter = 1;
        

        _ropeConnection.type = RopeConnectionType.TwoWayCouplingBetweenRigidbodyAndRope;
        _ropeConnection.autoFindRopeLocation = true;
        _ropeConnection.rigidbodySettings.body = _cube;
        _ropeConnection.transformSettings.transform = _cube.transform;
    }
}
