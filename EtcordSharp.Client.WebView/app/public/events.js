function EventEmitter() {
  this._events = {};
}

EventEmitter.prototype.on = function(name, listener) {
  if (!this._events[name]) {
    this._events[name] = [];
  }

  this._events[name].push(listener);
};

EventEmitter.prototype.removeListener = function(name, listenerToRemove) {
  if (!this._events[name]) {
    throw new Error(`Can't remove a listener. Event "${name}" doesn't exits.`);
  }

  const filterListeners = (listener) => listener !== listenerToRemove;

  this._events[name] = this._events[name].filter(filterListeners);
};

EventEmitter.prototype.emit = function(name, data) {
  if (!this._events[name]) {
    throw new Error(`Can't emit an event. Event "${name}" doesn't exits.`);
  }

  const fireCallbacks = (callback) => {
    callback(data);
  };

  this._events[name].forEach(fireCallbacks);
};



var events = new EventEmitter();