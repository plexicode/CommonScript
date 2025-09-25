const CommonScriptRuntime = (() => {
  // #INCLUDE ./GEN-version.js

  // #INCLUDE ./GEN-pastel.js
  PASTEL_regCallback('hardCrash', args => { throw new Error(args[0]); });

  PASTEL_regCallback('jsonSerialize', args => {
    debugger;
    console.log(args);
    let [obj, useIndent] = args;
    
    throw new Error('waaaat');
  });

  // #INCLUDE ./newEngineContextBuilder.js

  return Object.freeze({
    newEngineContextBuilder,
    task: Object.freeze({
      suspendTask: task => { PUBLIC_requestTaskSuspension(task, false, 0); },
      sleepTask: (task, millis) => { PUBLIC_requestTaskSuspension(task, true, millis); },
      invokeFunction: (anyTaskRef, fn, wrappedArgs) => {
        let ec = PUBLIC_getExecutionContextFromTask(anyTaskRef);
        let newTask = PUBLIC_createTaskForFunctionWithWrappedArgs(ec, fn, wrappedArgs);
        PUBLIC_resumeTask(newTask);
      },
    }),
    runtimeValueConverter: Object.freeze({
      toReadableString: PUBLIC_valueToString,
      wrapNativeHandle: PUBLIC_wrapNativeHandle,
      wrapBoolean: PUBLIC_wrapBoolean,
      wrapInteger: PUBLIC_wrapInteger,
      wrapString: PUBLIC_wrapString,
      unwrapNativeHandle: PUBLIC_unwrapNativeHandle,
      unwrapInteger: PUBLIC_unwrapInteger,
      unwrapFloat: PUBLIC_unwrapFloat,
      unwrapAppContext: PUBLIC_getApplicationContextFromTask,
      listAdd: PUBLIC_listValueAdd,
      listLength: PUBLIC_listLength,
      listGet: PUBLIC_listGet,
      listSet: PUBLIC_listSet,
    }),
  });
})();
