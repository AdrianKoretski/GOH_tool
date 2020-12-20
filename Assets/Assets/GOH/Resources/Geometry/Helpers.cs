using System;
using UnityEngine;

namespace GOH
{
    public class Helpers
    {
        public static float Cross(Vector2 vector_0, Vector2 vector_1)
        {
            return vector_0.x * vector_1.y - vector_1.x * vector_0.y;
        }

        public static float Angle(Node node_0, Node node_middle, Node node_1)
        {
            Vector2 v_0 = node_0.position;
            Vector2 v_m = node_middle.position;
            Vector2 v_1 = node_1.position;
            float length_01 = (v_0 - v_m).magnitude;
            float length_12 = (v_m - v_1).magnitude;
            float length_20 = (v_1 - v_0).magnitude;
            float angle = Mathf.Acos((length_12 * length_12 + length_01 * length_01 - length_20 * length_20) / (2 * length_12 * length_01));
            if (Cross(v_0 - v_m, v_1 - v_m) > 0)
                return angle;
            return 2 * Mathf.PI - angle;
        }

        public static Vector2 InterceptPoint(Edge edge, Node node_0, Node node_1)
        {
            Vector2 p_0 = node_0.position;
            Vector2 p_1 = node_1.position;
            Vector2 q_0 = edge.node_0.position;
            Vector2 q_1 = edge.node_1.position;
            Vector2 r = p_1 - p_0;
            Vector2 s = q_1 - q_0;
            if (Cross(r, s) == 0)
                throw new Exception("Ill defined intercept. ");
            float t = Cross((q_0 - p_0), s) / Cross(r, s);
            float u = Cross((q_0 - p_0), r) / Cross(r, s);
            if (t < 0 || u < 0 || u > 1)
                return new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            return p_0 + t * r;
        }

        public static bool IsContained(Vector2 vector_0, Vector2 vector_1, Vector2 vector_2, Vector2 vector_p)
        {
            if (Cross(vector_1 - vector_0, vector_p - vector_0) >= 0)
                return false;
            if (Cross(vector_2 - vector_1, vector_p - vector_1) >= 0)
                return false;
            if (Cross(vector_0 - vector_2, vector_p - vector_2) >= 0)
                return false;
            return true;
        }

        public static bool IsContained(Vector2[] vectors, Node node)
        {
            return IsContained(vectors[0], vectors[1], vectors[2], node.position);
        }

        public static bool IsContained(Node[] triangle, Node node)
        {
            return IsContained(triangle[0].position, triangle[1].position, triangle[2].position, node.position);
        }

        public static bool HasIntersect(Vector2 vector_0, Vector2 vector_1, Vector2 vector_2, Vector2 vector_3)
        {
            Vector2 v0 = vector_2 - vector_0;
            Vector2 v1 = vector_3 - vector_0;
            Vector2 v2 = vector_1 - vector_0;

            if (Cross(v2, v0) * Cross(v2, v1) > 0)
                return false;

            v0 = vector_0 - vector_2;
            v1 = vector_1 - vector_2;
            v2 = vector_3 - vector_2;

            if (Cross(v2, v0) * Cross(v2, v1) > 0)
                return false;
            return true;
        }

        public static bool HasIntersect(Edge edge, Node node_0, Node node_1)
        {
            return HasIntersect(edge.node_0.position, edge.node_1.position, node_0.position, node_1.position);
        }

        public static bool HasIntersect(Edge edge, Vector2 vector_0, Vector2 vector_1)
        {
            return HasIntersect(edge.node_0.position, edge.node_1.position, vector_0, vector_1);
        }

        public static Pip Average(Pip pip_0, Pip pip_1)
        {
            return new Pip((pip_0.position + pip_1.position) / 2, (pip_0.orientation + pip_1.orientation) / 2, (pip_0.timestamp + pip_1.timestamp) / 2);
        }

        public static float Distance(Node node_0, Node node_1)
        {
            return Vector2.Distance(node_0.position, node_1.position);
        }
    }
}