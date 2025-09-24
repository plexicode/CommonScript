import compilerEngineFactory from './GEN-compiler.js';
import runtime from './GEN-runtime.js';

export const compile = (binaryName, filesByName, moduleCodeByFilenameByModuleId) => {
    let factory = compilerEngineFactory('TestScript', 'HEAD', []);
    let builtinCodeFilesByNameByModuleId = {};
    let modules = JSON.parse(JSON.stringify(moduleCodeByFilenameByModuleId || {}));
    modules[binaryName + 'Test'] = JSON.parse(JSON.stringify(filesByName));
    let output = factory.doStaticCompilation(binaryName + 'Test', modules, builtinCodeFilesByNameByModuleId);
    if (output.errorMessage) {
        return {
            ok: false,
            error: output.errorMessage,
        };
    }

    return { ok: true, byteCode: new Uint8Array(output.byteCodePayload) };
};

let engine = null;
let getEngine = () => {
    if (engine) return engine;
    let engineBuilder = runtime.newEngineContextBuilder('TestScript', 'HEAD');

    engineBuilder.registerExtension('io_stdout', (task, argsArray) => {
        let str = runtime.runtimeValueConverter.toReadableString(argsArray[0]);
        console.log(str);
        return null;
    });

    engineBuilder.registerExtension('sleep', (task, argsArray) => {

        let durationSec = runtime.runtimeValueConverter.unwrapFloat(argsArray[0]);
        let durationMillis = Math.floor(durationSec * 1000);
        runtime.task.sleepTask(task, durationMillis);
        return null;
    });

    engineBuilder.registerExtension('delay_invoke', (task, argsArray) => {
        let fnValue = argsArray[0];
        let durationSec = runtime.runtimeValueConverter.unwrapFloat(argsArray[1]);
        let durationMillis = Math.floor(durationSec * 1000);
        setTimeout(() => {
            runtime.task.invokeFunction(task, fnValue, []);
        }, durationMillis);
        return null;
    });

    engine = engineBuilder.lockConfiguration();

    return engine;
};


let pause = async millis => new Promise(res => { setTimeout(() => res(true), millis); });

let runTask = async task => {
    let result = await task.resume();

    while (true) {
        if (result.isSuccess()) return;
        if (result.isError()) return;

        if (result.isTimedSleep()) {
            await pause(result.getSleepAmountMillis());
            result = await task.resume();
        }

        if (result.isSuspend()) {
            throw new Error('NYI');
        }
    }
};

export const run = async (byteCode) => {
    let rt = getEngine();
    let appCtx = {};
    let rtCtx = rt.createRuntimeContext(byteCode, [], appCtx);

    await runTask(rtCtx.getMainTask());
};
