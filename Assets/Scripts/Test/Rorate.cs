using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rorate : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] float speed;
    [SerializeField] float _angle;
    [SerializeField] float _sign;

    private void Update()
    {
        _angle = Vector3.Angle(Vector3.forward, _target.position- transform.position);
        //_sign = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(transform.position, _target.position)));
        //_angle = _angle * _sign;

        //
    }

    public void Rotate()
    {
        transform.RotateAround( _target.position, Vector3.up, _angle);
        //transform.rotation = Quaternion.EulerAngles(90, 0, transform.rotation.z);
        //transform.rotation = new Quaternion(45, transform.rotation.y, transform.rotation.z, transform.rotation.w);
    }
}
