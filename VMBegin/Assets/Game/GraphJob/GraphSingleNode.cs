using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Game.GraphJob {
    public class GraphSingleNode : GraphNode {

        public override void Init(Vector2 position, Action<IGraphNode, string> onNameChange) {
            base.Init(position, onNameChange);

            NodeType = NodeType.SingleChooice;

            Choices.Add("Next");
        }

        public override void Draw() {
            base.Draw();

            /* OUTPUT CONTAINER */

            foreach (var choice in Choices) {

                Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));

                choicePort.portName = choice;
                outputContainer.Add(choicePort);

            }

            base.RefreshExpandedState();
        }
    }
}