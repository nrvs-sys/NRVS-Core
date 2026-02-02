using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PositionUtility
{
    public enum Direction
    {
        XAxis,
        YAxis,
        ZAxis,
    } 

    public static void GetUniformCircle(float radius, int count, in Vector3[] positions, Direction forward = Direction.YAxis)
    {
        for (int i = 0; i < count; i++)
        {
            var radians = i * 2 * MathF.PI / count;

            var vertical = Mathf.Sin(radians);
            var horizontal = Mathf.Cos(radians);

            Vector3 dir = Vector3.forward;

            switch(forward)
            {
                case Direction.XAxis:
                    dir = new Vector3(0, vertical, horizontal);
                    break;
                case Direction.YAxis:
                    dir = new Vector3(horizontal, 0, vertical);
                    break;
                case Direction.ZAxis:
                    dir = new Vector3(horizontal, vertical, 0);
                    break;
            }
            

            positions[i] = dir * radius;
        }
    }

    public static void GetUniformOval(float width, float height, int count, in Vector3[] positions, Direction forward = Direction.YAxis)
    {
        var w = width * 0.5f;
        var h = height * 0.5f;

        for (int i = 0; i < count; i++)
        {
            var radians = i * 2 * MathF.PI / count;

            var vertical = Mathf.Sin(radians) * h;
            var horizontal = Mathf.Cos(radians) * w;

            Vector3 dir = Vector3.forward;

            switch (forward)
            {
                case Direction.XAxis:
                    dir = new Vector3(0, vertical, horizontal);
                    break;
                case Direction.YAxis:
                    dir = new Vector3(horizontal, 0, vertical);
                    break;
                case Direction.ZAxis:
                    dir = new Vector3(horizontal, vertical, 0);
                    break;
            }


            positions[i] = dir * width;
        }
    }
}
