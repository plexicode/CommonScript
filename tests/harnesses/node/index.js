import * as TestScript from './testscript/index.js';
import * as TestData from './testdata/index.js';

let runTestCase = async (testCaseId) => {
    let info = TestData.getTestCaseInfo(testCaseId);
    let modules = info.modules;
    let mainModule = modules.main;
    delete modules.main;

    let bundle = TestScript.compile(testCaseId, mainModule, modules);
    if (!bundle.ok) {
        console.log("ERROR IN TESTCASE: " + testCaseId);
        console.log(bundle.error);
        console.log("ERROR END");
        return;
    }

    await TestScript.run(bundle.byteCode);
};

let main = async () => {
    let testCaseIds = TestData.getTestCaseIds();
    for (let testCaseId of testCaseIds) {
        console.log(`[${testCaseId}] STARTING...`);
        await runTestCase(testCaseId);
        console.log(`[${testCaseId}] DONE`);
        console.log('-------------------------------------------------------');
    }
    console.log("Total tests: " + testCaseIds.length);
};

main();
