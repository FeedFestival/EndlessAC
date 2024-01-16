using GameScrypt.Bus;
using System;

namespace Game.Shared.Bus {
    public class InteractBus : GSBus, IInteractBus {
        public bool Emit(InteractEvt evt, object data = null) {
            return base.Emit(evt, data);
        }
        public IEventPackage On(InteractEvt evt, Action handler) {
            return base.On(evt, handler) as IEventPackage;
        }
        public IEventPackage On(InteractEvt evt, Action<object> handler) {
            return base.On(evt, handler) as IEventPackage;
        }
        public void UnregisterByEvent(InteractEvt evt) {
            base.UnregisterByEvent(evt);
        }
        public void UnregisterByEvent(InteractEvt evt, Action handler) {
            base.UnregisterByEvent(evt, handler);
        }
        public void UnregisterByEvent(InteractEvt evt, Action<object> handler) {
            base.UnregisterByEvent(evt, handler);
        }
    }
}
