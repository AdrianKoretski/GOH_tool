using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOH
{
    public class Settings : MonoBehaviour
    {
        [Tooltip("Value that determines the maximum distance between two consecutive non similar polygons.")]
        public float NonSimilarityThreshold = 0.005f;
        [Range(1f, 20f)]
        [Tooltip("Maximum view distance. You can override the suggested limits in \"GOH_Settings.cs\". Proceed at your own risk.")]
        public float MaxViewDistance = 5;
        [Range(1f, 179f)]
        [Tooltip("Maximum view angle in degrees. Note that this is the total angle (as opposed to half the angle).")]
        public float FieldOfView = 90;
        [Tooltip("When the construction of a polygon fails (usually due to colinear points), the positions is wiggled randomly within this value and attempted again.")]
        public float WiggleDelta = 0.001f;
    }
}