# Extending CommonScript to embed coding widgets on websites

In this walkthrough, I’ll be creating a code editor widget that can be embedded
on a web page with the following features:

- A canvas element that you can programmatically draw to using a built-in *gfx*
  library
- A panel that displays standard output from print statements.
- End-user-written code runs in a background thread (web worker) such that
  infinite loops don’t lock up the browser.

If you’d like to see the final version, you can play with it
[here](https://plexi.io/commonscript/samples/gfxwidget).

> The goal of this walkthrough is configuring and using the CommonScript
> web-embed library rather than creating a rich code editor. As such, a
> minimally styled textarea element will be used for simplicity. However feel
> free to spice things up with pre-built editors such as
> [Ace](https://ace.c9.io/) in your own projects.

## `*-lib.js` vs `web-*.js`

For JavaScript there are a few options of libraries to use. The `runtime-lib.js`
and `compiler-lib.js` pair are direct wrappers around the compiler and runtime
and are very unopinionated in nature. The web library assumes you will be using
this in a web page, viewed in a browser, will have all the code available at
compile time, and will potentially run third-party-written code that could
contain problems such as infinite loops that could lock up the browser. As such,
it runs in a background Web Worker making it ideal for student environments.
This incurs slight performance disadvantage as the background and main threads
must communicate asynchronously if any interaction with the DOM is needed. If
you are extending CommonScript for first-party development for the web, you may
want to consider using the `compiler-lib.js` and `runtime-lib.js` libraries
directly.

## `web-embed.js`

The code in `web-embed.js` should be included in your page. It defines a single
function in the global scope called `createCommonScriptWebEnvironment` that
takes in two strings: your language name, and the version. This returns a
builder object with the following methods:

- **`registerRuntimeUrl(url)`**
  This is the URL that should be loaded to run the background worker. More on
  how this file should be set up in the following section.
- **`addMainThreadExtension(extensionId, fn)`**
  Adds an extension function that will run in the main thread along with its
  implementation. This should be used for things that can ONLY run in the main
  browser thread such as things that interact with DOM elements. Because this
  implementation is called from the background worker thread, the input
  arguments and return value are serialized to strings. As such, you can only
  have a single string as an argument value and a string as a return value. For
  sending complex information, the values must be serialized before being sent
  and deserialized upon receipt.
- **`registerBackgroundThreadExtension(extensionId)`**
  Adds an extension function that will run in the background thread. This is for
  the compiler to allow compilation of these functions. However, the actual
  implementation will be provided in `web-bgworker.js`.
- **`addModule(name, files)`**
  Adds a built-in module to your CommonScript extension. The name argument is
  the name by which it will be imported. The files argument is a
  string-to-string object that maps file names to file content.
- **`onStdOut(fn)`**
  The function you provide will be called when standard output is received from
  the worker thread (print statements). The function will take in a single
  string value.
- **`build()`**
  Returns a compilation engine with the provided configuration. Once invoked,
  the configuration cannot be changed without creating a new environment.

Once an environment is built with `.build()`, the resulting engine object has
two methods:

- **`.compile(mainModuleName, filesByModuleName)`**
  This will compile the given code using the configured environment. The
  compilation bundle object returned will have a boolean field called `ok`. If
  `true`, it can be passed to the `.run()` method. If `false`, it will have a
  string field called `errorMessage`.
- **`.run(compilationBundle)`**
  Runs the compilation bundle from the `.compile()` output in a background
  thread.

## `web-bgworker.js`

This code contains the background worker process. However, you must add code to
call and configure it. To include multiple files in a web worker you can either
concatenate the code together or use the
[importScripts()](https://developer.mozilla.org/en-US/docs/Web/API/WorkerGlobalScope/importScripts).

The `web-bgworker.js` adds a single function to the global scope in the
background thread called `CommonScriptBgRunner`. Similar to the foreground
thread, it takes in a language name and a version. These values must match the
main thread configuration otherwise it will be rejected by the runtime. The
output of this function is also a builder object with configuration methods:

- **`.registerBackgroundWorkerExtension(id, fn)`**
  This registers an extension method that can be run directly in the background
  thread. Background thread extensions do not need to be sent to the main thread
  and therefore run synchronously and quickly.
- **`.start()`**
  Starts execution of the runtime. You do NOT have to provide any other
  information such as the compilation bundle. The compiled payload will be sent
  to this thread via internal `postMessage` calls.

## Example: a canvas-drawing scripting environment

The code for this can be seen
[here](https://github.com/plexicode/CommonScript/blob/main/samples/gfxwidget/files/init.js).
I’ll be explaining key components of the GfxWidget implementation.

In the main thread code, we can create an environment builder for our graphics
widget by calling `createCommonScriptWebEnvironment`. Our language is called
`GfxWidget` and as we are not distributing built binaries and the runtime is
always coupled with the compiler, we don’t care about the version number, so we
simply name the version `HEAD`.

```javascript
let builder = createCommonScriptWebEnvironment('GfxWidget', 'HEAD');
```

Next we need to provide a URL to the background worker that the browser can load
when creating a Worker instance. For our server, this is just
`/web-bgworker.js`. Yours will likely be different.

```javascript
bulder.registerRuntimeUrl('/web-bgworker.js');
```

Next we need to start providing the extension methods that we will need to
interact with the DOM. For example, we’ll need to query what the current screen
width is. To do this, we create an extension function called `screen_width` and
provide the following implementation:

```javascript
builder.addMainThreadExtension('screen_width', () => {
  return `${ui.display.width}`;
});
```

Note that the return value must always be a string for extensions that run in
the main thread.

Another extension we’ll need is the ability to draw a rectangle to the canvas.
For this, we’ll add an extension called `draw_rectangle`.

```javascript
builder.addMainThreadExtension('draw_rectangle', args => {
  let [x, y, w, h, r, g, b] = args.split(',');
  canvasCtx.fillStyle = 'rgb(' + parseInt(r) + ',' + parseInt(g) + ',' + parseInt(b) + ')';
  canvasCtx.fillRect(parseInt(x), parseInt(y), parseInt(w), parseInt(h));
  return '';
});
```

Even though the name of our argument is `args`, the argument provided will
always be a single string. To send all 7 numeric arguments, we use a `.split`
on a comma and call `parseInt` on the results. For more advanced value
round-tripping one could use JSON serialization/parsing. Once we have these
values, we can draw the rectangle to the canvas.

The other extensions are configured similarly.

Next we provide the implementation of the `gfx` module, such that it can be
imported by the user’s code. To do this, we use the `.addModule()` method on the
bulder.

```javascript
builder.addModule('gfx', {
  'gfx.script': "the contents of the module as one big string"
});
```

In the repository, this string is defined in
[`code.js`](https://github.com/plexicode/CommonScript/blob/main/samples/gfxwidget/files/code.js)
but amounts to this:

```javascript
function drawRectangle(x, y, w, h, r, g, b) {
   $draw_rectangle([x, y, w, h, r, g, b].join(','));
}

function drawPixel(x, y, r, g, b) {
   $draw_rectangle([x, y, 2, 2, r, g, b].join(','));
}

function drawCircle(centerX, centerY, rad, r, g, b) {
   $draw_ellipse([centerX - rad, centerY - rad, rad * 2, rad * 2, r, g, b].join(','));
}

function drawEllipse(left, top, width, height, r, g, b) {
   $draw_ellipse([left, top, width, height, r, g, b].join(','));
}

function fill(r, g, b) {
   s = [0, 0, $screen_width(), $screen_height(), r, g, b].join(',');
   $draw_rectangle(s);
}

function getScreenWidth() {
   return tryParseInt($screen_width());
}

function getScreenHeight() {
   return tryParseInt($screen_height());
}
```

Note that the draw methods use `.join(',')` to serialize the inputs into a
single string before they are sent to the main thread and the
`getScreenWidth`/`Height` functions use `tryParseInt()` on the output of their
extension implementations.

Finally, we provide a way for standard output to appear to the user. This is
done using the `.onStdOut` method.

```javascript
builder.onStdOut(text => {
  let row = document.createElement('div');
  row.append(text);
  document.getElementById('output').append(row);
});
```

Once the full configuration is provided, `.build()` creates a new engine
instance. This same engine instance can be used for each successive click of the
“Run” button. However, note that if there were multiple widgets on the page,
you’d likely need to either create multiple environments so that the extensions
can point to the corresponding canvases and output correctly.

For the background worker, little configuration is needed. We only add the
declaration of the environment name/version and our intention for it to start
without any further configuration:

```javascript
CommonScriptBgRunner('GfxWidget', 'HEAD').start();
```

However, if our widget required background extensions, they would be configured
here.

For simplicity on our server, we add the above line of code to the end of the
`web-bgworker.js` file. However in your own configuration and server setup
multiple files can be included using JavaScript's `importScripts()` function.
Base64 data-uri URLs are not recommended, though, as this file is somewhat
large and can also break browser caching.

## Regarding cross-thread latency

Thread communication between the main thread and background web worker is
*relatively quick*. However it is not instantaneous. Creating an extension
design that requires back-and-forth chattiness will ultimately feel sluggish.

Good extension design is critical for minimizing latency. Extensions should be
designed to use batch-like communication or cahcing. For example, if one were to
build a web-based game in CommonScript that ran at 60FPS using a similar
graphics module, this is possible with some modifications.

For games in particular, all graphical write commands should be batched into a
single buffer. At the end of the game frame, the buffer can be flushed in unison
to the main thread. The return value of that extension can be a similar buffer
containing all game input events from the user. The overall amount of time spent
on thread interop is limited to a single round-trip.
