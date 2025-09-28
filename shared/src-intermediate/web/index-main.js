const createCommonScriptWebEnvironment = (langName, langVer) => {

  // #INCLUDE-INDENT ../../../compiler/src-intermediate/lib.js

  const MSG_PREFIX = '#COMMONSCRIPT_MSG:';

  let runtimeUrl = null;
  let localExtensions = {};
  let bgExtensions = {};
  let modules = {};

  let o = null;

  let addMainThreadExtension = (id, fn) => {
    localExtensions[id] = fn;
    return o;
  };
  let registerBackgroundThreadExtension = (id) => {
    bgExtensions[id] = true;
    return o;
  };

  let deepCopy = v => JSON.parse(JSON.stringify(v));

  let addModule = (name, filesByName) => {
    modules[name] = deepCopy(filesByName);
    return o;
  };

  let stdOutHandlers = [];
  let onStdOut = fn => {
    stdOutHandlers.push(fn);
    return o;
  };

  let build = () => {
    let extensions = [...Object.keys(localExtensions), ...Object.keys(bgExtensions)];
    let engine = createCommonScriptCompilationEngine(langName, langVer, extensions);

    return Object.freeze({
      compile: (mainModuleName, files) => {
        let result = engine.doStaticCompilation(mainModuleName, deepCopy(files), modules);
        if (result.byteCodePayload) return { ok: true, byteCode: new Uint8Array(result.byteCodePayload) };
        return { ok: false, errorMessage: result.errorMessage };
      },
      run: async compilation => {
        if (!compilation.ok || !compilation.byteCode) throw new Error("Invalid compilation");
        let byteCodeHexBuf = [];
        let bc = compilation.byteCode;
        let bcLen = bc.length;
        let hex = '0123456789ABCDEF'.split('');
        let c;
        for (let i = 0; i < bcLen; i++) {
          c = bc[i];
          byteCodeHexBuf[i] = hex[c >> 4] + hex[c & 15];
        }
        let byteCodeHex = byteCodeHexBuf.join('');
        let worker = new Worker(runtimeUrl, { })
        worker.addEventListener('message', e => {
          let data = e.data;
          if (!data.startsWith(MSG_PREFIX)) return;
          let colonIdx = data.indexOf(':', MSG_PREFIX.length);
          if (colonIdx === -1) return;
          let payload = data.substring(colonIdx + 1);
          let [ msgType, extId, msgId ] = data.substring(MSG_PREFIX.length, colonIdx).split(',');
          switch (msgType) {
            case 'INIT':
              if (payload !== 'ready') throw new Error();
              worker.postMessage(MSG_PREFIX + 'FINALIZE:' + JSON.stringify({
                extensions: {
                  blocking: extensions,
                  nonBlocking: [],
                },
                byteCodeHex,
                cliArgs: [],
              }));
              break;

            case 'REQ':
              let ext = localExtensions[extId];
              if (!ext) throw new Error("Unrecognized extension invocation: " + extId);
              let result = ext(payload);
              let responsePrefix = MSG_PREFIX + 'RES,' + msgId + ':';
              if (result instanceof Promise) {
                result.then(resultAwaited => {
                  worker.postMessage(responsePrefix + resultAwaited);
                });
              } else {
                worker.postMessage(responsePrefix + result);
              }
              break;

            case 'MSG':
              throw new Error();

            case 'STDOUT':
              stdOutHandlers.forEach(fn => {
                fn(payload);
              });
              break;
          }
        });
      },
    });
  };
  o = Object.freeze({
    registerRuntimeUrl: url => { runtimeUrl = url; return o; },
    addMainThreadExtension,
    registerBackgroundThreadExtension,
    addModule,
    onStdOut,
    build,
  });
  return o;
};
