const _listeners = [];
const eventName = 'click';

export function attachWindowClickEvent(element, instance, capture) {

    const listener = function (e) {

        if (!element.contains(e.target)) {
            instance.invokeMethodAsync('RaiseOnClickOutside')
        }
    };

    window.addEventListener(eventName, listener, { capture });

    _listeners.push({
        element,
        listener
    });
}

export function detachWindowClickEvent(element) {

    for (var i = _listeners.length - 1; i >= 0; i--) {

        var item = _listeners[i];

        if (item.element != element)
            continue;

        window.removeEventListener(
            eventName,
            item.listener
        );

        _listeners.splice(i, 1);
    }
}
