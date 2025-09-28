const createNewCommonScriptRuntimeFactory = (() => {
  // #INCLUDE-INDENT ./GEN-version.js

  // #INCLUDE-INDENT ./GEN-pastel.js
  PASTEL_regCallback('hardCrash', args => { throw new Error(args[0]); });

  // #INCLUDE-INDENT ./jsonUtil.js
  PASTEL_regCallback('jsonParse', args => {
    let [str, bufOut] = args;
    jsonUtil_parseToArray(str, bufOut);
    return null;
  });

  return (langName, langVer) => {
    let fingerprint = langName + ':' + langVer;
    let isLocked = false;
    let ensureNotLocked = () => { if (isLocked) throw new Error("Configuration Locked!"); };

    let extensions = {};
    let stdoutHandlers = [];
    let o = null;

    let registerExtension = (name, fn) => {
      ensureNotLocked();
      if (typeof fn !== 'function') throw new TypeError("Not a function");
      if (fn.length !== 3) throw new Error("Extension function needs exactly 3 arguments.");
      extensions[name] = fn;
      return o;
    };

    let addOnStdOutHandler = fn => {
      ensureNotLocked();
      if (typeof fn !== 'function') throw new TypeError("Not a function");
      stdoutHandlers.push(line => fn(line));
      return o;
    };

    registerExtension('io_stdout', (_, utils, args) => {
      let str = utils.valueConverter.valueToReadableString(args[0]);
      stdoutHandlers.forEach(fn => fn(str));
    });

    registerExtension('sleep', (task, utils, args) => {
      let durationSec = utils.valueConverter.unwrapFloat(args[0]);
      let durationMillis = Math.floor(durationSec * 1000);
      PUBLIC_requestTaskSuspension(task, true, durationMillis);
    });

    registerExtension('delay_invoke', (task, utils, args) => {
      let fnValue = args[0];
      let durationSec = utils.valueConverter.unwrapFloat(args[1]);
      let durationMillis = Math.floor(durationSec * 1000);
      // TODO: execution context must be alerted that a delay invoke is active
      // so as to not declare the process over if no active tasks are present.
      setTimeout(() => {
        utils.task.launchFunctionAsTask(fnValue, []);
      }, durationMillis);
    });

    let lockConfigurationAndGetFinalizedRuntimeFactory = () => {
      ensureNotLocked();
      let finalizedExtensions = Object.freeze({ ...extensions });

      let pause = async millis => new Promise(r => {
        setTimeout(() => r(true), millis);
      });

      let runTaskAsync = async (taskObjRaw) => {
        while (true) {
          let resultRaw = PUBLIC_resumeTask(taskObjRaw);
          switch (PUBLIC_getTaskResultStatus(resultRaw)) {
            case 1: return { ok: true  };
            case 2: return { ok: false, error: PUBLIC_getTaskResultError(resultRaw) };
            case 3: return new Promise(res => {
              throw new Error("Need to store a promise/resolver on the raw task and return it here.");
            });
            case 4:
              await pause(PUBLIC_getTaskResultSleepAmount(resultRaw));
              break;
            default: throw new Error();
          }
        }
      };

      let ensureByteCodeIsByteArray = byteArrOrBase64 => {
        if (typeof byteArrOrBase64 === 'string') {
          throw new Error("base64 Not implemented");
        }
        if (!(byteArrOrBase64 instanceof Uint8Array)) {
          throw new Error("Invalid byte code input.");
        }
        return byteArrOrBase64;
      };

      let createNewRuntimeInstance = (byteArrOrBase64, cliArgs, appCtx) => {
        let byteCodeBytes = ensureByteCodeIsByteArray(byteArrOrBase64);

        let utils = {
          valueConverter: Object.freeze({
            valueToReadableString: PUBLIC_valueToString,
            wrapNativeHandle: PUBLIC_wrapNativeHandle,
            wrapBoolean: b => PUBLIC_wrapBoolean(mainTaskRaw, !!b),
            wrapInteger: i => PUBLIC_wrapInteger(mainTaskRaw, i),
            wrapString: s => PUBLIC_wrapString(mainTaskRaw, s, false),
            unwrapNativeHandle: PUBLIC_unwrapNativeHandle,
            unwrapInteger: PUBLIC_unwrapInteger,
            unwrapFloat: PUBLIC_unwrapFloat,
          }),
          collections: Object.freeze({
            listAdd: PUBLIC_listValueAdd,
            listLength: PUBLIC_listLength,
            listGet: PUBLIC_listGet,
            listSet: PUBLIC_listSet,
          }),
          task: Object.freeze({
            requestTaskSuspension: task => { PUBLIC_requestTaskSuspension(task, false, 0); return null; },
            launchFunctionAsTask: async (fn, wrappedArgs) => {
              let newTask = PUBLIC_createTaskForFunctionWithWrappedArgs(execCtx, fn, wrappedArgs);
              return runTaskAsync(newTask);
            },
            resumeTask: task => {
              PUBLIC_resumeTask(task);
              return null;
            },
            setStackTop: (task, valueWrapped) => {
              PUBLIC_setTaskStackTopValue(task, valueWrapped);
            },
          }),
        };

        let boundExtensions = {};
        Object.keys(finalizedExtensions).forEach(id => {
          let fn = finalizedExtensions[id];
          boundExtensions[id] = (task, args) => fn(task, utils, args) ?? null;
        });

        let execCtx = PUBLIC_initializeExecutionContext(byteCodeBytes, boundExtensions, appCtx, () => { allFinishedResolver(true); });
        let initErr = PUBLIC_getExecutionContextError(execCtx);
        if (initErr) throw new Error("Could not initialized executable: " + initErr);
        let args = (cliArgs || []).map(v => `${v}`);
        let mainTaskRaw = createMainTask(execCtx, args);

        let allFinishedResolver;
        let allFinishedPromise = new Promise(r => { allFinishedResolver = r; });

        let isStarted = false;
        return Object.freeze({
          start: async () => {
            if (isStarted)  throw new Error("runtime process already started");
            isStarted = true;
            await runTaskAsync(mainTaskRaw);
            await allFinishedPromise;
            // What to return?
          },
        });
      };

      return Object.freeze({ createNewRuntimeInstance });
    };

    o = Object.freeze({
      registerExtension,
      addOnStdOutHandler,
      lockConfigurationAndGetFinalizedRuntimeFactory,
    });
    return o;
  };

})();
