using System;
using System.Collections.Generic;
using UnityEngine;

namespace KwarterMaster
{
    public class ResourceFlowGraph
    {
        private Dictionary<(string, string), ResourceFlow> resourceFlows;
        private Dictionary<string, ResourceNode> resourceNodes;

        public ResourceFlowGraph()
        {
            resourceFlows = new Dictionary<(string, string), ResourceFlow>();
            resourceNodes = new Dictionary<string, ResourceNode>();
        }

        public void AddFlow(string inputResource, string outputResource, float rate, float ecUsage)
        {
            if (inputResource != null && !resourceNodes.ContainsKey(inputResource))
            {
                resourceNodes[inputResource] = new ResourceNode(inputResource);
            }

            if (!resourceNodes.ContainsKey(outputResource))
            {
                resourceNodes[outputResource] = new ResourceNode(outputResource);
            }

            var flowKey = (inputResource ?? "", outputResource);
            if (!resourceFlows.ContainsKey(flowKey))
            {
                resourceFlows[flowKey] = new ResourceFlow(
                    inputResource != null ? resourceNodes[inputResource] : null,
                    resourceNodes[outputResource],
                    rate,
                    ecUsage
                );
            }
            else
            {
                resourceFlows[flowKey].Rate += rate;
                resourceFlows[flowKey].ECUsage += ecUsage;
            }
        }

        public void AddHarvester(string outputResource, float rate, float ecUsage)
        {
            AddFlow(null, outputResource, rate, ecUsage);
        }

        // Get all flows with the given resource as input
        public List<ResourceFlow> GetProductFlows(string resource)
        {
            List<ResourceFlow> flows = new List<ResourceFlow>();
            foreach (var flow in resourceFlows.Values)
            {
                if (flow.Input != null && flow.Input.Name == resource)
                {
                    flows.Add(flow);
                }
            }

            return flows;
        }

        public void VisitNodes(Action<ResourceNode> visitor)
        {
            foreach (var node in resourceNodes.Values)
            {
                visitor(node);
            }
        }

        public void VisitFlows(Action<ResourceFlow> visitor, bool include_harvesters = false)
        {
            foreach (var flow in resourceFlows.Values) if (include_harvesters || !flow.IsHarvester())
                {
                    visitor(flow);
                }
        }

        public float GetInputRate(ResourceNode node)
        {
            float inputRate = 0f;
            foreach (var flow in GetInputFlows(node, true))
            {
                inputRate += flow.Rate;
            }

            return inputRate;
        }

        public float GetOutputRate(ResourceNode node)
        {
            float outputRate = 0f;
            foreach (var flow in GetOutputFlows(node))
            {
                outputRate += flow.Rate;
            }

            return outputRate;
        }

        public float GetECUsage(ResourceNode node)
        {
            float ecUsage = 0f;
            foreach (var flow in GetInputFlows(node, true))
            {
                ecUsage += flow.ECUsage;
            }

            return ecUsage;
        }

        public void SetInputRates()
        {
            VisitNodes(node => node.InputRate = GetInputRate(node));
        }

        public void SetECUsages()
        {
            VisitNodes(node => node.ECUsage += GetECUsage(node));
        }

        public void SetStorage(string resource, float storageAmount)
        {
            if (resourceNodes.ContainsKey(resource))
            {
                resourceNodes[resource].AddStorage(storageAmount);
            }
            else
            {
                throw new ArgumentException($"Resource {resource} not found in graph");
            }
        }

        public List<ResourceFlow> GetInputFlows(ResourceNode node, bool include_harvesters = false)
        {
            List<ResourceFlow> inputs = new List<ResourceFlow>();
            foreach (var flow in resourceFlows.Values)
            {
                if (flow.Output == node && (include_harvesters || !flow.IsHarvester()))
                {
                    inputs.Add(flow);
                }
            }

            return inputs;
        }

        public List<ResourceFlow> GetOutputFlows(ResourceNode node)
        {
            List<ResourceFlow> outputs = new List<ResourceFlow>();
            foreach (var flow in resourceFlows.Values)
            {
                if (flow.Input == node)
                {
                    outputs.Add(flow);
                }
            }

            return outputs;
        }

        public void AssignXLevels()
        {
            // HashSet to track visited nodes and prevent revisiting
            HashSet<ResourceNode> visitedNodes = new HashSet<ResourceNode>();

            // Helper method to recursively assign xLevels
            void VisitNodeForXLevel(ResourceNode node)
            {
                Debug.Log($"Visiting node {node.Name}");
                // If the node has already been visited, skip it
                if (visitedNodes.Contains(node))
                {
                    Debug.Log($"Node {node.Name} already visited, skipping");
                    return;
                }

                Debug.Log($"Node {node.Name} not visited yet, processing");

                // Mark the node as visited
                visitedNodes.Add(node);
                Debug.Log($"Node {node.Name} has {GetInputFlows(node).Count} inputs");
                // If no inputs, assign xLevel 0 (harvester)
                if (GetInputFlows(node).Count == 0)
                {
                    node.XLevel = 0;
                    Debug.Log($"Node {node.Name} is a harvester, assigned XLevel 0");
                }
                else
                {
                    Debug.Log($"Node {node.Name} has inputs, calculating XLevel");
                    // Calculate xLevel based on max input xLevel + 1
                    foreach (var flow in GetInputFlows(node))
                    {
                        Debug.Log($"Processing input {flow}");
                        VisitNodeForXLevel(flow.Input);  // Ensure input node is processed first
                        node.XLevel = Mathf.Max(node.XLevel, flow.Input.XLevel + 1);
                        Debug.Log($"Node {node.Name} assigned XLevel {node.XLevel} based on input {flow.Input.Name}");
                    }
                }
            }

            // Traverse all nodes in the graph
            foreach (var node in resourceNodes.Values)
            {
                if (!visitedNodes.Contains(node))
                {
                    VisitNodeForXLevel(node);
                }
            }
        }

        public void AssignYLevels()
        {
            // Dictionary to track the current Y level for each X level
            Dictionary<int, int> currentYLevels = new Dictionary<int, int>();

            // Track the overall maximum Y level across all nodes
            int globalMaxYLevel = -1;

            // Helper method to recursively assign yLevels for the outputs of a node
            void VisitNodeForYLevel(ResourceNode node)
            {
                Debug.Log($"Visiting node: {node.Name}, XLevel: {node.XLevel}, YLevel: {node.YLevel}");

                // Track whether it's the first output
                bool isFirstOutput = true;

                // Visit all output nodes and assign yLevels
                foreach (var flow in GetOutputFlows(node))
                {
                    if (flow.Output.YLevel == -1)  // Only assign yLevel if it hasn't been set
                    {
                        if (isFirstOutput)
                        {
                            // First output: same yLevel as the input node
                            flow.Output.YLevel = node.YLevel;
                            isFirstOutput = false;  // Mark that we've handled the first output
                            Debug.Log($"First output: {flow.Output.Name} inherits YLevel {flow.Output.YLevel} from {node.Name}");
                        }
                        else
                        {
                            // Increment the Y level for subsequent outputs
                            globalMaxYLevel++;
                            flow.Output.YLevel = globalMaxYLevel;
                            Debug.Log($"Subsequent output: {flow.Output.Name} assigned new YLevel {flow.Output.YLevel} (incremented)");
                        }

                        Debug.Log($"Node {flow.Output.Name} assigned YLevel {flow.Output.YLevel} based on input {node.Name}");
                    }

                    // Recursively visit the output node
                    VisitNodeForYLevel(flow.Output);
                }

                // After processing all outputs, update the current Y level for future nodes in the same X level
                if (currentYLevels.ContainsKey(node.XLevel))
                {
                    currentYLevels[node.XLevel] = Mathf.Max(currentYLevels[node.XLevel], node.YLevel + 1);
                }
                else
                {
                    currentYLevels[node.XLevel] = node.YLevel + 1;
                }
                Debug.Log($"Updated currentYLevels[{node.XLevel}] to {currentYLevels[node.XLevel]} after processing {node.Name}");
            }

            // Start by processing all the harvesters (nodes with no inputs)
            foreach (var node in resourceNodes.Values)
            {
                if (GetInputFlows(node).Count == 0)  // If the node is a harvester
                {
                    if (node.YLevel == -1)  // Ensure the harvester itself has a Y level
                    {
                        globalMaxYLevel++;  // Increment the global max Y level
                        node.YLevel = globalMaxYLevel;  // Start after the maximum Y level encountered
                        Debug.Log($"Harvester {node.Name} assigned YLevel {node.YLevel}");
                    }

                    // Visit all its outputs
                    VisitNodeForYLevel(node);

                    // After processing this harvester and its outputs, update globalMaxYLevel
                    globalMaxYLevel = Mathf.Max(globalMaxYLevel, currentYLevels.ContainsKey(node.XLevel) ? currentYLevels[node.XLevel] : globalMaxYLevel);
                    Debug.Log($"Updated globalMaxYLevel to {globalMaxYLevel} after processing harvester {node.Name}");
                }
            }
        }

        public int GetMaxXLevel()
        {
            int maxXLevel = -1;
            foreach (var node in resourceNodes.Values)
            {
                maxXLevel = Mathf.Max(maxXLevel, node.XLevel);
            }

            return maxXLevel;
        }

        public int GetMaxYLevel()
        {
            int maxYLevel = -1;
            foreach (var node in resourceNodes.Values)
            {
                maxYLevel = Mathf.Max(maxYLevel, node.YLevel);
            }

            return maxYLevel;
        }

        public void DebugGraph()
        {
            foreach (var flow in resourceFlows.Values)
            {
                Debug.Log($"Flow from {(flow.Input != null ? flow.Input.Name : "Harvester")} to {flow.Output.Name}: {flow.Rate}");
            }

            foreach (var node in resourceNodes.Values)
            {
                Debug.Log($"Storage for {node.Name}: {node.AvailableStorage}");
                Debug.Log($"XLevel for {node.Name}: {node.XLevel}, YLevel for {node.Name}: {node.YLevel}");
            }
        }
    }

}