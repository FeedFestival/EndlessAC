using System.Collections.Generic;
using UnityEngine;

namespace Game.GraphJob {
    public class GraphJobData {


    }

    public class GraphJobErrorData {

        public Color32 Color { get; set; }

        public GraphJobErrorData() {
            generateRandomColor();
        }

        private void generateRandomColor() {

            Color = new Color32(
                255,    //(byte)Random.Range(65, 255),
                0,      //(byte)Random.Range(50, 175),
                0,      //(byte)Random.Range(50, 175),
                255
            );
        }
    }

    public class GraphJobNodeErrorData {

        public GraphJobErrorData ErrorData { get; set; }
        public List<IGraphNode> Nodes { get; set; }

        public GraphJobNodeErrorData() {
            ErrorData = new GraphJobErrorData();
            Nodes = new List<IGraphNode>();
        }
    }
}
