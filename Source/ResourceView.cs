using UnityEngine;

namespace KwarterMaster
{
    public class ResourceView
    {
        private Vector2 _scrollPosition = Vector2.zero;
        private float _width;
        private ResourceFlowGraph _resourceFlowGraph;

        public ResourceView(ResourceFlowGraph resourceFlowGraph, float width)
        {
            _resourceFlowGraph = resourceFlowGraph;
            _width = width;
        }

        public void Draw()
        {
            _scrollPosition = GUILayout.BeginScrollView(
                _scrollPosition, GUILayout.Width(_width), GUILayout.Height(500)
            );

            // Set up basic node positions
            Vector2 orePosition = new Vector2(10, 10);
            Vector2 lfPosition = new Vector2(220, 70);
            Vector2 oxPosition = new Vector2(220, 130);


            // Determine the scrollable area size based on node positions
            float contentWidth = (_resourceFlowGraph.GetMaxXLevel() + 1) * (ResourceNode.Width + ResourceNode.XSpacing);
            float contentHeight = (_resourceFlowGraph.GetMaxYLevel() + 1) * (ResourceNode.Height + ResourceNode.YSpacing);
            // Create a larger area for content to avoid clipping
            GUILayout.BeginHorizontal(GUILayout.Width(contentWidth), GUILayout.Height(contentHeight));
            GUILayout.BeginVertical();

            _resourceFlowGraph.VisitNodes((node) => node.Draw());
            _resourceFlowGraph.VisitFlows((flow) => flow.Draw());

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

        }


    }
}