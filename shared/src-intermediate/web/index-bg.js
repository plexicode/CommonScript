const CommonScriptBgRunner = (langName, langVer) => {

  // #INCLUDE-INDENT ../../../runtime/src-intermediate/lib.js

  const MSG_PREFIX = '#COMMONSCRIPT_MSG:';

  let o = null;

  let extensions = {};
  let responseCallbacks = {};
  let nextCallbackId = 1;


  let isLocked = false;
  let ensureNotLocked = () => { if (isLocked) throw new Error(); };

  let onRoundtripStart = async (mainThreadArgs) => {

    let builder = createNewCommonScriptRuntimeFactory(langName, langVer)
      .addOnStdOutHandler(line => {});

    Object.keys(extensions).forEach(extId => {
      builder.registerExtension(extId, inlineExtensions[extId]);
    });

    let registerMainThreadBlockingExtension = (id) => {
      let prefix = MSG_PREFIX + 'REQ,' + id + ',';
      builder.registerExtension(id, (task, utils, args) => {
        let cbId = 'R' + nextCallbackId++;
        responseCallbacks[cbId] = nativeStr => {
          let strValue = utils.valueConverter.wrapString(nativeStr);
          utils.task.setStackTop(task, strValue);
          utils.task.resumeTask(task);
        };
        utils.task.requestTaskSuspension(task);
        let arg = args.length
          ? utils.valueConverter.valueToReadableString(args[0])
          : '';
        let msg = prefix + cbId + ':' + arg;
        postMessage(msg);
      });
    };

    let registerMainThreadNonBlockingExtension = (id) => {
      let prefix = MSG_PREFIX + 'MSG,' + id + ':';
      builder.registerExtension(id, (task, utils, args) => {
        let arg = args.length
          ? utils.valueConverter.valueToReadableString(args[0])
          : '';
        let msg = prefix + arg;
        postMessage(msg);
      });
    };

    let remoteExt = mainThreadArgs.extensions || {};
    (remoteExt.blocking || []).forEach(extId => {
      registerMainThreadBlockingExtension(extId);
    });
    (remoteExt.nonBlocking || []).forEach(extId => {
      registerMainThreadNonBlockingExtension(extId);
    });

    let byteCode = [];
    let byteCodeHex = mainThreadArgs.byteCodeHex;
    let bcLen = byteCodeHex.length;
    for (let i = 0; i < bcLen; i += 2) {
      byteCode.push(parseInt(byteCodeHex.substring(i, i + 2), 16));
    }

    let factory = builder.lockConfigurationAndGetFinalizedRuntimeFactory();
    let rt = factory.createNewRuntimeInstance(new Uint8Array(byteCode), mainThreadArgs.cliArgs, {});
    await rt.start();
    postMessage(MSG_PREFIX + 'END:');
  };

  let start = () => {
    ensureNotLocked();
    isLocked = true;
    addEventListener('message', e => {
      let rawData = e.data + '';
      if (!rawData.startsWith(MSG_PREFIX)) return;
      let colonIdx = rawData.indexOf(':', MSG_PREFIX.length);
      if (colonIdx === -1) return;
      let sideChannel = rawData.substring(MSG_PREFIX.length, colonIdx).split(',');
      let payload = rawData.substring(colonIdx + 1);
      let [msgType, msgId] = sideChannel;
      switch (msgType) {
        case 'REQ':
          throw new Error("Not implemented");
        case 'RES':
          let cb = responseCallbacks[msgId];
          if (cb) {
            cb(payload);
            delete responseCallbacks[msgId];
          }
          break;
        case 'FINALIZE':
          onRoundtripStart(JSON.parse(payload));
          break;
        default:
          throw new Error();
      }
    });

    postMessage(MSG_PREFIX + 'INIT:ready');
  };

  let registerBackgroundWorkerExtension = (id, fn) => {
    ensureNotLocked();
    extensions[id] = fn;
    return o;
  };

  o = Object.freeze({
    start,
    registerBackgroundWorkerExtension,
  });

  return o;
};
