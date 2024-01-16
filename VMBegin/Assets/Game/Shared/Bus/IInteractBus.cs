using Game.Shared.Bus;
using System;

public interface IInteractBus {
    bool Emit(InteractEvt evt, object data = null);
    IEventPackage On(InteractEvt evt, Action handler);
    IEventPackage On(InteractEvt evt, Action<object> handler);
    void UnregisterByEvent(InteractEvt evt);
    void UnregisterByEvent(InteractEvt evt, Action handler);
    void UnregisterByEvent(InteractEvt evt, Action<object> handler);
}
