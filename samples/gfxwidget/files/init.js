
window.addEventListener('load', () => {
    let ui = {
        display: document.getElementById('display-out'),
        runBtn: document.getElementById('run-btn'),
        resetBtn: document.getElementById('reset-btn'),
        textOut: document.getElementById('text-out'),
        editor: document.getElementById('editor'),
    };

    ui.editor.value = ORIGINAL_CODE;
    let canvasCtx = ui.display.getContext('2d');

    let clear = e => { while (e.firstChild) e.removeChild(e.firstChild); };

    let engine = createCommonScriptWebEnvironment('GfxWidget', 'HEAD')
        .registerRuntimeUrl('/web-bgworker.js')
        .addMainThreadExtension('screen_width', () => {
            return `${ui.display.width}`;
        })
        .addMainThreadExtension('screen_height', () => {
            return `${ui.display.height}`;
        })
        .addMainThreadExtension('draw_rectangle', args => {
            let [x, y, w, h, r, g, b] = args.split(',');
            canvasCtx.fillStyle = 'rgb(' + parseInt(r) + ',' + parseInt(g) + ',' + parseInt(b) + ')';
            canvasCtx.fillRect(parseInt(x), parseInt(y), parseInt(w), parseInt(h));
            return '';
        })
        .addMainThreadExtension('draw_ellipse', args => {
            let [x, y, w, h, r, g, b] = args.split(',').map(n => parseFloat(n));
            canvasCtx.fillStyle = 'rgb(' + r + ',' + g + ',' + b + ')';
            canvasCtx.beginPath();
            canvasCtx.ellipse(x + w / 2, y + h / 2, w / 2, h / 2, 0, 0, Math.PI * 2);
            canvasCtx.fill();
            return '';
        })
        .addModule('gfx', {
            'gfx.script': BUILTIN_GFX_LIB,
        })
        .onStdOut(line => {
            output.append(makeElement('div', line));
        })
        .build();

    let printStdOut = (msg, isError) => {
        let row = document.createElement('div');
        row.append('' + msg);
        if (isError) row.style.color = 'red';
        ui.textOut.append(row);
    };

    ui.runBtn.addEventListener('click', async () => {
        let code = editor.value;
        let result = engine.compile('main', { main: { 'main.script': code } });
        if (!result.ok) {
            clear(ui.textOut);
            printStdOut(result.errorMessage, true);
            return;
        }
        ui.runBtn.disabled = true;
        await engine.run(result);
        ui.runBtn.disabled = false;
    });
    ui.resetBtn.addEventListener('click', () => {
        ui.textOut.innerHTML = '';
        editor.value = ORIGINAL_CODE;
    });
});
