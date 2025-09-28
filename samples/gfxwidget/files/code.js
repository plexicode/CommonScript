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

function main(args) {
    width = gfx.getScreenWidth();
    height = gfx.getScreenHeight();
    centerX = width / 2;
    centerY = height / 2;
    radius = (width < height ? width : height) * 0.4;
    pointCount = 20;

    gfx.fill(0, 0, 0);
    for (i = 0; i < pointCount; i++) {
        angle = 2 * 3.14159 * i / pointCount;
        x = floor(centerX + math.cos(angle) * radius);
        y = floor(centerY + math.sin(angle) * radius);
        gfx.drawCircle(x, y, 4, 255, 255, 255);
    }
}
`.trim();
