using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.GraphJob {
    public class JobGraphView : GraphView {

        private Dictionary<string, GraphJobNodeErrorData> _ungroupedNodes;
        private Dictionary<Group, Dictionary<string, GraphJobNodeErrorData>> _groupedNodes;

        public JobGraphView() {

            _ungroupedNodes = new Dictionary<string, GraphJobNodeErrorData>();
            _groupedNodes = new Dictionary<Group, Dictionary<string, GraphJobNodeErrorData>>();

            addManipulators();
            addGridBackground();

            addStyles();

            deleteSelection = (operationName, askUser) => {
                var nodesToDelete = new List<GraphNode>();
                foreach (var el in selection) {
                    if (el is GraphNode node) {
                        nodesToDelete.Add(node);
                    }
                }

                foreach (var node in nodesToDelete) {
                    removeUngroupedNode(node);
                    base.RemoveElement(node);
                }
            };

            elementsAddedToGroup = (group, elements) => {
                foreach (var el in elements) {
                    if (el is GraphNode node) {

                        removeUngroupedNode(node);
                        addGroupedNode(node, group);
                    }
                }
            };

            elementsRemovedFromGroup = (group, elements) => {
                foreach (var el in elements) {
                    if (el is GraphNode node) {

                        removeGroupedNode(node, group);
                        addUngroupedNode(node);
                    }
                }
            };
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {

            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port => {

                if (startPort == port || startPort.node == port.node || startPort.direction == port.direction) {
                    return;
                }

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        private void addGridBackground() {
            var gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }

        private void addStyles() {

            var styleSheet = (StyleSheet)EditorGUIUtility.Load("GameScryptGraphView.uss");
            styleSheets.Add(styleSheet);
            styleSheet = (StyleSheet)EditorGUIUtility.Load("GraphJobNodeStyles.uss");
            styleSheets.Add(styleSheet);
        }

        private void addManipulators() {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(createNodeContextualMenu());

            this.AddManipulator(createGroupContextualMenu());
        }

        private IManipulator createNodeContextualMenu() {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => {
                    menuEvent.menu.AppendAction("Add Single Node", actionEvent => {
                        AddElement(createSingleNode(getLocalMousePosition(actionEvent.eventInfo.localMousePosition)));
                    });
                }
            );

            return contextualMenuManipulator;
        }

        private IManipulator createGroupContextualMenu() {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => {
                    menuEvent.menu.AppendAction("Add Group", actionEvent => {
                        AddElement(createGroup("Group Name", getLocalMousePosition(actionEvent.eventInfo.localMousePosition)));
                    });
                }
            );

            return contextualMenuManipulator;
        }

        private Group createGroup(string groupName, Vector2 position) {
            var group = new Group() {
                title = groupName
            };

            group.SetPosition(new Rect(position, Vector2.zero));

            return group;
        }

        private Node createSingleNode(Vector2 position) {
            var node = new GraphSingleNode();
            return setupNode(node, position);
        }

        private Node setupNode(IGraphNode node, Vector2 position) {

            node.Init(position, (node, newName) => {
                if (node.Group == null) {
                    removeUngroupedNode(node);
                    node.Name = newName;
                    addUngroupedNode(node);

                    return;
                }

                // TODO: this is where we are...
                var currentGroup = node.Group;
                // let's see if it works without

                removeGroupedNode(node as GraphNode, node.Group);
                node.Name = newName;
                addGroupedNode(node as GraphNode, node.Group);
            });
            node.Draw();

            addUngroupedNode(node);

            return node as Node;
        }

        private void addUngroupedNode(IGraphNode node) {
            if (!_ungroupedNodes.ContainsKey(node.Name)) {
                var nodeErrorData = new GraphJobNodeErrorData();
                nodeErrorData.Nodes.Add(node);
                _ungroupedNodes.Add(node.Name, nodeErrorData);
            } else {
                _ungroupedNodes[node.Name].Nodes.Add(node);

                Color errorColor = _ungroupedNodes[node.Name].ErrorData.Color;
                node.SetErrorStyle(errorColor);

                // color first node
                if (_ungroupedNodes[node.Name].Nodes.Count == 2) {
                    _ungroupedNodes[node.Name].Nodes[0].SetErrorStyle(errorColor);
                }
            }
        }

        private void removeUngroupedNode(IGraphNode node) {
            _ungroupedNodes[node.Name].Nodes.Remove(node);
            node.ResetStyle();

            if (_ungroupedNodes[node.Name].Nodes.Count == 1) {
                _ungroupedNodes[node.Name].Nodes[0].ResetStyle();
            }

            if (_ungroupedNodes[node.Name].Nodes.Count == 0) {
                _ungroupedNodes.Remove(node.Name);
            }
        }

        private void addGroupedNode(GraphNode node, Group group) {

            node.Group = group;

            if (!_groupedNodes.ContainsKey(group)) {
                _groupedNodes.Add(group, new Dictionary<string, GraphJobNodeErrorData>());
            }

            if (!_groupedNodes[group].ContainsKey(node.Name)) {
                var nodeErrorData = new GraphJobNodeErrorData();
                nodeErrorData.Nodes.Add(node);
                _groupedNodes[group].Add(node.Name, nodeErrorData);
            } else {
                _groupedNodes[group][node.Name].Nodes.Add(node);

                Color errorColor = _groupedNodes[group][node.Name].ErrorData.Color;
                node.SetErrorStyle(errorColor);

                // color first node
                if (_groupedNodes[group][node.Name].Nodes.Count == 2) {
                    _groupedNodes[group][node.Name].Nodes[0].SetErrorStyle(errorColor);
                }
            }
        }

        private void removeGroupedNode(GraphNode node, Group group) {

            _groupedNodes[group][node.Name].Nodes.Remove(node);
            node.ResetStyle();

            if (_groupedNodes[group][node.Name].Nodes.Count == 1) {
                _groupedNodes[group][node.Name].Nodes[0].ResetStyle();
            }

            if (_groupedNodes[group][node.Name].Nodes.Count == 0) {
                _groupedNodes[group].Remove(node.Name);
            }

            node.Group = null;
        }

        private Vector2 getLocalMousePosition(Vector2 mousePosition) {
            Vector2 worldMousePosition = mousePosition;

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
            return localMousePosition;
        }
    }
}
