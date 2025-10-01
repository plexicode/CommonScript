(() => {

    // #COMMENT As the compiler and runtime are bundled together, bound their versions together
    // #INCLUDE-INDENT ../../../compiler/src-intermediate/GEN-version.js
    const LANG = 'DOMScript';
    const LANGVER = VER.join('.');

    // #INCLUDE-INDENT ../../../compiler/src-intermediate/lib.js
    // #INCLUDE-INDENT ../../../runtime/src-intermediate/lib.js

    window.addEventListener('load', () => {
        let allSyncSources = [];
        let allAsyncSources = [];
        [...document.querySelectorAll('script')].forEach((scriptTag, i) => {
            let scriptType = (scriptTag.type || '').split(';')[0].toLowerCase().trim();
            if (scriptType !== 'text/domscript') return;
            let n = allSyncSources.length + 1;
            if (scriptTag.src) {
                allAsyncSources.push({ tag: scriptTag, src: scriptTag.src });
            } else {
                allSyncSources.push({ tag: scriptTag, code: scriptTag.textContent, tagNum: n });
            }
        });

        let continueWithCompilation = (contentByFiles) => {
            const extensions = (() => {
                // #INCLUDE-INDENT ./extensions.js
                return EXT
            })();
            let eng = createCommonScriptCompilationEngine(LANG, LANGVER, Object.keys(extensions));
            // #INCLUDE-INDENT ./GEN-dom-module.js
            let compResult = eng.doStaticCompilation(
                'main',
                { main: contentByFiles },
                { dom: DOM_MODULE });
            if (compResult.errorMessage) throw new Error("DOMScript: " + compResult.errorMessage);
            let byteCode = new Uint8Array(compResult.byteCodePayload);

            let rtFactoryBuilder = createNewCommonScriptRuntimeFactory(LANG, LANGVER)
                .addOnStdOutHandler(line => console.log(line));
            Object.keys(extensions).forEach(extId => {
                rtFactoryBuilder.registerExtension(extId, extensions[extId]);
            });
            let rtFactory = rtFactoryBuilder.lockConfigurationAndGetFinalizedRuntimeFactory();
            let rt = rtFactory.createNewRuntimeInstance(byteCode, [], {});
            rt.start();
        };

        let files = {};
        allSyncSources.forEach(src => {
            files['inline-script-' + src.n + '.script'] = src.code;
        });

        if (allAsyncSources.length) {
            Promise.all(allAsyncSources.map(async scriptTag => {
                let httpResponse = await fetch(scriptTag.src);
                let textData = await httpResponse.text();
                let name = scriptTag.src;
                if (!name.toLowerCase().endsWith('.script')) name += '.script';
                files[name] = textData;
            })).then(() => {
                continueWithCompilation(files);
            });
        } else {
            continueWithCompilation(files);
        }
    });

    return Object.freeze({

    });
})();
