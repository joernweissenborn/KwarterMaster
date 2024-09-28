using UnityEngine;

namespace KwarterMaster
{
    public class ResourceView
    {
        private Vector2 _scrollPosition = Vector2.zero;
        private readonly ResourceFlowGraph _resourceFlowGraph;

        public ResourceView(ResourceFlowGraph resourceFlowGraph)
        {
            _resourceFlowGraph = resourceFlowGraph;
        }

        public void Draw()
        {
            _scrollPosition = GUILayout.BeginScrollView(
                _scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)
            );

            // Determine the scrollable area size based on node positions
            float contentWidth = (_resourceFlowGraph.GetMaxXLevel() + 1) * (ResourceNode.Width + ResourceNode.XSpacing);
            int maxYLevel = _resourceFlowGraph.GetMaxYLevel();
            float contentHeight = (maxYLevel + 1) * ResourceNode.Height + maxYLevel * ResourceNode.YSpacing;

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