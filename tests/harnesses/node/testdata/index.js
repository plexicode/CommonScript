import * as CASES from './GEN-cases.js';

let ignoredTests = new Set([

    // JSON: The parser/serializer is done as a platform-specific extension and is only done for C#.
    // For the parser this makes some sense. However, the serializer should be 100% pastel.
    'json',

    // Uses JSON (see above)
    'builtinlibs',

    // I am so tired.
    'xml',
]);

let dataRaw = CASES.getTestData();
let testCaseIds = new Set(dataRaw.testCases);
let allIdsSorted = [...testCaseIds].filter(v => v !== 'smoke');
allIdsSorted.sort();
allIdsSorted = ['smoke', ...allIdsSorted];
if (allIdsSorted.length !== dataRaw.testCases.length) throw new Error();
allIdsSorted = allIdsSorted.filter(tc => !ignoredTests.has(tc));

let builtinLibs = {
    'testlib': {
        'testlib.script': dataRaw.files.filter(f => f.path === 'testlib/testlib.script')[0].content,
    }
};
let deepCopy = o => JSON.parse(JSON.stringify(o));

let testCases = {};

dataRaw.files.filter(f => f.path.endsWith('.config')).forEach(f => {
    let pathParts = f.path.split('/');
    let testCaseId = pathParts[0];
    let info = {
        id: testCaseId,
        modules: deepCopy(builtinLibs),
        mainModule: null,
        isSimpleMain: null,
    };
    testCases[testCaseId] = info;

    f.content.split('\n')
        .map(line => line.trim())
        .filter(line => !!line)
        .map(line => line.split(':').map(v => v.trim()))
        .filter(attrs => attrs[0] !== 'MODULE' || attrs[1] !== 'testlib') // already accounted for
        .forEach(attrs => {
            switch (attrs[0]) {
                case 'MAIN':
                    info.mainModule = attrs[1];
                    info.isSimpleMain = info.mainModule === '.';
                    break;
                case 'MODULE':
                    let modId = attrs[1];
                    let dir = attrs[2];
                    if (dir !== './' + modId) throw new Error(testCaseId + ": Module location is weird");
                    info.modules[attrs[1]] = {}
                    break;
                default: throw new Error(testCaseId + " test config has an error");
            }
        });
});

dataRaw.files
    .filter(f => f.path.endsWith('.script') && testCaseIds.has(f.path.split('/')[0]))
    .map(f => {
        let parts = f.path.split('/');
        let testCaseId = parts[0];
        let testCase = testCases[testCaseId];
        if (testCase.isSimpleMain) {
            return { testCaseId, moduleId: 'main', path: parts.slice(1).join('/'), content: f.content };
        }
        return { testCaseId, moduleId: parts[1], path: parts.slice(2).join('/'), content: f.content };
    })
    .forEach(file => {
        let info = testCases[file.testCaseId];
        if (!info.modules[file.moduleId]) info.modules[file.moduleId] = {};
        info.modules[file.moduleId][file.path] = file.content;
    });

export function getTestCaseInfo(testCaseId) {
    let info = testCases[testCaseId];
    if (!info) throw new Error("No testcase by id: " + testCaseId);
    return deepCopy(info);
}

export function getTestCaseIds() {
    return [...allIdsSorted];
}
