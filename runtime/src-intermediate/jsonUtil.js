let jsonUtil_flatten = (o, buf) => {
    switch (typeof o) {
        case 'boolean': return buf.push(1, o);
        case 'number':
            let n = o + '';
            return buf.push(n.indexOf('.') === -1 ? 2 : 3, n);
        case 'string': return buf.push(4, o);
        case 'object':
            if (o === null) return buf.push(0);
            if (Array.isArray(o)) {
                buf.push(5, o.length);
                for (let i = 0; i < o.length; i++) {
                    jsonUtil_flatten(o[i], buf);
                }
            } else {
                let keys = Object.keys(o);
                buf.push(6, keys.length);
                for (let i = 0; i < keys.length; i++) {
                    buf.push(keys[i]);
                    jsonUtil_flatten(o[keys[i]], buf);
                }
            }
            return;
    }
};
let jsonUtil_parseToArray = (str, bufOut) => {
    try {
        let root = JSON.parse(str);
        bufOut.push(true);
        jsonUtil_flatten(root, bufOut);
    } catch (ex) {
        bufOut.push(false, 1, 1); // TODO: parse out line and col
    }
};
