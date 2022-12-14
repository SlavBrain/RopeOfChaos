using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transforming : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] float _speed;
    [SerializeField] float _angle;
    [SerializeField] float _sign;
    [SerializeField] HingeJoint _springJoint;
    float MINsIZE = 0.4f;

    private void Update()
    {
        //_angle = Vector3.Angle(Vector3.forward, _target.position- transform.position);
        //_sign = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(transform.position, _target.position)));
        //_angle = _angle * _sign;
        if (Input.GetMouseButtonDown(0))
        {
            //Rotate();
        }
        //
    }

    public void Rotate()
    {
        //transform.eulerAngles = new Vector3(transform.rotation.x,  _angle, transform.rotation.z);
        //transform.rotation = Quaternion.EulerAngles(90, 0, transform.rotation.z);
        //transform.rotation = new Quaternion(45, transform.rotation.y, transform.rotation.z, transform.rotation.w);
    }

    public void Decrease()
    {        
        transform.localScale = new Vector3(transform.localScale.x, Mathf.MoveTowards(transform.localScale.y,MINsIZE,_speed), transform.localScale.z);
        if (_springJoint.connectedBody != null)
        {
            _springJoint.connectedAnchor = _target.transform.position-transform.position;
        }
    }
}
