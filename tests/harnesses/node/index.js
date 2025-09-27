import * as TestScript from './testscript/index.js';
import * as TestData from './testdata/index.js';

let runTestCase = async (testCaseId) => {
    let info = TestData.getTestCaseInfo(testCaseId);
    let modules = info.modules;
    let mainModule = modules.main;
    delete modules.main;

    // Add "Test" suffix since test cases sometimes coincide with built-in
    // module names which causes problems when using the test case ID
    // directly as the binary name.
    let binaryName = testCaseId + 'Test';

    let bundle = TestScript.compile(binaryName, mainModule, modules);
    if (!bundle.ok) {
        console.log("ERROR IN TESTCASE: " + testCaseId);
        console.log(bundle.error);
        console.log("ERROR END");
        return false;
    }

    await TestScript.run(bundle.byteCode);
    return true;
};

let main = async () => {
    let testCaseIds = TestData.getTestCaseIds();
    let runs = 0;
    let passes = 0;
    for (let testCaseId of testCaseIds) {
        console.log(`[${testCaseId}] STARTING...`);
        let passed = await runTestCase(testCaseId);
        runs++;
        passes += passed ? 1 : 0;
        console.log(`[${testCaseId}] DONE`);
        console.log('-------------------------------------------------------');
    }
    console.log("Total tests: " + runs);
    console.log("Passes: " + passes);
    console.log("Fails: " + (runs - passes));
};

main();
