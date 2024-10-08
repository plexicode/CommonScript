const CommonScript = (() => {

  //%%%PASTEL_GENERATED%%%
  //%%%JS_WRAPPER%%%

  return Object.freeze({
    newEngineContextBuilder,
    task: Object.freeze({
        suspendTask: task => { PUBLIC_requestTaskSuspension(task, false, 0); },
        sleepTask: (task, millis) => { PUBLIC_requestTaskSuspension(task, true, millis); },
    }),
    runtimeValueConverter: Object.freeze({
      toReadableString: PUBLIC_valueToString,
      wrapNativeHandle: PUBLIC_wrapNativeHandle,
      wrapBoolean: PUBLIC_wrapBoolean,
      wrapInteger: PUBLIC_wrapInteger,
      wrapString: PUBLIC_wrapString,
      unwrapNativeHandle: PUBLIC_unwrapNativeHandle,
      unwrapInteger: PUBLIC_unwrapInteger,
      unwrapAppContext: PUBLIC_getApplicationContextFromTask,
      listAdd: PUBLIC_listValueAdd,
      // listLength: PUBLIC_listLength,
      // listGet: PUBLIC_listGet,
    }),
  });
})();
