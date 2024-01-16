namespace Game.Shared.Bus {
    public static class __ {
        public static IGameBus GameBus {
            get {
                if (_gameBus == null) {
                    _gameBus = new GameBus();
                }
                return _gameBus;
            }
        }
        private static IGameBus _gameBus;

        public static IInteractBus InteractBus {
            get {
                if (_bus == null) {
                    _bus = new InteractBus();
                }
                return _bus;
            }
        }
        private static IInteractBus _bus;

        public static void ClearAll() {
            _gameBus = null;
        }
    }
}
