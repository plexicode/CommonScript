let newEngineContextBuilder = (name, ver) => {
  let extensions = {};
  let registerExtension = (name, fn) => {
    extensions[name] = fn;
    return o;
  };

  let lockConfiguration = () => {
    let finalizedExtensions = Object.freeze({ ...extensions });

    let buildRuntimeTask = (ec, taskObj) => {
      return Object.freeze({
        resume: () => {
          let resultRaw = RunInterpreter(taskObj);
          return buildTaskResult(resultRaw);
        },
      })
    };

    let buildTaskResult = rawResult => {
      let type = PUBLIC_getTaskResultStatus(rawResult);
      let err = PUBLIC_getTaskResultError(rawResult);
      let sleepAmt = PUBLIC_getTaskResultSleepAmount(rawResult);
      return Object.freeze({
        isError: () => type === 2,
        isSuccess: () => type === 1,
        isSuspend: () => type === 3,
        isTimedSleep: () => type === 4,
        getSleepAmountMillis: () => type === 4 ? sleepAmt : null,
        getError: () => err ? [...err] : null,
      });
    };

    let engine = {
      createAdaptiveCompilation: (rootModuleId) => {
        throw new Error("Not implemented");
      },
      createRuntimeContext: (byteArrOrBase64, cliArgs, appCtx) => {
        if (typeof byteArrOrBase64 === 'string') {
          throw new Error("base64 Not implemented");
        }
        if (!(byteArrOrBase64 instanceof Uint8Array)) {
          throw new Error("Invalid byte code input.");
        }

        let byteCodeBytes = byteArrOrBase64;

        let execCtx = PUBLIC_initializeExecutionContext(byteCodeBytes, finalizedExtensions, appCtx);
        let initErr = PUBLIC_getExecutionContextError(execCtx);
        if (initErr) throw new Error("Could not initialized executable: " + initErr);
        let args = (cliArgs || []).map(v => `${v}`);
        let mainTaskRaw = createMainTask(execCtx, args);
        let mainTask = buildRuntimeTask(execCtx, mainTaskRaw);

        return Object.freeze({
          getMainTask: () => mainTask,
          startMainTask: () => mainTask.resume(),
        });
      },
    };

    let o = Object.freeze({ ...engine });
    return o;
  };

  let o = Object.freeze({
    registerExtension,
    lockConfiguration,
  });
  return o;
};
