let EXT = {
    dom_create_element: (task, utils, args) => {
        let tag = utils.valueConverter.valueToReadableString(args[0]);
        let e = document.createElement(tag);
        return utils.valueConverter.wrapNativeHandle(e);
    },
    dom_set_attr: (task, utils, args) => {
        let e = utils.valueConverter.unwrapNativeHandle(args[0]);
        let attr = utils.valueConverter.valueToReadableString(args[1]);
        let value = utils.valueConverter.valueToReadableString(args[2]);
        e[attr] = value;
        return null;
    },
    dom_set_style_attr: (task, utils, args) => {
        let e = utils.valueConverter.unwrapNativeHandle(args[0]);
        let attr = utils.valueConverter.valueToReadableString(args[1]);
        let value = utils.valueConverter.valueToReadableString(args[2]);
        e.style[attr] = value;
        return null;
    },
    dom_get_element_by_id: (task, utils, args) => {
        let id = utils.valueConverter.valueToReadableString(args[0]);
        return utils.valueConverter.wrapNativeHandle(document.getElementById(id));
    },
    dom_append: (task, utils, args) => {
        let parent = utils.valueConverter.unwrapNativeHandle(args[0]);
        let type = utils.valueConverter.valueToReadableString(args[2]);
        switch (type) {
            case 'string':
                parent.append(utils.valueConverter.valueToReadableString(args[1]));
                return null;
            case 'native':
                parent.append(utils.valueConverter.unwrapNativeHandle(args[1]));
                return null;
        }
        throw new Error(".append() not implemented for type: '" + type + "'");
    },
};
