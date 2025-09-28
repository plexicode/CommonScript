const BUILTIN_GFX_LIB = `
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
`.trim();

const ORIGINAL_CODE = `
import gfx;
import math;
import random;

function main(args) {
    width = gfx.getScreenWidth();
    height = gfx.getScreenHeight();
    centerX = width / 2;
    centerY = height / 2;
    radius = (width < height ? width : height) * 0.4;
    dotCount = random.randomInt(3, 50);
    dotRadius = 100 / dotCount;

    colors = [
        [255, 0, 0], // red
        [255, 128, 0], // orange
        [255, 255, 0], // yellow
        [0, 255, 0], // lime
        [0, 128, 0], // gren
        [0, 128, 255], // light blue
        [0, 0, 255], // blue
        [128, 0, 128], // purple
        [255, 0, 255], // magenta
        [0, 0, 0], // black
        [128, 128, 128], // gray
        [255, 255, 255], // white
    ];
    random.shuffle(colors);
    bgColor = colors.pop();
    dotColor = colors.pop();

    gfx.fill(bgColor[0], bgColor[1], bgColor[2]);
    for (i = 0; i < dotCount; i++) {
        angle = 2 * 3.14159 * i / dotCount;
        x = floor(centerX + math.cos(angle) * radius);
        y = floor(centerY + math.sin(angle) * radius);
        gfx.drawCircle(
          x, y, dotRadius,
          dotColor[0], dotColor[1], dotColor[2]);
    }
}

`.trim();
