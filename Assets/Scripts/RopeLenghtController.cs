using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class RopeLenghtController : MonoBehaviour
{
    public float speed = 1;
    ObiRopeCursor cursor;
    ObiRope rope;
    float target = 1f;

    void OnEnable()
    {
        cursor = GetComponent<ObiRopeCursor>();
        rope = cursor.GetComponent<ObiRope>();
    }

    void Update()
    {
        StartCoroutine(movingCursor());
        if (Input.GetMouseButton(0))
        {
            cursor.ChangeLength(rope.restLength - speed * Time.deltaTime);
            cursor.
            rope.blueprint.RecalculateBounds();

        }
    }

    private IEnumerator movingCursor()
    {
        while (true)
        {
            if (cursor.cursorMu >= 0.9f)
            {
                target = 0f;
            }
            if (cursor.cursorMu <= 0.1f)
                target = 1f;
            cursor.cursorMu = Mathf.MoveTowards(cursor.cursorMu, target, Time.deltaTime / 1000);
            yield return null;
        }
    }

}
