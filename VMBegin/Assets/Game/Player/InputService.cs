using System.Collections.Generic;

namespace Game.Player {
    public class InputService {

        public static readonly Dictionary<string, string[]> CONTROLS = new Dictionary<string, string[]> {
            {
                "MOUSE",
                new string[2] { "LMB", "RMB" }
            },
            {
                "ANALOGUE",
                new string[4] { "LS Up", "LS Left", "RS Up", "RS Left" }
            },
        };

        public static bool IsControlAnalogue(string controlShort) {
            foreach (var analShort in CONTROLS["ANALOGUE"]) {
                if (controlShort == analShort) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsControlLeftAnalogue(string controlShort) {
            var leftAnalogue = new string[2] { CONTROLS["ANALOGUE"][0], CONTROLS["ANALOGUE"][1] };
            foreach (var analShort in leftAnalogue) {
                if (controlShort == analShort) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsControlRightAnalogue(string controlShort) {
            var rightAnalogue = new string[2] { CONTROLS["ANALOGUE"][2], CONTROLS["ANALOGUE"][3] };
            foreach (var analShort in rightAnalogue) {
                if (controlShort == analShort) {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsControlOfTypeMouse(string controlShort) {
            foreach (var analShort in CONTROLS["MOUSE"]) {
                if (controlShort == analShort) {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsControlLeftClick(string controlShort) {
            return controlShort == CONTROLS["MOUSE"][0];
        }
    }
}
