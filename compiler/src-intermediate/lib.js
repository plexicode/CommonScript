const createCommonScriptCompilationEngine = (() => {
  // #INCLUDE-INDENT ./GEN-version.js

  const PST = (() => {
    // #INCLUDE-INDENT ./GEN-pastel.js
    return {
      CompilerContext_new,
      PUBLIC_GetNextRequiredModuleId,
      PUBLIC_SupplyFilesForModule,
      PUBLIC_EnsureDependenciesFulfilled,
      PUBLIC_CompleteCompilation,
      PUBLIC_getTokenErrPrefix,
      PUBLIC_buildVerifiedImageResourceDescriptor,
      PUBLIC_base64ToBytes,
      registerExt: PASTEL_regCallback,
    };
  })();

  PST.registerExt('throwParserException', (args) => {
    // TODO: this should just receive a string instead of making a distinction here to assemble the error message.
    let [n, a, b] = args;
    if (n === 1) throw new Error(PST.PUBLIC_getTokenErrPrefix(a) + b);
    if (n === 2) throw new Error(`[${a}] ${b}`);
    if (n === 3) throw new Error(a)
    throw new Error();
  });

  PST.registerExt('hardCrash', args => {
    throw new Error(args[0]);
  });

  PST.registerExt('imageHandleToIntArrayOfBytes', args => {
    let canvas = args[0];
    let format = args[1];
    let b64 = canvas.toDataURL('image/' + (format === 'JPEG' ? 'jpeg' : 'png')).split(',').pop();
    let byteArr = PST.PUBLIC_base64ToBytes(b64);
    return byteArr;
  });

  let newCompilationEngine = (languageId, version, extensions) => {

    let extensionNames = [...extensions].map(v => '' + v);

    let createAdaptiveCompilation = (rootModuleId) => {

      // TODO: include error logic and catching in provideFiles and shortcircuit getCompilation result when applicable.
      let compiler = PST.CompilerContext_new(rootModuleId + '', languageId + '', version + '', [...extensionNames]);
      let nextModuleIdCache = null;
      let isDone = false;
      let errorOverride = null;

      let updateNext = () => {
        nextModuleIdCache = PST.PUBLIC_GetNextRequiredModuleId(compiler);
        isDone = nextModuleIdCache === null;
      };
      updateNext();

      // TODO: why is isBuiltIn ignored?
      let provideFilesImpl = (nextModId, filesLookup, isBuiltIn, optTextResources, optBinResources, optImgResources) => {
        if (nextModId !== nextModuleIdCache) throw new Error('');

        // #IF IS_DEBUG
        PST.PUBLIC_SupplyFilesForModule(compiler, nextModId, { ...filesLookup }, false, false, optTextResources, optBinResources, optImgResources);
        // #ELSE
        try {
          PST.PUBLIC_SupplyFilesForModule(compiler, nextModId, { ...filesLookup }, false, false, optTextResources, optBinResources, optImgResources);
        } catch (ex) {
          isDone = true;
          errorOverride = ex.message;
          return;
        }
        // #ENDIF

        updateNext();
      };

      let getCompilation = () => {
        if (!isDone) throw new Error('');
        if (errorOverride) return { errorMessage: errorOverride };

        PST.PUBLIC_EnsureDependenciesFulfilled(compiler);
        let bytes = PST.PUBLIC_CompleteCompilation(compiler);
        return { byteCodePayload: bytes };
      };

      return Object.freeze({
        isComplete: () => isDone,
        getNextRequiredModule: () => nextModuleIdCache,
        provideFilesForUserModuleCompilation: (nextModId, filesLookup, txtResLookup, binResLookup, imgResLookup) => provideFilesImpl(nextModId, filesLookup, false, txtResLookup || null, binResLookup || null, imgResLookup || null),
        provideFilesForBuiltinLibraryModuleCompilation: (nextModId, filesLookup, txtResLookup, binResLookup, imgResLookup) => provideFilesImpl(nextModId, filesLookup, true, txtResLookup || null, binResLookup || null, imgResLookup || null),
        getCompilation: () => {
          // #IF IS_DEBUG
          return getCompilation();
          // #ELSE
          try {
            return getCompilation();
          } catch (ex) {
            return { errorMessage: ex.message };
          }
          // #ENDIF
        },
      });
    };

    let sortOutResources = (lookupByModId, allModIds) => {

      let textLookupByModId = {};
      let binLookupByModId = {};
      let imgLookupByModId = {};

      allModIds.forEach(modId => {
        let txt = {};
        let bin = {};
        let img = {};
        textLookupByModId[modId] = txt;
        binLookupByModId[modId] = bin;
        imgLookupByModId[modId] = img;
        let resources = lookupByModId[modId] || {};
        Object.keys(resources).forEach(path => {
          let value = resources[path];
          if (typeof value === 'string') {
            txt[path] = value;
            return;
          }

          if (value && typeof value === 'object') {
            // TODO: this needs to be made generic/extensible to work in non-browser environments.
            // However the canvas approach should be default when such configuration is not explicitly set.
            if (value instanceof HTMLCanvasElement) {

              // Since it's already a valid canvas, treat anything without a JPEG cue as a PNG
              let t = path.split('/').pop().split('.');
              let ext = t.length > 1 ? t.pop().toUpperCase() : '';
              let format = ext === 'JPEG' || ext === 'JPG' ? 'JPEG' : 'PNG';

              img[path] = PST.PUBLIC_buildVerifiedImageResourceDescriptor(format, value.width, value.height, value);
              return;
            }

            if (value instanceof Uint8Array) {
              let arr = [];
              let len = value.length;
              for (let i = 0; i < len; i++) arr.push(value[i] | 0);
              bin[path] = arr;
              return;
            }
          }
          throw new Error(
            // #IF IS_DEBUG
            `Invalid resource provided by implementing scripting language in module '${modId}': ${path}` +
            // #ELSE
            '' +
            // #ENDIF
            ''
          );
        });
      });
      return {
        txtRes: textLookupByModId,
        binRes: binLookupByModId,
        imgRes: imgLookupByModId,
      };
    };

    let doStaticCompilation = (moduleId, userCodeFilesByModuleId, builtinCodeFilesByModuleId, optResourceLookupByModuleId) => {
      let adcomp = createAdaptiveCompilation(moduleId);
      let userCode = userCodeFilesByModuleId || {};
      let builtinCode = builtinCodeFilesByModuleId || {};
      let allModuleIds = [...new Set([...Object.keys(userCode), ...Object.keys(builtinCode)])];
      let { txtRes, binRes, imgRes } = sortOutResources(optResourceLookupByModuleId || {}, allModuleIds);
      while (!adcomp.isComplete()) {
        let nextModId = adcomp.getNextRequiredModule();
        let sources = [userCode, builtinCode];
        let moduleFound = false;
        for (let s = 0; s < 2; s++) {
          let isUserCode = s === 0;
          let files = sources[s][nextModId];
          if (files) {
            moduleFound = true;
            let doComp = () => {
              let comp = isUserCode
                ? adcomp.provideFilesForUserModuleCompilation
                : adcomp.provideFilesForBuiltinLibraryModuleCompilation;
              comp(nextModId, files, txtRes[nextModId] || {}, binRes[nextModId] || {}, imgRes[nextModId] || {});
            };
            // #IF IS_DEBUG
            doComp();
            // #ELSE
            try {
              doComp();
            } catch (ex) {
              return { errorMessage: ex.message };
            }
            // #ENDIF
          }
        }
      }
      let result = adcomp.getCompilation();
      return result;
    };

    return Object.freeze({
      createAdaptiveCompilation,
      doStaticCompilation,
    });
  };
  return newCompilationEngine;
})();
