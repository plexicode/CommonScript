const CommonScriptCompiler = (() => {

  const PST = (() => {
    //%%%PASTEL_GENERATED%%%
    return {
      CompilerContext_new,
      PUBLIC_GetNextRequiredModuleId,
      PUBLIC_SupplyFilesForModule,
      PUBLIC_EnsureDependenciesFulfilled,
      PUBLIC_CompleteCompilation,
      registerExt: PASTEL_regCallback,
    };
  })();

  const IS_DEBUG = true;

  let constructTokenPrefix = tok => {
    if (!tok) return '';
    // Need getters for each field or just move this to generated code.
    return `[TODO: token unpacking]: `;
  };

  PST.registerExt('throwParserException', (args) => {
    let t = args[0];
    if (t === 1) throw new Error(constructTokenPrefix(t[1]) + t[2]);
    if (t === 2) throw new Error(`[${t[1]}] ${t[2]}`);
    if (t === 3) throw new Error(t[1])
    throw new Error();
  });

  let newCompilationEngine = (languageId, version, extensions) => {

    let extensionNames = [...extensions].map(v => '' + v);

    let createAdaptiveCompilation = (rootModuleId) => {

      // TODO: include error logic and catching in provideFiles and shortcircuit getCompilation result when applicable.
      let compiler = PST.CompilerContext_new(rootModuleId + '', languageId + '', version + '', [...extensionNames]);
      let nextModuleIdCache = null;
      let isDone = false;

      let updateNext = () => {
        nextModuleIdCache = PST.PUBLIC_GetNextRequiredModuleId(compiler);
        isDone = nextModuleIdCache === null;
      };
      updateNext();

      // TODO: why is isBuiltIn ignored?
      let provideFilesImpl = (nextModId, filesLookup, isBuiltIn) => {
        if (nextModId !== nextModuleIdCache) throw new Error('');
        PST.PUBLIC_SupplyFilesForModule(compiler, nextModId, { ...filesLookup }, false, false);
        updateNext();
      };

      let getCompilation = () => {
        if (!isDone) throw new Error('');
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
