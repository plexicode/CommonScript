import compilerEngineFactory from './GEN-compiler.js';
import createNewCommonScriptRuntimeFactory from './GEN-runtime.js';

const LANGUAGE_NAME = 'TestScript';
const LANGUAGE_VER = 'HEAD';

let deepCopy = value => JSON.parse(JSON.stringify(value));

export const compile = (binaryName, filesByName, moduleCodeByFilenameByModuleId) => {
    let factory = compilerEngineFactory(LANGUAGE_NAME, LANGUAGE_VER, []);
    let builtinCodeFilesByNameByModuleId = {};
    let modules = deepCopy(moduleCodeByFilenameByModuleId || {});

    modules[binaryName] = deepCopy(filesByName);
    let output = factory.doStaticCompilation(binaryName, modules, builtinCodeFilesByNameByModuleId);
    if (output.errorMessage) {
        return {
            ok: false,
            error: output.errorMessage,
        };
    }

    return { ok: true, byteCode: new Uint8Array(output.byteCodePayload) };
};

let runtimeFactory =
    createNewCommonScriptRuntimeFactory(LANGUAGE_NAME, LANGUAGE_VER)
        .addOnStdOutHandler(line => console.log(line))
        .lockConfigurationAndGetFinalizedRuntimeFactory();

export const run = async (byteCode) => {
    await runtimeFactory
        .createNewRuntimeInstance(byteCode, [], {})
        .start();
};
