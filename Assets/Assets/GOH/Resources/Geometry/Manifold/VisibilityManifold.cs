using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GOH
{
    struct ClosePair
    {
        public Node bottom;
        public Node top;
        public float distance;

        public ClosePair(Node node_0, Node node_1, float distance)
        {
            if (node_0 == null || node_1 == null)
            {
                bottom = null;
                top = null;
                this.distance = distance;
                return;
            }
            if (node_0.timestamp > node_1.timestamp)
            {
                bottom = node_1;
                top = node_0;
            }
            else
            {
                bottom = node_0;
                top = node_1;
            }
            this.distance = distance;
        }
    }

    public class VisibilityManifold
    {
        private List<VisibilityPolygon> m_vis_polys = new List<VisibilityPolygon>();

        public void addArea(VisibilityPolygon vp)
        {
            List<Node> vp_graph = vp.getVisibilityArea();

            m_vis_polys.Add(vp);
            setManifoldIDs(vp_graph);

            saveNodeObj(vp_graph);
            GameObject new_layer = SimplifiedGraphVisualiser.instance.AddLayer(vp.getTimestamp());
            addLayerVisuals(vp_graph, new_layer.transform);

            if (layer_count() != 1)
                if (vp.compare(second_last()))
                    connectSame(vp.getTimestamp());
                else
                    connectDifferent();
            else
                cap();
        }

        private VisibilityPolygon last()
        {
            return m_vis_polys[layer_count() - 1];
        }

        private VisibilityPolygon second_last()
        {
            return m_vis_polys[layer_count() - 2];
        }

        private int layer_count()
        {
            return m_vis_polys.Count;
        }

        private void connectSame(float timeStamp)
        {
            for (int i = 0; i < second_last().count() - 1; i++)
            {
                connectNodeTime(i, i, timeStamp);
                connectNodeTime(i + 1, i, timeStamp);
                addTriangle(
                    second_last().at(i),
                    second_last().at(i + 1),
                    last().at(i)
                    );
                addTriangle(
                    last().at(i),
                    second_last().at(i + 1),
                    last().at(i + 1)
                    );
            }
            connectDangler(timeStamp);
        }

        private void connectDangler(float timeStamp)
        {
            Node n0 = second_last().first();
            Node n1 = last().last();
            connectNodeTime(second_last().count() - 1, last().count() - 1, timeStamp);
            new Edge(n0, n1);
            addTriangle
                (
                second_last().last(),
                second_last().first(),
                last().last()
                );
            addTriangle(
                last().last(),
                second_last().first(),
                last().first()
                );
        }

        // Big mess.

        private void connectDifferent()
        {
            VisibilityPolygon layer_0 = second_last();
            VisibilityPolygon layer_1 = last();

            connectSimilarNodes(layer_0, layer_1);

            List<Node> single_nodes_0 = getUnconnectedNodes(layer_0, layer_1.getTimestamp());
            List<Node> single_nodes_1 = getUnconnectedNodes(layer_1, layer_0.getTimestamp());

            List<ClosePair> closest_0 = generateClosestNodePairs(single_nodes_0, layer_0, layer_1);
            List<ClosePair> closest_1 = generateClosestNodePairs(single_nodes_1, layer_1, layer_0);

            while (closest_0.Count != 0 || closest_1.Count != 0)
            {
                ClosePair closest = new ClosePair(null, null, float.PositiveInfinity);
                for (int i = 0; i < closest_0.Count; i++)
                {
                    if (doesConnectionCross(closest_0[i].bottom, closest_0[i].top, layer_0, layer_1))
                    {
                        Node node = getClosest(closest_0[i].bottom, layer_0, layer_1);
                        float distance = Helpers.Distance(closest_0[i].bottom, node);
                        closest_0[i] = new ClosePair(closest_0[i].bottom, node, distance);
                    }
                    if (closest_0[i].distance < closest.distance)
                        closest = closest_0[i];
                }
                for (int i = 0; i < closest_1.Count; i++)
                {
                    if (doesConnectionCross(closest_1[i].bottom, closest_1[i].top, layer_0, layer_1))
                    {
                        Node node = getClosest(closest_1[i].top, layer_0, layer_1);
                        float distance = Helpers.Distance(closest_1[i].top, node);
                        closest_1[i] = new ClosePair(closest_1[i].top, node, distance);
                    }
                    if (closest_1[i].distance < closest.distance)
                        closest = closest_1[i];
                }
                int index_of_0 = layer_0.IndexOf(closest.bottom);
                int index_of_1 = layer_1.IndexOf(closest.top);
                connectNodeTime(index_of_0, index_of_1, layer_1.getTimestamp());
                closest_0.Remove(closest);
                closest_1.Remove(closest);
            }
            triangulate(layer_0, layer_1);
            connectDangler(layer_1.getTimestamp());
        }

        List<ClosePair> generateClosestNodePairs(List<Node> single_nodes, VisibilityPolygon single_layer, VisibilityPolygon other_layer)
        {
            List<ClosePair> node_list_closest = new List<ClosePair>();

            for (int j = 0; j < single_nodes.Count; j++)
            {
                Node node = getClosest(single_nodes[j], other_layer, single_layer);
                float distance = Helpers.Distance(single_nodes[j], node);
                node_list_closest.Add(new ClosePair(single_nodes[j], node, distance));
            }
            return node_list_closest;
        }

        private void connectSimilarNodes(VisibilityPolygon layer_0, VisibilityPolygon layer_1)
        {
            for (int i = 0; i < layer_0.count(); i++)
                for (int j = 0; j < layer_1.count(); j++)
                    if (Node.Compare(layer_1.at(j), layer_0.at(i)))
                        connectNodeTime(i, j, layer_0.getTimestamp());
        }

        private Node getClosest(Node node_1, VisibilityPolygon layer_0, VisibilityPolygon layer_1)
        {
            float distance = float.PositiveInfinity;
            Node closest_index = null;
            for (int i = 0; i < layer_0.count(); i++)
                if (Helpers.Distance(layer_0.at(i), node_1) < distance && !doesConnectionCross(layer_0.at(i), node_1, layer_0, layer_1))
                {
                    distance = Helpers.Distance(layer_0.at(i), node_1);
                    closest_index = layer_0.at(i);
                }
            return closest_index;
        }

        private bool doesConnectionCross(Node node_0, Node node_1, VisibilityPolygon layer_0, VisibilityPolygon layer_1)
        {
            int index_0 = layer_0.IndexOf(node_0);
            int index_1 = layer_1.IndexOf(node_1);
            for (int i = index_0 + 1; i < layer_0.count(); i++)
                for (int j = 0; j < index_1; j++)
                    if (layer_0.at(i).IsNeighbor(layer_1.at(j)))
                        return true;

            for (int i = index_1 + 1; i < layer_1.count(); i++)
                for (int j = 0; j < index_0; j++)
                    if (layer_1.at(i).IsNeighbor(layer_0.at(j)))
                        return true;
            return false;
        }

        private void triangulate(VisibilityPolygon source, VisibilityPolygon dest)
        {
            int src_index = 0;
            int dst_index = 0;

            while (src_index != source.count() - 1 || dst_index != dest.count() - 1)
            {
                if (src_index != source.count() - 1 && dest.at(dst_index).IsNeighbor(source.at(src_index + 1)))
                {
                    addTriangle
                        (
                            dest.at(dst_index),
                            source.at(src_index),
                            source.at(src_index + 1)
                        );
                    src_index++;
                    continue;
                }
                if (dst_index != dest.count() - 1 && source.at(src_index).IsNeighbor(dest.at(dst_index + 1)))
                {
                    addTriangle
                    (
                        dest.at(dst_index),
                        source.at(src_index),
                        dest.at(dst_index + 1)
                    );
                    dst_index++;
                    continue;
                }
                else
                {
                    if (src_index == source.count() - 1 && dst_index == dest.count() - 2)
                    {
                        addTriangle(source.at(src_index), dest.at(dst_index + 1), dest.at(dst_index));
                        break;
                    }
                    if (src_index == source.count() - 2 && dst_index == dest.count() - 1)
                    {
                        addTriangle(source.at(src_index), source.at(src_index + 1), dest.at(dst_index));
                        break;
                    }
                    if (src_index != source.count() - 1)
                        connectNodeTime(src_index + 1, dst_index, dest.getTimestamp());
                    else if (dst_index != dest.count() - 1)
                        connectNodeTime(src_index, dst_index + 1, dest.getTimestamp());
                }
            }
        }

        public void cap()
        {
            List<Node> vp_copy = new List<Node>(last().getVisibilityArea());
            Node origin = vp_copy[0];
            vp_copy.RemoveAt(0);
            for (int i = 0; i < vp_copy.Count - 2; i++)
            {
                if (Helpers.Angle(vp_copy[i], vp_copy[i + 1], vp_copy[i + 2]) < Math.PI)
                {
                    if (m_vis_polys.Count == 1)
                        addTriangle(vp_copy[i + 1], vp_copy[i], vp_copy[i + 2]);
                    else
                        addTriangle(vp_copy[i], vp_copy[i + 1], vp_copy[i + 2]);
                    vp_copy.RemoveAt(i + 1);
                    i--;
                }
            }
            for (int i = 0; i < vp_copy.Count - 1; i++)
                if (m_vis_polys.Count == 1)
                    addTriangle(vp_copy[i], origin, vp_copy[i + 1]);
                else
                    addTriangle(origin, vp_copy[i], vp_copy[i + 1]);
        }

        private List<Node> getUnconnectedNodes(VisibilityPolygon input_vertex_list, float timestamp)
        {
            List<Node> output_vertex_list = new List<Node>();

            for (int i = 0; i < input_vertex_list.count(); i++)
                if (!input_vertex_list.at(i).HasNeighborFromTime(timestamp))
                {
                    for (int j = i; j < input_vertex_list.count() && !input_vertex_list.at(j).HasNeighborFromTime(timestamp); j++)
                        output_vertex_list.Add(input_vertex_list.at(j));
                    break;
                }
            return output_vertex_list;
        }

        private void addTriangle(Node vx0, Node vx1, Node vx2)
        {
            displayTriangle(vx0, vx1, vx2);
            saveTriangleObj(vx0, vx1, vx2);
        }

        private void connectNodeTime(int node_index_0, int node_index_1, float timeStamp)
        {
            Node n0 = second_last().at(node_index_0);
            Node n1 = last().at(node_index_1);
            new Edge(n0, n1);
            GameObject go = GOH_Visualiser.CreateLine(null, getManifoldPosition(layer_count() - 2, node_index_0), getManifoldPosition(layer_count() - 1, node_index_1), Color.black, Color.black, "Line " + " ");
            SimplifiedGraphVisualiser.instance.AddGeom(go, timeStamp);
        }

        // ----------------- The following code is here to be able to export the manifolds to obj files.

        private int manifold_vertex_count = 1;

        //public string destination = "C:\\Users\\Reynard\\Desktop\\save.obj";
        public string destination = "C:\\Users\\Adrian Koretski\\Desktop\\save.obj";

        private void setManifoldIDs(List<Node> new_vp_graph)
        {
            for (int i = 0; i < new_vp_graph.Count; i++)
            {
                //new_vp_graph[i].manifold_id = manifold_vertex_count;
                manifold_vertex_count++;
            }
        }

        private void saveNodeObj(List<Node> vp_graph)
        {
            if (layer_count() == 0)
                System.IO.File.WriteAllText(destination, "");
            for (int i = 0; i < vp_graph.Count; i++)
                System.IO.File.AppendAllText(destination, "v " + vp_graph[i].position.x + " " + vp_graph[i].position.y + " " + vp_graph[i].timestamp + "\n");
        }
        private void saveTriangleObj(Node vx0, Node vx1, Node vx2)
        {
            //System.IO.File.AppendAllText(destination, "f " + vx1.manifold_id + " " + vx0.manifold_id + " " + vx2.manifold_id + "\n\n");
        }

        // ----------------- All code past this point is for display purposes and serves no purpose for the alg.

        private List<Vector3> display_position = new List<Vector3>();
        private List<Vector3> display_normal = new List<Vector3>();
        private List<int> display_index = new List<int>();

        private void addLayerVisuals(List<Node> new_vp_graph, Transform parent)
        {
            for (int i = 0; i < new_vp_graph.Count; i++)
                GOH_Visualiser.CreatePoint(parent, getManifoldPosition(layer_count() - 1, i), Color.black, " ");
            for (int i = 0; i < new_vp_graph.Count - 1; i++)
                GOH_Visualiser.CreateLine(parent, getManifoldPosition(layer_count() - 1, i), getManifoldPosition(layer_count() - 1, i + 1), Color.black, Color.black, "Line " + " ");
        }

        private void displayTriangle(Node vx0, Node vx1, Node vx2)
        {
            Vector3 v0 = vx0.position;
            Vector3 v1 = vx1.position;
            Vector3 v2 = vx2.position;
            v0.z = v0.y + 10;
            v1.z = v1.y + 10;
            v2.z = v2.y + 10;
            v0.y = vx0.timestamp;
            v1.y = vx1.timestamp;
            v2.y = vx2.timestamp;
            Vector3 normal = (Vector3.Cross((v2 - v1), (v0 - v1))).normalized;

            display_position.Add(v0);
            display_position.Add(v1);
            display_position.Add(v2);

            display_normal.Add(normal);
            display_normal.Add(normal);
            display_normal.Add(normal);

            display_index.Add(display_index.Count);
            display_index.Add(display_index.Count);
            display_index.Add(display_index.Count);

            display_position.Add(v1);
            display_position.Add(v0);
            display_position.Add(v2);

            display_normal.Add(-normal);
            display_normal.Add(-normal);
            display_normal.Add(-normal);

            display_index.Add(display_index.Count);
            display_index.Add(display_index.Count);
            display_index.Add(display_index.Count);

            ManifoldVisualiser.instance.GetComponent<MeshFilter>().mesh.vertices = display_position.ToArray();
            ManifoldVisualiser.instance.GetComponent<MeshFilter>().mesh.triangles = display_index.ToArray();
            ManifoldVisualiser.instance.GetComponent<MeshFilter>().mesh.normals = display_normal.ToArray();
        }

        private void generatePoint(Vector3 position, Color color, String name, Transform transform)
        {
            GameObject temp = new GameObject(name);
            temp.transform.parent = transform;
            temp.transform.position = position;
            temp.AddComponent<SpriteRenderer>();
            temp.GetComponent<SpriteRenderer>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            temp.GetComponent<SpriteRenderer>().color = color;
        }

        private Vector3 getManifoldPosition(int polygon_index, int node_index)
        {
            Vector3 v = new Vector3();
            v.y = polygon_index;
            v.z = -50;
            v.x = -node_index + (float)(m_vis_polys[polygon_index].count()) / 2;
            return v;
        }
    }
}