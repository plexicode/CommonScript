const CommonScriptWidgetBuilderBuilder = (() => {
  // #INCLUDE-INDENT ../../compiler/src-intermediate/lib.js
  // #INCLUDE-INDENT ../../runtime/src-intermediate/lib.js

  let newBuilder = (languageId, version) => {
    let onPrintHandlers = [];
    let extensionFunctionsByName = {};
    let moduleFilesById = {};

    let onPrint = fn => {
      onPrintHandlers.push(fn);
      return builder;
    };

    let addExtensions = (idsToFunctions) => {
      Object.keys(idsToFunctions).map(id => addExtension(id, idsToFunctions[id]));
      return builder;
    };

    let addExtension = (id, fn) => {
      if (typeof fn !== 'function') throw new TypeError();
      extensionFunctionsByName[id] = fn;
      return builder;
    };

    let addModule = (name, moduleSourceFileLookup) => {
      moduleFilesById[name] = JSON.parse(JSON.stringify(moduleSourceFileLookup));
      return builder;
    };

    let build = () => {

      let extensions = {
        on_stdout: (task, valueConvertUtil, args) => {
          let str = valueConvertUtil.toReadableString(args[0]);
          onPrintHandlers.forEach(fn => fn(str));
          return null;
        },
        ...extensionFunctionsByName,
      };
      let wrappedExtensions = {};
      Object.keys(extensions).forEach(id => {
        let fn = extensions[id];
        wrappedExtensions[id] = (task, args) => {

        };
      })
      let compiler = createCommonScritpCompilationEngine(`${languageId}`, `${version}`, extensions);

    };

    let builder = {
      build,
      onPrint,
      addExtension,
      addExtensions,
      addModule,
    };

    return Object.freeze(builder);
  };
  return Object.freeze({
    newBuilder,
  });
})();
