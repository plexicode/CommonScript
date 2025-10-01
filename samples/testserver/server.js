const PORT = 8084;

import express from 'express';
import fs from 'fs';

let readFile = async path => new Promise(res => {
    fs.readFile('../../dist/web-bgworker.js', { encoding: 'utf-8' }, (err, data) => {
        if (err) throw new Error(err);
        res(data);
    });
});

let app = express();

app.use('/web-bgworker.js', async (req, res) => {
    let code = [
        // Keep the bgworker constructor simple, wrap the dist web-worker JS file and
        // the initializer line into one, as the end-user should use a flat file as well.
        await readFile('../.../dist/web-bgworker.js'),
        'CommonScriptBgRunner("GfxWidget", "HEAD").start();',
        ''
    ].join('\n');

    res.setHeader('Content-Type', 'text/javascript');
    res.send(code);
});
app.use('/dist', express.static('../../dist'));
app.use('/gfxwidget', express.static('../gfxwidget'));
app.get('/', (req, res) => {
    res.send([
        '<!DOCTYPE html>',
        '<html>',
        '<head>',
        '<meta charset="utf-8">',
        '<title>Demos</title>',
        '</head>',
        '<body>',
        '<h1>CommonScript Web-based Demos</h1>',
        ...[
            { name: "GFX Widget", link: '/gfxwidget' },
        ].map(info => {
            return '<div><a href="' + info.link + '">' + info.name + '</a></div>';
        }),
        '</body>',
        '</html>',
    ].join('\n'));
});

app.listen(PORT, () => {
    console.log("GfxWidget server running on port " + PORT);
});
