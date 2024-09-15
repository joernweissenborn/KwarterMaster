using System;
using System.Collections.Generic;
using UnityEngine;

namespace KwarterMaster
{
    public class ResourceFlowGraph
    {
        private Dictionary<(string, string), ResourceFlow> resourceFlows;
        private Dictionary<string, HarvesterNode> harvesterNodes;
        private Dictionary<string, ProductNode> productNodes;

        public ResourceFlowGraph()
        {
            resourceFlows = new Dictionary<(string, string), ResourceFlow>();
            harvesterNodes = new Dictionary<string, HarvesterNode>();
            productNodes = new Dictionary<string, ProductNode>();
        }

        public void Clear()
        {
            resourceFlows.Clear();
            harvesterNodes.Clear();
            productNodes.Clear();
        }

        public void Update()
        {
            AssignXLevels();
            AssignYLevels();
            CalculateProductionRates();
            CalculateEcUsage();
        }

        private bool KnownResource(string resource)
        {
            return harvesterNodes.ContainsKey(resource) || productNodes.ContainsKey(resource);
        }

        private ResourceNode GetNode(string resource)
        {
            if (harvesterNodes.ContainsKey(resource))
            {
                return harvesterNodes[resource];
            }
            else if (productNodes.ContainsKey(resource))
            {
                return productNodes[resource];
            }
            else
            {
                throw new ArgumentException($"Resource {resource} not found in graph");
            }
        }

        private List<ResourceNode> GetNodes()
        {
            List<ResourceNode> nodes = new List<ResourceNode>();
            nodes.AddRange(harvesterNodes.Values);
            nodes.AddRange(productNodes.Values);
            return nodes;
        }

        public void AddFlow(string inputResource, string outputResource, float inputRate, float outputRate, float ecUsage)
        {
            //Debug.Log($"Adding flow from {inputResource ?? "Harvester"} to {outputResource} with input rate {inputRate}, output rate {outputRate}, EC usage {ecUsage}");
            if (inputResource != null)
            {

                if (!KnownResource(inputResource))
                {
                    //Debug.Log($"Adding product node for {inputResource}");
                    productNodes[inputResource] = new ProductNode(inputResource);
                }
                if (!KnownResource(outputResource))
                {
                    //Debug.Log($"Adding product node for {outputResource}");
                    productNodes[outputResource] = new ProductNode(outputResource);
                }
            }
            else
            {
                if (!KnownResource(outputResource))
                {
                    //Debug.Log($"Adding harvester node for {outputResource}");
                    harvesterNodes[outputResource] = new HarvesterNode(outputResource);
                }
            }

            var flowKey = (inputResource ?? "", outputResource);
            if (!resourceFlows.ContainsKey(flowKey))
            {
                resourceFlows[flowKey] = new ResourceFlow(
                    inputResource != null ? GetNode(inputResource) : null,
                    GetNode(outputResource),
                    inputRate,
                    outputRate,
                    ecUsage
                );
            }
            else
            {
                resourceFlows[flowKey].InputRate += inputRate;
                resourceFlows[flowKey].OutputRate += outputRate;
                resourceFlows[flowKey].ECUsage += ecUsage;
            }
        }

        public void AddHarvester(string outputResource, float outputRate, float ecUsage)
        {
            AddFlow(null, outputResource, 0, outputRate, ecUsage);
        }

        public void VisitNodes(Action<ResourceNode> visitor)
        {
            foreach (var node in GetNodes())
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

        public float GetProductionRate(ResourceNode node)
        {
            float productionRate = 0f;
            foreach (var flow in GetInputFlows(node, true))
            {
                productionRate += flow.OutputRate;
            }

            return productionRate;
        }

        public float GetActualProductionRate(ResourceNode node)
        {
            float productionRate = 0f;
            foreach (var flow in GetInputFlows(node))
            {
                productionRate += flow.ActualOutputRate;
            }

            return productionRate;
        }

        private void CalculateActualNodeProductionRate(ProductNode node)
        {
            node.ProductionRate = GetProductionRate(node);
            node.ActualProductionRate = GetActualProductionRate(node);
            //Debug.Log($"Node {node.Name} actual production rate: {node.ActualProductionRate}");

            float actualInput = node.ActualProductionRate / GetOutputFlows(node).Count;
            foreach (var flow in GetOutputFlows(node))
            {
                flow.ActualInputRate = actualInput;
                //Debug.Log($"Flow from {node.Name} to {flow.Output.Name} actual input rate: {flow.ActualInputRate}");
                CalculateActualNodeProductionRate((ProductNode)flow.Output);
            }
        }

        public void CalculateProductionRates()
        {
            foreach (var node in harvesterNodes.Values)
            {
                // Calculate production rate for harvesters
                node.ProductionRate = GetProductionRate(node);
                //Debug.Log($"Harvester {node.Name} production rate: {node.ProductionRate}");

                float actualInput = node.GetActualProductionRate() / GetOutputFlows(node).Count;
                foreach (var flow in GetOutputFlows(node))
                {
                    flow.ActualInputRate = actualInput;
                    //Debug.Log($"Flow from {node.Name} to {flow.Output.Name} actual input rate: {flow.ActualInputRate}");
                    CalculateActualNodeProductionRate((ProductNode)flow.Output);
                }
            }
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

        public void CalculateEcUsage()
        {
            foreach (var node in GetNodes())
            {
                node.ECUsage = GetECUsage(node);
            }
        }

        public float TotalECUsage()
        {
            float totalEcUsage = 0f;
            foreach (var node in GetNodes())
            {
                totalEcUsage += node.ECUsage;
            }

            return totalEcUsage;
        }

        public void AddStorage(string resource, float storageAmount)
        {
            if (KnownResource(resource))
            {
                GetNode(resource).Storage += storageAmount;
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
                //Debug.Log($"Visiting node {node.Name}");
                // If the node has already been visited, skip it
                if (visitedNodes.Contains(node))
                {
                    //Debug.Log($"Node {node.Name} already visited, skipping");
                    return;
                }

                //Debug.Log($"Node {node.Name} not visited yet, processing");

                // Mark the node as visited
                visitedNodes.Add(node);
                //Debug.Log($"Node {node.Name} has {GetInputFlows(node).Count} inputs");
                // If no inputs, assign xLevel 0 (harvester)
                if (GetInputFlows(node).Count == 0)
                {
                    node.XLevel = 0;
                    //Debug.Log($"Node {node.Name} is a harvester, assigned XLevel 0");
                }
                else
                {
                    //Debug.Log($"Node {node.Name} has inputs, calculating XLevel");
                    // Calculate xLevel based on max input xLevel + 1
                    foreach (var flow in GetInputFlows(node))
                    {
                        //Debug.Log($"Processing input {flow}");
                        VisitNodeForXLevel(flow.Input);  // Ensure input node is processed first
                        node.XLevel = Mathf.Max(node.XLevel, flow.Input.XLevel + 1);
                        //Debug.Log($"Node {node.Name} assigned XLevel {node.XLevel} based on input {flow.Input.Name}");
                    }
                }
            }

            // Traverse all nodes in the graph
            foreach (var node in GetNodes())
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
                //Debug.Log($"Visiting node: {node.Name}, XLevel: {node.XLevel}, YLevel: {node.YLevel}");

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
                            //Debug.Log($"First output: {flow.Output.Name} inherits YLevel {flow.Output.YLevel} from {node.Name}");
                        }
                        else
                        {
                            // Increment the Y level for subsequent outputs
                            globalMaxYLevel++;
                            flow.Output.YLevel = globalMaxYLevel;
                            //Debug.Log($"Subsequent output: {flow.Output.Name} assigned new YLevel {flow.Output.YLevel} (incremented)");
                        }

                        //Debug.Log($"Node {flow.Output.Name} assigned YLevel {flow.Output.YLevel} based on input {node.Name}");
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
                //Debug.Log($"Updated currentYLevels[{node.XLevel}] to {currentYLevels[node.XLevel]} after processing {node.Name}");
            }

            // Start by processing all the harvesters (nodes with no inputs)
            foreach (var node in harvesterNodes.Values)
            {

                if (node.YLevel == -1)  // Ensure the harvester itself has a Y level
                {
                    globalMaxYLevel++;  // Increment the global max Y level
                    node.YLevel = globalMaxYLevel;  // Start after the maximum Y level encountered
                    //Debug.Log($"Harvester {node.Name} assigned YLevel {node.YLevel}");
                }

                // Visit all its outputs
                VisitNodeForYLevel(node);

                // After processing this harvester and its outputs, update globalMaxYLevel
                globalMaxYLevel = Mathf.Max(globalMaxYLevel, currentYLevels.ContainsKey(node.XLevel) ? currentYLevels[node.XLevel] : globalMaxYLevel);
                //Debug.Log($"Updated globalMaxYLevel to {globalMaxYLevel} after processing harvester {node.Name}");
            }
        }

        public int GetMaxXLevel()
        {
            int maxXLevel = -1;
            foreach (var node in productNodes.Values)
            {
                maxXLevel = Mathf.Max(maxXLevel, node.XLevel);
            }

            return maxXLevel;
        }

        public int GetMaxYLevel()
        {
            int maxYLevel = -1;
            foreach (var node in productNodes.Values)
            {
                maxYLevel = Mathf.Max(maxYLevel, node.YLevel);
            }

            return maxYLevel;
        }

        public void DebugGraph()
        {
            foreach (var flow in resourceFlows.Values)
            {
                Debug.Log($"Flow from {(flow.Input != null ? flow.Input.Name : "Harvester")} to {flow.Output.Name}: {flow.OutputRate}");
            }

            foreach (var node in productNodes.Values)
            {
                Debug.Log($"Storage for {node.Name}: {node.Storage}");
                Debug.Log($"XLevel for {node.Name}: {node.XLevel}, YLevel for {node.Name}: {node.YLevel}");
            }
        }
    }

}