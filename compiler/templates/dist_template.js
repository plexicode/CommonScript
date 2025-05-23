const CommonScriptCompiler = (() => {

  const PST = (() => {
    //%%%PASTEL_GENERATED%%%
    return {
      CompilerContext_new,
      PUBLIC_GetNextRequiredModuleId,
      PUBLIC_SupplyFilesForModule,
      PUBLIC_EnsureDependenciesFulfilled,
      PUBLIC_CompleteCompilation,
      PUBLIC_getTokenErrPrefix,
      registerExt: PASTEL_regCallback,
    };
  })();

  const IS_DEBUG = false;

  PST.registerExt('throwParserException', (args) => {
    // TODO: this should just receive a string instead of making a distinction here to assemble the error message.
    let [n, a, b] = args;
    if (n === 1) throw new Error(PST.PUBLIC_getTokenErrPrefix(a) + b);
    if (n === 2) throw new Error(`[${a}] ${b}`);
    if (n === 3) throw new Error(a)
    throw new Error();
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
      let provideFilesImpl = (nextModId, filesLookup, isBuiltIn) => {
        if (nextModId !== nextModuleIdCache) throw new Error('');
        if (IS_DEBUG) {
          PST.PUBLIC_SupplyFilesForModule(compiler, nextModId, { ...filesLookup }, false, false);
        } else {
          try {
            PST.PUBLIC_SupplyFilesForModule(compiler, nextModId, { ...filesLookup }, false, false);
          } catch (ex) {
            isDone = true;
            errorOverride = ex.message;
            return;
          }
        }
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
        provideFilesForUserModuleCompilation: (nextModId, filesLookup) => provideFilesImpl(nextModId, filesLookup, false),
        provideFilesForBuiltinLibraryModuleCompilation: (nextModId, filesLookup) => provideFilesImpl(nextModId, filesLookup, true),
        getCompilation: () => {
          if (IS_DEBUG) return getCompilation();
          try {
            return getCompilation();
          } catch (ex) {
            return { errorMessage: ex.message };
          }
        },
      });
    };

    let doStaticCompilation = (moduleId, userCodeFilesByModuleId, builtinCodeFilesByModuleId) => {
      let adcomp = createAdaptiveCompilation(moduleId);
      let userCode = userCodeFilesByModuleId || {};
      let builtinCode = builtinCodeFilesByModuleId || {};
      while (!adcomp.isComplete()) {
        let nextModId = adcomp.getNextRequiredModule();
        let sources = [userCode, builtinCode];
        let moduleFound = false;
        for (let s = 0; s < 2; s++) {
          let isUserCode = s === 0;
          let files = sources[s][nextModId];
          if (files) {
            moduleFound = true;
            if (IS_DEBUG) {
              if (isUserCode) adcomp.provideFilesForUserModuleCompilation(nextModId, files);
              else adcomp.provideFilesForBuiltinLibraryModuleCompilation(nextModId, files);
            } else {
              try {
                if (isUserCode) adcomp.provideFilesForUserModuleCompilation(nextModId, files);
                else adcomp.provideFilesForBuiltinLibraryModuleCompilation(nextModId, files);
              } catch (ex) {
                return { errorMessage: ex.message };
              }
            }
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
